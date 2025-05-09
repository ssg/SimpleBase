// <copyright file="Base45.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Base45 encoding/decoding implementation.
/// </summary>
/// <param name="alphabet">Alphabet to use.</param>
public sealed class Base45(Base45Alphabet alphabet) : INonAllocatingBaseCoder, IBaseCoder
{
    static readonly Lazy<Base45> @default = new(() => new(Base45Alphabet.Default));

    /// <summary>
    /// Gets the default flavor.
    /// </summary>
    public static Base45 Default => @default.Value;

    /// <inheritdoc/>
    public byte[] Decode(ReadOnlySpan<char> text)
    {
        int outputLen = getDecodingBufferSize(text.Length);

        // since we can calculate exact length of the output, we don't need 
        byte[] output = new byte[outputLen];
        return internalDecode(text, output, out int bytesWritten) switch
        {
            (DecodeResult.Success, _)  => bytesWritten == output.Length ? output : throw new InvalidOperationException("Inconsistent buffer size returned -- probably a bug"),
            (DecodeResult.InvalidOutputLength, _) => throw new InvalidOperationException("Failed to allocate sufficient buffer -- likely a bug"),
            (DecodeResult.InvalidCharacter, char c) => throw CodingAlphabet.InvalidCharacter(c),
            (DecodeResult.InvalidInput, _) => throw new ArgumentException("Input buffer is incorrectly encoded or corrupt", nameof(text)),
            (DecodeResult.InvalidInputLength, _) => throw new ArgumentException("Input buffer is at incorrect size", nameof(text)),
            _ => throw new NotSupportedException(),
        };
    }

    /// <inheritdoc/>
    public string Encode(ReadOnlySpan<byte> bytes)
    {
        int outputLen = getEncodingBufferSize(bytes.Length);
        Span<char> output = outputLen < Bits.SafeStackMaxAllocSize
            ? stackalloc char[outputLen]
            : new char[outputLen];
        if (!TryEncode(bytes, output, out int numCharsWritten))
        {
            throw new InvalidOperationException("Failed to allocate large enough buffer -- likely a bug");
        }
        return output.ToString();
    }

    /// <inheritdoc/>
    public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text) => getDecodingBufferSize(text.Length);

    private static int getDecodingBufferSize(int len)
    {
        (int a, int b) = Math.DivRem(len, 3);

        // normally, b % 3 == 1 is a bug case and should never happen
        // but we are not expected to throw an exception here so we
        // just assume it's b % 3 == 2 and burn that bridge when we come to it.
        return (a * 2) + (b > 0 ? 1 : 0);
    }

    /// <inheritdoc/>
    public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> buffer) => getEncodingBufferSize(buffer.Length);

    /// <summary>
    /// Gets the exact size of the size of buffer required for encoding.
    /// </summary>
    /// <param name="len">Input buffer length.</param>
    /// <returns>Encoded buffer length.</returns>
    static int getEncodingBufferSize(int len)
    {
        (int wholeBlocks, int remainder) = Math.DivRem(len, 2);
        return (wholeBlocks * 3) + (remainder * 2);
    }

    /// <inheritdoc/>
    public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        (var result, _) = internalDecode(input, output, out bytesWritten);
        return result == DecodeResult.Success;
    }

    enum DecodeResult
    {
        Success,
        InvalidOutputLength,
        InvalidCharacter,
        InvalidInput,
        InvalidInputLength,
    }

    (DecodeResult, char?) internalDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        // we replicate the length calculation here to reduce the number of calculations
        (int wholeBlocks, int remainder) = Math.DivRem(input.Length, 3);
        bytesWritten = 0;
        if (remainder == 1)
        {
            return (DecodeResult.InvalidInputLength, null);
        }
        if (output.Length < (wholeBlocks * 2) + (remainder > 0 ? 1 : 0))
        {
            return (DecodeResult.InvalidOutputLength, null);
        }

        var table = alphabet.ReverseLookupTable;

        // process whole blocks
        int i = 0;
        for (int block = 0; block < wholeBlocks; block++)
        {
            char chr = input[i++];
            int c = table[chr] - 1;
            if (c < 0)
            {
                return (DecodeResult.InvalidCharacter, chr);
            }
            chr = input[i++];
            int d = table[chr] - 1;
            if (d < 0)
            {
                return (DecodeResult.InvalidCharacter, chr);
            }
            chr = input[i++];
            int e = table[chr] - 1;
            if (e < 0)
            {
                return (DecodeResult.InvalidCharacter, chr);
            }

            int value = c + (d * 45) + (e * 45 * 45);
            if (value > 0xFFFF)
            {
                return (DecodeResult.InvalidInput, null);
            }

            output[bytesWritten++] = (byte)(value >> 8);
            output[bytesWritten++] = (byte)(value & 0xFF);
        }

        // process remainder block
        if (remainder == 2)
        {
            char chr = input[i++];
            int c = table[chr] - 1;
            if (c < 0)
            {
                return (DecodeResult.InvalidCharacter, chr);
            }
            chr = input[i++];
            int d = table[chr] - 1;
            if (d < 0)
            {
                return (DecodeResult.InvalidCharacter, chr);
            }
            int value = c + (d * 45);
            if (value > 0xFF)
            {
                return (DecodeResult.InvalidInput, null);
            }
            output[bytesWritten++] = (byte)value;
        }

        return (DecodeResult.Success, null);
    }

    /// <inheritdoc/>
    public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int numCharsWritten)
    {
        (int wholeBlocks, int remainder) = Math.DivRem(input.Length, 2);
        int wholeLen = wholeBlocks * 2;
        numCharsWritten = 0;

        int outputLength = getEncodingBufferSize(input.Length);
        if (output.Length < outputLength)
        {
            return false;
        }

        // process whole blocks first
        int i = 0;
        while (i < wholeLen)
        {            
            ushort a = input[i++];
            ushort b = input[i++];
            ushort value = (ushort)((a << 8) | b); // big-endian per spec
            (ushort quotient, ushort c) = Math.DivRem(value, (ushort)45);
            (ushort e, ushort d) = Math.DivRem(quotient, (ushort)45);
            output[numCharsWritten++] = alphabet.Value[c];
            output[numCharsWritten++] = alphabet.Value[d];
            output[numCharsWritten++] = alphabet.Value[e];
        }

        // process the last byte
        if (remainder == 1)
        {
            (ushort d, ushort c) = Math.DivRem(input[i], (ushort)45);

            output[numCharsWritten++] = alphabet.Value[c];
            output[numCharsWritten++] = alphabet.Value[d];
        }
        return true;
    }
}
