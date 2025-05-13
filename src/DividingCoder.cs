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
/// Dividing encoding schemes can't encode prefixing zeroes due to mathematical insignificance
/// of them. So they're always encoded as hardcoded zero characters at the beginning.
/// </remarks>
public abstract class DividingCoder<TAlphabet> : IBaseCoder, INonAllocatingBaseCoder
    where TAlphabet: CodingAlphabet
{
    readonly int divisor;
    readonly int reductionFactor;
    readonly char zeroChar;

    /// <summary>
    /// Gets the encoding alphabet.
    /// </summary>
    public TAlphabet Alphabet { get; }

    /// <summary>
    /// Creates a new instance of DividingCoder with a given alphabet.
    /// </summary>
    /// <param name="alphabet">Alphabet to use. The length of alphabet is used as a divisor.</param>
    public DividingCoder(TAlphabet alphabet)
    {
        Alphabet = alphabet;
        zeroChar = alphabet.Value[0];
        divisor = alphabet.Length;
        reductionFactor = Convert.ToInt32(1000 * Math.Log2(divisor) / 8);
    }

    /// <inheritdoc/>
    public virtual int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        return getSafeByteCountForDecoding(text.Length, countPrefixChars(text, zeroChar));
    }

    int getSafeByteCountForDecoding(int textLen, int zeroPrefixLen)
    {
        return zeroPrefixLen + ((textLen - zeroPrefixLen) * reductionFactor / 1000) + 1;
    }

    static int countPrefixChars(ReadOnlySpan<char> text, char zeroChar)
    {
        int count = 0;
        while (count < text.Length && text[count] == zeroChar)
        {
            count += 1;
        }
        return count;
    }

    static int countPrefixZeroes(ReadOnlySpan<byte> bytes)
    {
        int count = 0;
        while (count < bytes.Length && bytes[count] == 0)
        {
            count += 1;
        }
        return count;
    }

    /// <inheritdoc/>
    public virtual int GetSafeCharCountForEncoding(ReadOnlySpan<byte> bytes)
    {
        return getSafeCharCountForEncoding(bytes.Length, countPrefixZeroes(bytes));
    }

    int getSafeCharCountForEncoding(int bytesLen, int zeroPrefixLen)
    {
        return zeroPrefixLen + ((bytesLen - zeroPrefixLen) * 1000 / reductionFactor) + 1;
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

        int zeroPrefixLen = countPrefixZeroes(bytes);
        int outputLen = getSafeCharCountForEncoding(bytes.Length, zeroPrefixLen);

        // we can't use `String.Create` here to reduce allocations because
        // Spans aren't supported in lambda expressions.
        Span<char> output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc char[outputLen] : new char[outputLen];

        return internalEncode(bytes, output, zeroPrefixLen, out int numCharsWritten)
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

        int zeroPrefixLen = countPrefixChars(text, zeroChar);
        int outputLen = getSafeByteCountForDecoding(text.Length, zeroPrefixLen);
        Span<byte> output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc byte[outputLen] : new byte[outputLen];
        var result = internalDecode(text, output, zeroPrefixLen, out Range rangeWritten);

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
        return internalEncode(input, output, zeroPrefixLen: -1, out numCharsWritten);
    }

    /// <inheritdoc/>
    public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        if (input.Length == 0)
        {
            bytesWritten = 0;
            return true;
        }

        int zeroPrefixLen = countPrefixChars(input, zeroChar);
        var result = internalDecode(
            input,
            output,
            zeroPrefixLen,
            out Range rangeWritten);

        output[rangeWritten].CopyTo(output);
        bytesWritten = rangeWritten.End.Value - rangeWritten.Start.Value;
        return result is (DecodeResult.Success, _);
    }

    static DecodeResult decodeZeroes(Span<byte> output, int length, out int bytesWritten)
    {
        if (length > output.Length)
        {
            bytesWritten = 0;
            return DecodeResult.InsufficientOutputBuffer;
        }

        output[..length].Clear();
        bytesWritten = length;
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
        int zeroPrefixLen,
        out int numCharsWritten)
    {
        if (input.Length == 0)
        {
            numCharsWritten = 0;
            return true;
        }

        ReadOnlySpan<char> alphabet = Alphabet.Value;
        if (zeroPrefixLen < 0)
        {
            // zero prefix isn't already calculated - so we do zero encoding while counting
            for (zeroPrefixLen = 0; zeroPrefixLen < input.Length && input[zeroPrefixLen] == 0; zeroPrefixLen++)
            {
                output[zeroPrefixLen] = zeroChar;
            }
        }
        else if (zeroPrefixLen > 0)
        {
            // fast fill because we already know prefix count beforehand
            output[..zeroPrefixLen].Fill(zeroChar);
        }

        int numDigits = 0;
        int index = zeroPrefixLen;
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

        translatedCopy(output[^numDigits..], output[zeroPrefixLen..], alphabet);
        numCharsWritten = zeroPrefixLen + numDigits;
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
        int zeroPrefixLen,
        out Range rangeWritten)
    {
        var table = Alphabet.ReverseLookupTable;
        int min = output.Length;
        for (int i = zeroPrefixLen; i < input.Length; i++)
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

        if (zeroPrefixLen > 0)
        {
            output[(min - zeroPrefixLen)..min].Clear();
            min -= zeroPrefixLen;
        }

        rangeWritten = min..output.Length;
        return (DecodeResult.Success, null);
    }
}
