// <copyright file="DividingCoder.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SimpleBase;

/// <summary>
/// Generic dividing Encoding/Decoding implementation to be used by other dividing encoders.
/// </summary>
/// <remarks>
/// Dividing encoding schemes can't encode prefixing zeroes due to mathematical insignificance
/// of them. So they're always encoded as hardcoded zero characters at the beginning.
/// </remarks>
/// <remarks>
/// Creates a new instance of DividingCoder with a given alphabet.
/// </remarks>
/// <param name="alphabet">Alphabet to use. The length of alphabet is used as a divisor.</param>
public abstract class DividingCoder<TAlphabet>(TAlphabet alphabet)
    : IBaseCoder, INonAllocatingBaseCoder
    where TAlphabet: CodingAlphabet
{
    readonly int reductionFactor = Convert.ToInt32(1000 * Math.Log2(alphabet.Length) / 8);
    readonly char zeroChar = alphabet.Value[0];

    /// <summary>
    /// Gets the encoding alphabet.
    /// </summary>
    public TAlphabet Alphabet { get; } = alphabet;

    /// <inheritdoc/>
    public virtual int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        return getSafeByteCountForDecoding(text.Length, countPrefixChars(text, zeroChar));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int getSafeByteCountForDecoding(int textLen, int zeroPrefixLen)
    {
        return zeroPrefixLen + ((textLen - zeroPrefixLen) * reductionFactor / 1000) + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int countPrefixChars(ReadOnlySpan<char> text, char zeroChar)
    {
        return text.IndexOfAnyExcept(zeroChar) switch
        {
            -1 => text.Length,
            int index => index
        };
    }

    /// <inheritdoc/>
    public virtual int GetSafeCharCountForEncoding(ReadOnlySpan<byte> bytes)
    {
        return getSafeCharCountForEncoding(bytes.Length, Bits.CountPrefixingZeroes(bytes));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        int zeroPrefixLen = Bits.CountPrefixingZeroes(bytes);
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
        return internalEncode(input, output, Bits.CountPrefixingZeroes(input), out numCharsWritten);
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

        bytesWritten = rangeWritten.End.Value - rangeWritten.Start.Value;
        return result is (DecodeResult.Success, _);
    }

    bool internalEncode(
        ReadOnlySpan<byte> input,
        Span<char> output,
        int zeroPrefixLen,
        out int charsWritten)
    {
        if (input.Length == 0)
        {
            charsWritten = 0;
            return true;
        }

        ReadOnlySpan<char> alphabet = Alphabet.Value;
        if (zeroPrefixLen > 0)
        {
            output[..zeroPrefixLen].Fill(zeroChar);
        }

        var payload = input[zeroPrefixLen..];
        if (payload.Length == 0)
        {
            charsWritten = zeroPrefixLen;
            return true;
        }

        int padCount = (payload.Length + 3) / 4;
        int alignedByteSize = padCount * 4;
        int divisor = alphabet.Length;

        Span<uint> pads = alignedByteSize < Bits.SafeStackMaxAllocSize 
            ? stackalloc uint[padCount] 
            : new uint[padCount];

        pads.Clear();

        var padsAsBytes = MemoryMarshal.AsBytes(pads);
        int padding = alignedByteSize - payload.Length;
        payload.CopyTo(padsAsBytes[padding..]);

        if (BitConverter.IsLittleEndian)
        {
            BinaryPrimitives.ReverseEndianness(pads, pads);
        }

        int digitCount = 0;
        int startPad = 0;

        while (startPad < padCount)
        {
            if (pads[startPad] == 0)
            {
                startPad++;
                continue;
            }

            ulong remainder = 0;
            for (int i = startPad; i < padCount; i++)
            {
                ulong temp = (remainder << 32) | pads[i];
                pads[i] = (uint)(temp / (uint)divisor);
                remainder = temp % (uint)divisor;
            }

            int writePos = zeroPrefixLen + digitCount;
            if (writePos >= output.Length)
            {
                charsWritten = 0;
                return false;
            }

            output[writePos] = (char)remainder;
            digitCount++;
        }

        var digits = output.Slice(zeroPrefixLen, digitCount);
        digits.Reverse();

        for (int i = 0; i < digitCount; i++)
        {
            digits[i] = alphabet[digits[i]];
        }

        charsWritten = zeroPrefixLen + digitCount;
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
        int divisor = Alphabet.Length;

        var payload = input[zeroPrefixLen..];
        if (payload.Length == 0)
        {
            if (zeroPrefixLen > 0)
            {
                output[..zeroPrefixLen].Clear();
            }
            rangeWritten = ..zeroPrefixLen;
            return (DecodeResult.Success, null);
        }

        int capacityBytes = (payload.Length * reductionFactor / 1000) + 1;
        int capacityPads = (capacityBytes + 3) / 4;

        Span<uint> pads = (capacityPads * 4) < Bits.SafeStackMaxAllocSize 
            ? stackalloc uint[capacityPads] 
            : new uint[capacityPads];

        pads.Clear();

        for (int i = 0; i < payload.Length; i++)
        {
            char c = payload[i];
            int val = table[c] - 1;
            if (val < 0)
            {
                rangeWritten = ..0;
                return (DecodeResult.InvalidCharacter, c);
            }

            ulong carry = (ulong)val;
            for (int j = capacityPads - 1; j >= 0; j--)
            {
                ulong temp = (ulong)pads[j] * (uint)divisor + carry;
                pads[j] = (uint)temp;
                carry = temp >> 32;
            }

            if (carry > 0)
            {
                rangeWritten = ..0;
                return (DecodeResult.InsufficientOutputBuffer, null);
            }
        }

        if (BitConverter.IsLittleEndian)
        {
            BinaryPrimitives.ReverseEndianness(pads, pads);
        }

        var resultBytes = MemoryMarshal.AsBytes(pads);

        int skip = 0;
        while (skip < resultBytes.Length && resultBytes[skip] == 0)
        {
            skip++;
        }

        int payloadLen = resultBytes.Length - skip;
        int totalLen = zeroPrefixLen + payloadLen;

        if (totalLen > output.Length)
        {
            rangeWritten = ..0;
            return (DecodeResult.InsufficientOutputBuffer, null);
        }

        if (zeroPrefixLen > 0)
        {
            output[..zeroPrefixLen].Clear();
        }

        resultBytes[skip..].CopyTo(output[zeroPrefixLen..]);

        rangeWritten = ..totalLen;
        return (DecodeResult.Success, null);
    }
}
