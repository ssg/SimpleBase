// <copyright file="Base2.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleBase;

/// <summary>
/// Base2 implementation that encodes a byte array into 1 and 0's by converting
/// each byte indiviudually by encoding most-significant byte first.
/// </summary>
public class Base2 : IBaseCoder, INonAllocatingBaseCoder, IBaseStreamCoder
{
    private static readonly Lazy<Base2> @default = new(() => new Base2());

    /// <summary>
    /// Default instance of Base2
    /// </summary>
    public static Base2 Default => @default.Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Base2"/> class.
    /// </summary>
    public Base2()
    {        
    }

    /// <inheritdoc/>
    public byte[] Decode(ReadOnlySpan<char> text)
    {
        if (text.Length % 8 != 0)
        {
            throw new ArgumentException("Input length must be multiply of 8", nameof(text));
        }

        var output = new byte[text.Length / 8];
        if (!internalDecode(text, output, out int bytesWritten))
        {
            throw new ArgumentException("Invalid Base2 character encountered", nameof(text));
        }

        return output;
    }

    /// <inheritdoc/>
    public void Decode(TextReader input, Stream output)
    {
        StreamHelper.Decode(input, output, input => Decode(input.Span));
    }

    /// <inheritdoc/>
    public async Task DecodeAsync(TextReader input, Stream output)
    {
        await StreamHelper.DecodeAsync(input, output, input => Decode(input.Span));
    }

    /// <inheritdoc/>
    public string Encode(ReadOnlySpan<byte> bytes)
    {
        int outputLen = bytes.Length * 8;
        var output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc char[outputLen] : new char[outputLen];
        internalEncode(bytes, output);
        return new string(output);
    }

    /// <inheritdoc/>
    public void Encode(Stream input, TextWriter output)
    {
        StreamHelper.Encode(input, output, (input, lastBlock) => Encode(input.Span));
    }

    /// <inheritdoc/>
    public async Task EncodeAsync(Stream input, TextWriter output)
    {
        await StreamHelper.EncodeAsync(input, output, (input, lastBlock) => Encode(input.Span));
    }

    /// <inheritdoc/>
    public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        return text.Length / 8;
    }

    /// <inheritdoc/>
    public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> buffer)
    {
        return buffer.Length * 8;
    }

    /// <inheritdoc/>
    public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        bytesWritten = 0;
        int inputLen = input.Length;
        if (inputLen == 0)
        {
            return true;
        }

        (int numBlocks, int remainder) = Math.DivRem(inputLen, 8);
        if (remainder != 0 || output.Length < numBlocks)
        {
            // we don't handle non-canonical encoding yet
            return false;
        }

        return internalDecode(input, output, out bytesWritten);
    }

    bool internalDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        bytesWritten = 0;
        byte pad = 0;
        for (int i = 0; i < input.Length; i++)
        {
            int c = input[i] - '0';
            if ((c & 1) != c)
            {
                // invalid character
                return false;
            }
            int shift = 7 - (i % 8);
            pad |= (byte)(c << shift);
            if (shift == 0)
            {
                output[bytesWritten++] = pad;
                pad = 0;
            }
        }
        return true;
    }

    /// <inheritdoc/>
    public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int numCharsWritten)
    {
        numCharsWritten = 0;
        int targetLen = input.Length * 8;
        if (output.Length < targetLen)
        {
            return false;
        }

        internalEncode(input, output);
        numCharsWritten = targetLen;
        return true;
    }

    static void internalEncode(ReadOnlySpan<byte> input, Span<char> output)
    {
        for (int i = 0, o = 0; i < input.Length; i++, o += 8)
        {
            var b = input[i];
            output[o + 0] = (b & 0b1000_0000) != 0 ? '1' : '0';
            output[o + 1] = (b & 0b0100_0000) != 0 ? '1' : '0';
            output[o + 2] = (b & 0b0010_0000) != 0 ? '1' : '0';
            output[o + 3] = (b & 0b0001_0000) != 0 ? '1' : '0';
            output[o + 4] = (b & 0b0000_1000) != 0 ? '1' : '0';
            output[o + 5] = (b & 0b0000_0100) != 0 ? '1' : '0';
            output[o + 6] = (b & 0b0000_0010) != 0 ? '1' : '0';
            output[o + 7] = (b & 0b0000_0001) != 0 ? '1' : '0';
        }
    }
}
