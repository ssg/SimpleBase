// <copyright file="MoneroBase58.cs" company="Sedat Kapanoglu">
// Copyright (c) 2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Monero variant of Base58 Encoding/Decoding algorithm. Differently from other Base58 implementations,
/// Monero encodes using 8-byte blocks and converts them into 11-byte blocks instead of going byte-by-byte.
/// This makes Monero a bit less algorihmically complex. If the block size is smaller than 11 bytes, the
/// rest is padded with encoded zeroes ("1" on Monero Base58 alphabet).
/// </summary>
/// <param name="alphabet">An optional custom alphabet to use. By default, monero uses Bitcoin alphabet.</param>
public sealed class MoneroBase58(Base58Alphabet alphabet) : IBaseCoder, INonAllocatingBaseCoder
{
    static readonly int[] encodedBlockSizes = [0, 2, 3, 5, 6, 7, 9, 10, 11];
    const int blockSize = 8;
    static readonly int encodedBlockSize = encodedBlockSizes[blockSize];

    /// <summary>
    /// Initializes a new instance of the <see cref="MoneroBase58"/> class
    /// </summary>
    public MoneroBase58()
        : this(Base58Alphabet.Bitcoin)
    {
    }

    /// <summary>
    /// Gets the encoding alphabet.
    /// </summary>
    public Base58Alphabet Alphabet { get; } = alphabet;

    /// <summary>
    /// Gets the character for zero.
    /// </summary>
    public char ZeroChar { get; } = alphabet.Value[0];

    /// <inheritdoc/>
    public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        return getSafeByteCountForDecoding(text.Length);
    }

    /// <inheritdoc/>
    public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> bytes)
    {
        return getSafeCharCountForEncoding(bytes.Length);
    }

    static int getSafeCharCountForEncoding(int length)
    {
        return ((length / blockSize) + 1) * encodedBlockSize;
    }

    static int getSafeByteCountForDecoding(int length)
    {
        return (length * blockSize / encodedBlockSize) + 1;
    }

    /// <summary>
    /// Encode to Base58 representation.
    /// </summary>
    /// <param name="bytes">Bytes to encode.</param>
    /// <returns>Encoded string.</returns>
    public string Encode(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
        {
            return string.Empty;
        }

        int outputLen = getSafeCharCountForEncoding(bytes.Length);
        Span<char> output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc char[outputLen] : new char[outputLen];

        return internalEncode(bytes, output, out int numCharsWritten)
            ? new string(output[..numCharsWritten])
            : throw new InvalidOperationException("Output buffer with insufficient size generated");
    }

    /// <summary>
    /// Decode a Base58 representation.
    /// </summary>
    /// <param name="text">Base58 encoded text.</param>
    /// <returns>Decoded bytes.</returns>
    public byte[] Decode(ReadOnlySpan<char> text)
    {
        if (text.Length == 0)
        {
            return [];
        }

        int outputLen = getSafeByteCountForDecoding(text.Length);
        Span<byte> output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc byte[outputLen] : new byte[outputLen];

        return internalDecode(
            text,
            output,
            out int bytesWritten) switch
        {
            (DecodeResult.Success, _) => output[..bytesWritten].ToArray(),
            (DecodeResult.InvalidCharacter, char c) => throw CodingAlphabet.InvalidCharacter(c),
            (DecodeResult.InsufficientOutputBuffer, _) => throw new InvalidOperationException("Output buffer with insufficient size generated - likely a bug"),
            _ => throw new InvalidOperationException("This should never be hit - likely a bug"),
        };
    }

    /// <inheritdoc/>
    public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int numCharsWritten)
    {
        return internalEncode(input, output, out numCharsWritten);
    }

    /// <inheritdoc/>
    public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        if (input.Length == 0)
        {
            bytesWritten = 0;
            return true;
        }

        return internalDecode(
            input,
            output,
            out bytesWritten) is (DecodeResult.Success, _);
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

        char zeroChar = ZeroChar;
        ReadOnlySpan<char> alphabet = Alphabet.Value;
        int offset = 0;
        int outputOffset = 0;
        (int numBlocks, int remainingLength) = Math.DivRem(input.Length, blockSize);
        for (int i = 0; i < numBlocks; i++)
        {
            var inputBlock = input[offset..(offset + blockSize)];
            encodeBlock(inputBlock, output[outputOffset..(outputOffset + encodedBlockSize)], alphabet, zeroChar);
            offset += blockSize;
            outputOffset += encodedBlockSize;
        }

        if (remainingLength > 0)
        {
            var remainingInputBlock = input[offset..];
            Span<char> tempPad = stackalloc char[encodedBlockSize];
            encodeBlock(remainingInputBlock, tempPad, alphabet, zeroChar);
            int lastBlockSize = encodedBlockSizes[remainingInputBlock.Length];
            tempPad[..lastBlockSize].CopyTo(output[outputOffset..]);
            outputOffset += lastBlockSize;
        }

        numCharsWritten = outputOffset;
        return true;
    }

    static void encodeBlock(ReadOnlySpan<byte> input, Span<char> output, ReadOnlySpan<char> alphabet, char zeroChar)
    {
        if (input.Length > blockSize)
        {
            throw new ArgumentException("Invalid block size", nameof(input));
        }

        if (output.Length != encodedBlockSize)
        {
            throw new ArgumentException("Invalid block size", nameof(output));
        }

        ulong pad = Bits.BigEndianBytesToUInt64(input);
        int lastPos = encodedBlockSizes[input.Length];
        int i = lastPos;
        while (i > 0)
        {
            (pad, ulong remainder) = Math.DivRem(pad, 58);
            output[--i] = alphabet[(int)remainder];
        }

        // fill the rest with encoded zeroes
        output[lastPos..].Fill(zeroChar);
    }

    enum DecodeResult
    {
        Success,
        InsufficientOutputBuffer,
        InvalidCharacter,
    }

    static (DecodeResult, char?) decodeBlock(ReadOnlySpan<char> input, Span<byte> output, ReadOnlySpan<byte> reverseLookupTable)
    {
        ulong pad = 0;
        for (int i = 0; i < input.Length; i++)
        {
            pad *= 58;
            char c = input[i];
            int value = reverseLookupTable[c] - 1;
            if (value < 0)
            {
                return (DecodeResult.InvalidCharacter, c);
            }
            pad += (uint)value;
        }
        Bits.UInt64ToBigEndianBytes(pad, output);
        return (DecodeResult.Success, null);
    }

    (DecodeResult, char?) internalDecode(
        ReadOnlySpan<char> input,
        Span<byte> output,
        out int bytesWritten)
    {
        var table = Alphabet.ReverseLookupTable;

        // read 11 char blocks from the input and decode them into 8-byte blocks
        int numBlocks = input.Length / encodedBlockSize;
        int wholeEndOffset = numBlocks * encodedBlockSize;
        bytesWritten = 0;
        int inputOffset = 0;
        while (inputOffset < wholeEndOffset)
        {
            var inputPad = input[inputOffset..(inputOffset + encodedBlockSize)];
            var outputPad = output[bytesWritten..(bytesWritten + blockSize)];
            var result = decodeBlock(inputPad, outputPad, table);
            if (result is not (DecodeResult.Success, _))
            {
                return result;
            }
            inputOffset += encodedBlockSize;
            bytesWritten += blockSize;
        }

        // decode the remainder block in a temporary buffer, and copy it back to the output
        int remainingLength = input.Length - wholeEndOffset;
        if (remainingLength > 0)
        {
            Span<byte> temp = stackalloc byte[blockSize];
            var remainingBuffer = input[wholeEndOffset..];
            var result = decodeBlock(remainingBuffer, temp, table);
            if (result is not (DecodeResult.Success, _))
            {
                return result;
            }
            int tempSize = encodedBlockSizes.AsSpan().IndexOf(remainingBuffer.Length);
            if (tempSize < 0)
            {
                // invalid length for encoded remaining buffer
                return (DecodeResult.InsufficientOutputBuffer, null);
            }
            temp[(blockSize - tempSize)..].CopyTo(output[bytesWritten..]);
            bytesWritten += tempSize;
        }
        return (DecodeResult.Success, null);
    }
}
