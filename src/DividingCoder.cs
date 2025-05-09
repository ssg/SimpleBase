// <copyright file="DividingCoder.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Diagnostics;

namespace SimpleBase;

/// <summary>
/// Generic dividing Encoding/Decoding implementation to be used by other dividing encoders. 
/// </summary>
/// <remarks>
/// This isn't used by Base58 because it handles zero-prefixes differently than other encodings.
/// </remarks>
/// <param name="alphabet">Alphabet to use.</param>
/// <param name="divisor">The divisor to be used in the encoding.</param>
/// <param name="reductionFactor">Factor to calculate buffer size reduction after decoding multiplied by 1000 (e.g. 733 for base58, 750 for base62)</param>
public abstract class DividingCoder<TAlphabet>(TAlphabet alphabet, int divisor, int reductionFactor)
    : IBaseCoder, INonAllocatingBaseCoder
    where TAlphabet: CodingAlphabet
{
    /// <summary>
    /// Gets the encoding alphabet.
    /// </summary>
    public TAlphabet Alphabet { get; } = alphabet;

    /// <summary>
    /// Retrieve safe byte count while avoiding multiple counting operations.
    /// </summary>
    /// <param name="textLen">Length of text.</param>
    /// <returns>Length of safe allocation.</returns>
    int getSafeByteCountForDecoding(int textLen)
    {
        return (textLen * reductionFactor / 1000) + 1;
    }

    /// <inheritdoc/>
    public virtual int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        return getSafeByteCountForDecoding(text.Length);
    }

    /// <inheritdoc/>
    public virtual int GetSafeCharCountForEncoding(ReadOnlySpan<byte> bytes)
    {
        int bytesLen = bytes.Length;

        return (bytesLen * 1000 / reductionFactor) + 1;
    }

    /// <summary>
    /// Encode to given base
    /// </summary>
    /// <param name="bytes">Bytes to encode.</param>
    /// <returns>Encoded string.</returns>
    public string Encode(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
        {
            return string.Empty;
        }

        int outputLen = GetSafeCharCountForEncoding(bytes);

        // we can't use `String.Create` here to reduce allocations because
        // Spans aren't supported in lambda expressions.
        Span<char> output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc char[outputLen] : new char[outputLen];

        return internalEncode(bytes, output, out int numCharsWritten)
            ? new string(output[..numCharsWritten])
            : throw new InvalidOperationException("Output buffer with insufficient size generated");
    }

    /// <summary>
    /// Decode from a given base
    /// </summary>
    /// <param name="text">Encoded text.</param>
    /// <returns>Decoded bytes.</returns>
    public byte[] Decode(ReadOnlySpan<char> text)
    {
        if (text.Length == 0)
        {
            return [];
        }

        int outputLen = getSafeByteCountForDecoding(text.Length);
        Span<byte> output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc byte[outputLen] : new byte[outputLen];
        var result = internalDecode(text, output, out Range rangeWritten);

        return result switch
        {
            (DecodeResult.InvalidCharacter, char c) => throw CodingAlphabet.InvalidCharacter(c),
            (DecodeResult.InsufficientOutputBuffer, _) => throw new InvalidOperationException("Output buffer was too small while decoding"),
            (DecodeResult.Success, _) => output[rangeWritten].ToArray(),
            _ => throw new InvalidOperationException("This should be never hit - probably a bug"),
        };
    }

    /// <inheritdoc/>
    public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int numCharsWritten)
    {
        return internalEncode(input, output, out numCharsWritten);
    }

    /// <inheritdoc/>
    public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int numBytesWritten)
    {
        if (input.Length == 0)
        {
            numBytesWritten = 0;
            return true;
        }

        var result = internalDecode(
            input,
            output,
            out Range rangeWritten);

        output[rangeWritten].CopyTo(output);
        numBytesWritten = rangeWritten.End.Value - rangeWritten.Start.Value;
        return result is (DecodeResult.Success, _);
    }

    static DecodeResult decodeZeroes(Span<byte> output, int length, out int numBytesWritten)
    {
        if (length > output.Length)
        {
            numBytesWritten = 0;
            return DecodeResult.InsufficientOutputBuffer;
        }

        output[..length].Clear();
        numBytesWritten = length;
        return DecodeResult.Success;
    }

    static void translatedCopy(
        ReadOnlySpan<char> source,
        Span<char> destination,
        ReadOnlySpan<char> alphabet)
    {
        Debug.Assert(source.Length <= destination.Length, "source is too big");
        for (int n = 0; n < source.Length; n++)
        {
            destination[n] = alphabet[source[n]];
        }
    }

    bool internalEncode(
        ReadOnlySpan<byte> input,
        Span<char> output,
        out int numCharsWritten)
    {
        if (input.Length == 0)
        {
            numCharsWritten = 0;
            return true;
        }

        ReadOnlySpan<char> alphabet = Alphabet.Value;

        int numDigits = 0;
        int index = 0;
        while (index < input.Length)
        {
            int carry = input[index++];
            int i = 0;
            for (int j = output.Length - 1; (carry != 0 || i < numDigits)
                && j >= 0; j--, i++)
            {
                carry += output[j] << 8;
                carry = Math.DivRem(carry, divisor, out int remainder);
                output[j] = (char)remainder;
            }

            numDigits = i;
        }

        translatedCopy(output[^numDigits..], output, alphabet);
        numCharsWritten = numDigits;
        return true;
    }

    enum DecodeResult
    {
        Success,
        InvalidCharacter,
        InsufficientOutputBuffer,
    }

    (DecodeResult, char?) internalDecode(
        ReadOnlySpan<char> input,
        Span<byte> output,
        out Range rangeWritten)
    {
        var table = Alphabet.ReverseLookupTable;
        int min = output.Length - 1;
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            int carry = table[c] - 1;
            if (carry < 0)
            {
                rangeWritten = ..0;
                return (DecodeResult.InvalidCharacter, c);
            }

            for (int o = output.Length - 1; o >= 0; o--)
            {
                carry += divisor * output[o];
                output[o] = (byte)carry;
                if (min > o && carry != 0)
                {
                    min = o;
                }

                carry >>= 8;
            }
        }

        rangeWritten = min..output.Length;
        return (DecodeResult.Success, null);
    }
}
