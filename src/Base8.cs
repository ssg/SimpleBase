// <copyright file="Base8.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleBase;

/// <summary>
/// Creates a new instance of Base8 with a given alphabet.
/// </summary>
/// <remarks>Currently doesn't support alternative alphabets.</remarks>
public class Base8 : IBaseCoder, INonAllocatingBaseCoder, IBaseStreamCoder
{
    static readonly Lazy<Base8> @default = new(() => new Base8());

    const int encodedPadSize = 8;
    const int decodedPadSize = 3;

    const char zeroChar = '0';

    /// <summary>
    /// Default Base8 implementation.
    /// </summary>
    public static Base8 Default => @default.Value;

    /// <inheritdoc/>
    public byte[] Decode(ReadOnlySpan<char> text)
    {
        int outputLen = getSafeByteCountForDecoding(text.Length);
        var output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc byte[outputLen] : new byte[outputLen];
        if (!internalDecode(text, output, out int bytesWritten))
        {
            throw new ArgumentException("Invalid Base8 character encountered", nameof(text));
        }
        return output[..bytesWritten].ToArray();
    }

    /// <inheritdoc/>
    public string Encode(ReadOnlySpan<byte> bytes)
    {
        int outputLen = getSafeCharCountForEncoding(bytes.Length);
        var output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc char[outputLen] : new char[outputLen];
        int charsWritten = internalEncode(bytes, output);
        return new string(output[..charsWritten]);
    }

    /// <inheritdoc/>
    public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        return getSafeByteCountForDecoding(text.Length);
    }

    static int getSafeByteCountForDecoding(int textLength)
    {
        return (textLength + encodedPadSize - 1) / encodedPadSize * decodedPadSize;
    }

    /// <inheritdoc/>
    public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> buffer)
    {
        return getSafeCharCountForEncoding(buffer.Length);
    }

    static int getSafeCharCountForEncoding(int bytesLength)
    {
        return (bytesLength + decodedPadSize - 1) / decodedPadSize * encodedPadSize;
    }

    /// <inheritdoc/>
    public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        bytesWritten = 0;
        if (output.Length < getSafeByteCountForDecoding(input.Length))
        {
            return false;
        }
        if (input.Length == 0)
        {
            return true;
        }

        return internalDecode(input, output, out bytesWritten);
    }

    static bool internalDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        bytesWritten = 0;
        if ((input.Length % encodedPadSize) is not 0 and not 6)
        {
            // invalid input length
            return false;
        }

        for (int i = 0; i < input.Length; i += encodedPadSize)
        {
            byte b0 = (byte)(input[i] - zeroChar);
            byte b1 = (byte)(input[i + 1] - zeroChar);
            byte b2 = (byte)(input[i + 2] - zeroChar);
            byte b3 = (byte)(input[i + 3] - zeroChar);
            byte b4 = (byte)(input[i + 4] - zeroChar);
            byte b5 = (byte)(input[i + 5] - zeroChar);
            if (b0 > 7 || b1 > 7 || b2 > 7 || b3 > 7 || b4 > 7 || b5 > 7)
            {
                // invalid character
                return false;
            }
            output[bytesWritten++] = (byte)((b0 << 5) | (b1 << 2) | (b2 >> 1));
            output[bytesWritten++] = (byte)(((b2 & 1) << 7) | (b3 << 4) | (b4 << 1) | (b5 >> 2));
            if (i + 6 >= input.Length)
            {
                // per spec:
                // "If there are not enough bits to complete the last 8-bit word then drop that last incomplete 8-bit word."
                // https://github.com/multiformats/multibase/blob/master/rfcs/Base8.md
                break;
            }
            byte b6 = (byte)(input[i + 6] - zeroChar);
            byte b7 = (byte)(input[i + 7] - zeroChar);
            if (b6 > 7 || b7 > 7)
            {
                // invalid character
                return false;
            }
            output[bytesWritten++] = (byte)(((b5 & 3) << 6) | (b6 << 3) | (b7 >> 0));
        }

        return true;
    }

    /// <inheritdoc/>
    public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int numCharsWritten)
    {
        if (output.Length < getSafeCharCountForEncoding(input.Length))
        {
            numCharsWritten = 0;
            return false;
        }

        numCharsWritten = internalEncode(input, output);
        return true;
    }

    static int internalEncode(ReadOnlySpan<byte> input, Span<char> output)
    {
        int o = 0;
        for (int i = 0; i < input.Length; i += decodedPadSize)
        {
            byte b0 = input[i];
            byte b1 = (i < input.Length - 1) ? input[i + 1] : (byte)0;
            byte b2 = (i < input.Length - 2) ? input[i + 2] : (byte)0;
            output[o++] = (char)((b0 >> 5) + zeroChar);
            output[o++] = (char)(((b0 >> 2) & 0b111) + zeroChar);
            output[o++] = (char)(((b0 << 1) & 0b111) + ((b1 >> 7) & 1) + zeroChar);
            output[o++] = (char)(((b1 >> 4) & 0b111) + zeroChar);
            output[o++] = (char)(((b1 >> 1) & 0b111) + zeroChar);
            output[o++] = (char)(((b1 << 2) & 0b111) + ((b2 >> 6) & 3) + zeroChar);
            output[o++] = (char)(((b2 >> 3) & 0b111) + zeroChar);
            output[o++] = (char)(((b2 << 0) & 0b111) + zeroChar);
        }

        return o;
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
    public void Decode(TextReader input, Stream output)
    {
        StreamHelper.Decode(input, output, input => Decode(input.Span));
    }

    /// <inheritdoc/>
    public async Task DecodeAsync(TextReader input, Stream output)
    {
        await StreamHelper.DecodeAsync(input, output, input => Decode(input.Span));
    }
}
