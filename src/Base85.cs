﻿// <copyright file="Base85.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SimpleBase;

/// <summary>
/// Base58 encoding/decoding class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Base85"/> class
/// using a custom alphabet.
/// </remarks>
/// <param name="alphabet">Alphabet to use.</param>
public class Base85(Base85Alphabet alphabet) : IBaseCoder, IBaseStreamCoder, INonAllocatingBaseCoder
{
    const int baseLength = 85;
    const int byteBlockSize = 4;
    const int stringBlockSize = 5;
    const long fourSpaceChars = 0x20202020;
    const int decodeBufferSize = 5120; // don't remember what was special with this number
    static readonly Lazy<Base85> z85 = new(() => new Base85(Base85Alphabet.Z85));
    static readonly Lazy<Base85> ascii85 = new(() => new Base85(Base85Alphabet.Ascii85));
    static readonly Lazy<Base85IPv6> rfc1924 = new(() => new Base85IPv6(Base85Alphabet.Rfc1924));

    /// <summary>
    /// Gets Z85 flavor of Base85.
    /// </summary>
    public static Base85 Z85 => z85.Value;

    /// <summary>
    /// Gets Ascii85 flavor of Base85.
    /// </summary>
    public static Base85 Ascii85 => ascii85.Value;

    /// <summary>
    /// Gets RFC 1924 IPv6 flavor of Base85.
    /// </summary>
    public static Base85IPv6 Rfc1924 => rfc1924.Value;

    /// <summary>
    /// Gets the encoding alphabet.
    /// </summary>
    public Base85Alphabet Alphabet { get; } = alphabet;

    /// <inheritdoc/>
    public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        bool usingShortcuts = Alphabet.AllZeroShortcut is not null || Alphabet.AllSpaceShortcut is not null;
        return getSafeByteCountForDecoding(text.Length, usingShortcuts);
    }

    /// <inheritdoc/>
    public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> bytes)
    {
        return getSafeCharCountForEncoding(bytes.Length);
    }

    /// <summary>
    /// Encode the given bytes into Base85.
    /// </summary>
    /// <param name="bytes">Bytes to encode.</param>
    /// <returns>Encoded text.</returns>
    public string Encode(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
        {
            return string.Empty;
        }

        // we can't use `String.Create` here to reduce allocations because
        // Spans aren't supported in lambda expressions.
        int outputLen = GetSafeCharCountForEncoding(bytes);
        Span<char> output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc char[outputLen] : new char[outputLen];

        return internalEncode(bytes, output, out int numCharsWritten)
            ? new string(output[..numCharsWritten])
            : throw new InvalidOperationException("Insufficient output buffer size while encoding Base85");
    }

    /// <inheritdoc/>
    public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int numCharsWritten)
    {
        int inputLen = input.Length;
        if (inputLen == 0)
        {
            numCharsWritten = 0;
            return true;
        }

        return internalEncode(input, output, out numCharsWritten);
    }

    /// <summary>
    /// Encode a given stream into a text writer.
    /// </summary>
    /// <param name="input">Input stream.</param>
    /// <param name="output">Output writer.</param>
    public void Encode(Stream input, TextWriter output)
    {
        StreamHelper.Encode(input, output, (buffer, lastBlock) => Encode(buffer.Span));
    }

    /// <summary>
    /// Encode a given stream into a text writer.
    /// </summary>
    /// <param name="input">Input stream.</param>
    /// <param name="output">Output writer.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task EncodeAsync(Stream input, TextWriter output)
    {
        await StreamHelper.EncodeAsync(input, output, (buffer, lastBlock) => Encode(buffer.Span))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Decode a text reader into a stream.
    /// </summary>
    /// <param name="input">Input reader.</param>
    /// <param name="output">Output stream.</param>
    public void Decode(TextReader input, Stream output)
    {
        StreamHelper.Decode(input, output, (text) => Decode(text.Span), decodeBufferSize);
    }

    /// <summary>
    /// Decode a text reader into a stream.
    /// </summary>
    /// <param name="input">Input reader.</param>
    /// <param name="output">Output stream.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task DecodeAsync(TextReader input, Stream output)
    {
        await StreamHelper.DecodeAsync(input, output, (text) => Decode(text.Span), decodeBufferSize)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Decode given characters into bytes.
    /// </summary>
    /// <param name="text">Characters to decode.</param>
    /// <returns>Decoded bytes.</returns>
    public byte[] Decode(ReadOnlySpan<char> text)
    {
        if (text.Length == 0)
        {
            return [];
        }

        // allocate a larger buffer if we're using shortcuts
        int decodeBufferLen = getSafeByteCountForDecoding(text.Length, Alphabet.HasShortcut);
        Span<byte> decodeBuffer = decodeBufferLen < Bits.SafeStackMaxAllocSize ? stackalloc byte[decodeBufferLen] : new byte[decodeBufferLen];
        return internalDecode(text, decodeBuffer, out int bytesWritten) switch
        {
            (DecodeResult.Success, _) => decodeBuffer[..bytesWritten].ToArray(),
            (DecodeResult.InvalidCharacter, char c) => throw CodingAlphabet.InvalidCharacter(c),
            (DecodeResult.InvalidShortcut, char c) => throw new ArgumentException($"Invalid location for a shortcut character: {c}", nameof(text)),
            (DecodeResult.InsufficientOutputBuffer, _) => throw new InvalidOperationException("Internal error: pre-allocated insufficient output buffer size"),
            _ => throw new InvalidOperationException("This should be never hit - probably a bug"),
        };
    }

    /// <inheritdoc/>
    public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        return internalDecode(input, output, out bytesWritten) is (DecodeResult.Success, _);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool writeEncodedValue(
        uint block,
        Span<char> output,
        string table,
        int blockLength,
        char? zeroShortcutChar,
        char? spaceShortcutChar,
        out int bytesWritten)
    {
        if (output.Length == 0)
        {
            bytesWritten = 0;
            return false;
        }

        if (block == 0 && zeroShortcutChar is not null)
        {
            output[0] = zeroShortcutChar.Value; // guaranteed to be non-null
            bytesWritten = 1;
            return true;
        }

        if (block == fourSpaceChars && spaceShortcutChar is not null)
        {
            output[0] = spaceShortcutChar.Value; // guaranteed to be non-null
            bytesWritten = 1;
            return true;
        }

        if (blockLength > output.Length)
        {
            bytesWritten = 0;
            return false;
        }

        // map the 4-byte packet to to 5-byte octets
        for (int i = stringBlockSize - 1; i >= 0; i--)
        {
            block = (uint)Math.DivRem(block, baseLength, out long remainder);
            if (i < blockLength)
            {
                output[i] = table[(int)remainder];
            }
        }

        bytesWritten = blockLength;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static DecodeResult writeDecodedValue(
        Span<byte> output,
        long value,
        int numBytesToWrite,
        out int bytesWritten)
    {
        if (numBytesToWrite > output.Length)
        {
            bytesWritten = 0;
            return DecodeResult.InsufficientOutputBuffer;
        }

        int o = 0;
        for (int i = byteBlockSize - 1; i >= 0 && numBytesToWrite > 0; i--, numBytesToWrite--)
        {
            byte b = (byte)((value >> (i * 8)) & 0xFF);
            output[o++] = b;
        }

        bytesWritten = o;
        return DecodeResult.Success;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool isWhiteSpace(char c)
    {
        return c is '\x20' or '\x85' or '\xA0' or (>= '\x09' and <= '\x0D');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static DecodeResult writeShortcut(
        Span<byte> output,
        ref int blockIndex,
        long value,
        out int bytesWritten)
    {
        if (blockIndex != 0)
        {
            bytesWritten = 0;
            return DecodeResult.InvalidShortcut;
        }

        blockIndex = 0; // restart block after the shortcut character
        return writeDecodedValue(output, value, byteBlockSize, out bytesWritten);
    }

    static int getSafeCharCountForEncoding(int bytesLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bytesLength);

#pragma warning disable IDE0046 // Convert to conditional expression - prefer clarity
        if (bytesLength == 0)
        {
            return 0;
        }

        return (bytesLength + byteBlockSize - 1) * stringBlockSize / byteBlockSize;
#pragma warning restore IDE0046 // Convert to conditional expression
    }

    static int getSafeByteCountForDecoding(int textLength, bool usingShortcuts)
    {
        if (usingShortcuts)
        {
            return textLength * byteBlockSize; // max possible size using shortcuts
        }

        // max possible size without shortcuts
        return (((textLength - 1) / stringBlockSize) + 1) * byteBlockSize;
    }

    bool internalEncode(
        ReadOnlySpan<byte> input,
        Span<char> output,
        out int numCharsWritten)
    {
        char? usesZeroShortcut = Alphabet.AllZeroShortcut;
        char? usesSpaceShortcut = Alphabet.AllSpaceShortcut;
        string table = Alphabet.Value;
        int fullLen = input.Length / byteBlockSize * byteBlockSize; // size of whole 4-byte blocks

        int i = 0;
        numCharsWritten = 0;
        while (i < fullLen)
        {
            // build a 32-bit representation of input
            uint block = (((uint)input[i++]) << 24)
                | (((uint)input[i++]) << 16)
                | (((uint)input[i++]) << 8)
                | input[i++];

            if (!writeEncodedValue(
                block,
                output[numCharsWritten..],
                table,
                stringBlockSize,
                usesZeroShortcut,
                usesSpaceShortcut,
                out int numWritten))
            {
                numCharsWritten += numWritten;
                return false;
            }

            numCharsWritten += numWritten;
        }

        // check if a part is remaining
        int remainingBytes = input.Length - fullLen;
        if (remainingBytes == 0)
        {
            return true;
        }

        uint lastBlock = 0;
        for (int n = 0; n < remainingBytes; n++)
        {
            lastBlock |= ((uint)input[i++]) << ((3 - n) * 8);
        }

        if (!writeEncodedValue(
            lastBlock,
            output[numCharsWritten..],
            table,
            remainingBytes + 1,
            usesZeroShortcut,
            usesSpaceShortcut,
            out int numWrittenFinal))
        {
            return false;
        }

        numCharsWritten += numWrittenFinal;
        return true;
    }

    enum DecodeResult
    {
        Success,
        InsufficientOutputBuffer,
        InvalidCharacter,
        InvalidShortcut,
    }

    (DecodeResult, char?) internalDecode(
       ReadOnlySpan<char> input,
       Span<byte> output,
       out int bytesWritten)
    {
        char? allZeroChar = Alphabet.AllZeroShortcut;
        char? allSpaceChar = Alphabet.AllSpaceShortcut;

        var table = Alphabet.ReverseLookupTable;

        int blockIndex = 0;
        long value = 0;
        int i = 0;
        bytesWritten = 0;
        while (i < input.Length)
        {
            char c = input[i++];
            if (isWhiteSpace(c))
            {
                continue;
            }

            // handle shortcut characters
            if (c == allZeroChar && allZeroChar is not null)
            {
                var result = writeShortcut(output[bytesWritten..], ref blockIndex, 0, out int bytesWrittenNow);
                if (result != DecodeResult.Success)
                {
                    return (result, c);
                }

                bytesWritten += bytesWrittenNow;
                continue;
            }
            else if (c == allSpaceChar && allSpaceChar is not null)
            {
                var result = writeShortcut(output[bytesWritten..], ref blockIndex, fourSpaceChars, out int bytesWrittenNow);
                if (result != DecodeResult.Success)
                {
                    return (result, c);
                }

                bytesWritten += bytesWrittenNow;
                continue;
            }

            // handle regular blocks
            int x = table[c] - 1; // map character to byte value
            if (x < 0)
            {
                return (DecodeResult.InvalidCharacter, c);
            }

            value = (value * baseLength) + x;
            blockIndex += 1;
            if (blockIndex == stringBlockSize)
            {
                var result = writeDecodedValue(output[bytesWritten..], value, byteBlockSize, out int bytesWrittenNow);
                if (result != DecodeResult.Success)
                {
                    return (result, null);
                }

                bytesWritten += bytesWrittenNow;
                blockIndex = 0;
                value = 0;
            }
        }

        if (blockIndex > 0)
        {
            // handle padding by treating the rest of the characters
            // as "u"s. so both big endianness and bit weirdness work out okay.
            for (int n = 0; n < stringBlockSize - blockIndex; n++)
            {
                value = (value * baseLength) + (baseLength - 1);
            }

            var result = writeDecodedValue(output[bytesWritten..], value, blockIndex - 1, out int bytesWrittenNow);
            if (result != DecodeResult.Success)
            {
                return (result, null);
            }

            bytesWritten += bytesWrittenNow;
        }

        return (DecodeResult.Success, null);
    }
}
