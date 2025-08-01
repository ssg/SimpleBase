﻿// <copyright file="Base32.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleBase;

/// <summary>
/// Base32 encoding/decoding functions.
/// </summary>
public sealed class Base32 : IBaseCoder, IBaseStreamCoder, INonAllocatingBaseCoder,
    INumericBaseCoder
{
    const int bitsPerByte = 8;
    const int bitsPerChar = 5;

    // this is an instance variable to allow unit tests to test this behavior
    internal readonly bool IsBigEndian;

    static readonly Lazy<Base32> crockford = new(() => new Base32(Base32Alphabet.Crockford));
    static readonly Lazy<Base32> rfc4648 = new(() => new Base32(Base32Alphabet.Rfc4648));
    static readonly Lazy<Base32> extendedHex = new(() => new Base32(Base32Alphabet.ExtendedHex));
    static readonly Lazy<Base32> extendedHexLower = new(() => new Base32(Base32Alphabet.ExtendedHexLower));
    static readonly Lazy<Base32> zBase32 = new(() => new Base32(Base32Alphabet.ZBase32));
    static readonly Lazy<Base32> geohash = new(() => new Base32(Base32Alphabet.Geohash));
    static readonly Lazy<Base32> bech32 = new(() => new Base32(Base32Alphabet.Bech32));
    static readonly Lazy<Base32> filecoin = new(() => new Base32(Base32Alphabet.FileCoin));

    /// <summary>
    /// Initializes a new instance of the <see cref="Base32"/> class with a
    /// custom alphabet.
    /// </summary>
    /// <param name="alphabet">Alphabet to use.</param>
    public Base32(Base32Alphabet alphabet)
        : this(alphabet, !BitConverter.IsLittleEndian)
    {
    }

    internal Base32(Base32Alphabet alphabet, bool isBigEndian)
    {
        if (alphabet.PaddingPosition != PaddingPosition.End)
        {
            throw new ArgumentException(
                "Only encoding alphabets with paddings at the end are supported by this implementation",
                nameof(alphabet));
        }

        Alphabet = alphabet;
        IsBigEndian = isBigEndian;
    }

    enum DecodeResult
    {
        Success = 0,
        InvalidInput,
        OutputOverflow,
    }

    /// <summary>
    /// Gets Douglas Crockford's Base32 flavor with substitution characters.
    /// </summary>
    public static Base32 Crockford => crockford.Value;

    /// <summary>
    /// Gets RFC 4648 variant of Base32 coder.
    /// </summary>
    public static Base32 Rfc4648 => rfc4648.Value;

    /// <summary>
    /// Gets Extended Hex variant of Base32 coder.
    /// </summary>
    /// <remarks>Also from RFC 4648.</remarks>
    public static Base32 ExtendedHex => extendedHex.Value;

    /// <summary>
    /// Gets Extended Hex variant of Base32 coder.
    /// </summary>
    /// <remarks>Also from RFC 4648.</remarks>
    public static Base32 ExtendedHexLower => extendedHexLower.Value;

    /// <summary>
    /// Gets z-base-32 variant of Base32 coder.
    /// </summary>
    /// <remarks>This variant is used in Mnet, ZRTP and Tahoe-LAFS.</remarks>
    public static Base32 ZBase32 => zBase32.Value;

    /// <summary>
    /// Gets Geohash variant of Base32 coder.
    /// </summary>
    public static Base32 Geohash => geohash.Value;

    /// <summary>
    /// Gets Bech32 variant of Base32 coder.
    /// </summary>
    public static Base32 Bech32 => bech32.Value;

    /// <summary>
    /// Gets FileCoin variant of Base32 coder Also known as RFC 4648 lowercase.
    /// </summary>
    public static Base32 FileCoin => filecoin.Value;

    /// <summary>
    /// Gets the encoding alphabet.
    /// </summary>
    public Base32Alphabet Alphabet { get; }

    /// <inheritdoc/>
    public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        return getAllocationByteCountForDecoding(text.Length - getPaddingCharCount(text));
    }

    /// <inheritdoc/>
    public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> buffer)
    {
        return (((buffer.Length - 1) / bitsPerChar) + 1) * bitsPerByte;
    }

    /// <inheritdoc/>
    public string Encode(long number)
    {
        return number >= 0
            ? Encode((ulong)number)
            : throw new ArgumentOutOfRangeException(nameof(number), "Number is negative");
    }

    static readonly byte[] zeroBuffer = [0];

    /// <inheritdoc/>
    public string Encode(ulong number)
    {
        const int numBytes = sizeof(ulong);
        if (number == 0)
        {
            return Encode(zeroBuffer.AsSpan());
        }

        var buffer = BitConverter.GetBytes(number);

        // skip zeroes for encoding
        int i;
        if (IsBigEndian)
        {
            // skip leading zeroes
            for (i = 0; buffer[i] == 0 && i < numBytes; i++)
            {
            }
            var span = buffer.AsSpan()[i..];
            span.Reverse(); // so the encoding is consistent between systems with different endianness
            return Encode(span);
        }

        // skip trailing zeroes
        for (i = numBytes - 1; buffer[i] == 0 && i > 0; i--)
        {
        }
        return Encode(buffer.AsSpan()[..(i + 1)]);
    }

    /// <inheritdoc/>
    public ulong DecodeUInt64(string text)
    {
        var buffer = Decode(text);
        if (buffer.Length > sizeof(ulong))
        {
            throw new InvalidOperationException("Decoded text is too long to fit in a buffer");
        }

        var span = buffer.AsSpan();
        Span<byte> newSpan = stackalloc byte[sizeof(ulong)];
        span.CopyTo(newSpan);
        if (IsBigEndian)
        {
            newSpan.Reverse();
        }

        return BitConverter.ToUInt64(newSpan);
    }

    /// <inheritdoc/>
    public bool TryDecodeUInt64(string text, out ulong number)
    {
        Span<byte> output = stackalloc byte[sizeof(ulong)];
        if (!TryDecode(text, output, out _))
        {
            number = 0;
            return false;
        }

        if (IsBigEndian)
        {
            output.Reverse();
        }

        // BitConverter.ToUInt64() specifically requires
        // the span to be 8 bytes long, so we can't just
        // use bytesWritten in the decoding here, we
        // need to pass the whole thing.
        number = BitConverter.ToUInt64(output);
        return true;
    }

    /// <inheritdoc/>
    public long DecodeInt64(string text)
    {
        var result = DecodeUInt64(text);
        if (result > long.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(text), "Decoded buffer is out of Int64 range");
        }
        return (long)result;
    }

    /// <summary>
    /// Encode a memory span into a Base32 string without padding.
    /// </summary>
    /// <param name="bytes">Buffer to be encoded.</param>
    /// <returns>Encoded string.</returns>
    public string Encode(ReadOnlySpan<byte> bytes)
    {
        return Encode(bytes, padding: false);
    }

    /// <summary>
    /// Encode a memory span into a Base32 string.
    /// </summary>
    /// <param name="bytes">Buffer to be encoded.</param>
    /// <param name="padding">
    ///     <see langword="true"/> if padding characters should be appended to the return value,
    ///     <see langword="false"/> otherwise.
    /// </param>
    /// <returns>Encoded string.</returns>
    public string Encode(ReadOnlySpan<byte> bytes, bool padding)
    {
        int bytesLen = bytes.Length;
        if (bytesLen == 0)
        {
            return string.Empty;
        }

        // we are ok with slightly larger buffer since the output string will always
        // have the exact length of the output produced.
        int outputLen = GetSafeCharCountForEncoding(bytes);
        Span<char> output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc char[outputLen] : new char[outputLen];
        if (!internalEncode(
            bytes,
            output,
            padding,
            out int numCharsWritten))
        {
            throw new InvalidOperationException("Internal error: couldn't calculate proper output buffer size for input");
        }

        // we can't use `String.Create` here to reduce allocations because
        // Spans aren't supported in lambda expressions.
        return new string(output[..numCharsWritten]);
    }

    /// <summary>
    /// Decode a Base32 encoded string into bytes.
    /// </summary>
    /// <param name="text">Encoded Base32 string.</param>
    /// <returns>Decoded bytes.</returns>
    public byte[] Decode(ReadOnlySpan<char> text)
    {
        int paddingLen = getPaddingCharCount(text);
        int textLen = text.Length - paddingLen;
        int outputLen = getAllocationByteCountForDecoding(textLen);
        if (outputLen == 0)
        {
            return [];
        }

        var outputBuffer = new byte[outputLen];
        var result = internalDecode(text[..textLen], outputBuffer, out int bytesWritten);
        return result switch
        {
            DecodeResult.InvalidInput => throw new ArgumentException("Invalid character in input", nameof(text)),
            DecodeResult.OutputOverflow => throw new InvalidOperationException("Output buffer is too small"),
            DecodeResult.Success when bytesWritten != outputLen => throw
                new InvalidOperationException("Actual written bytes are different"),
            DecodeResult.Success => outputBuffer,
            _ => throw new InvalidOperationException($"Unhandled decode result: {result}"),
        };
    }

    /// <summary>
    /// Encode a binary stream to a Base32 text stream without padding.
    /// </summary>
    /// <param name="input">Input bytes.</param>
    /// <param name="output">The writer the output is written to.</param>
    public void Encode(Stream input, TextWriter output)
    {
        Encode(input, output, padding: false);
    }

    /// <summary>
    /// Encode a binary stream to a Base32 text stream.
    /// </summary>
    /// <param name="input">Input bytes.</param>
    /// <param name="output">The writer the output is written to.</param>
    /// <param name="padding">
    ///     <see langword="true"/> if padding characters should be appended to <paramref name="output"/>,
    ///     <see langword="false"/> otherwise.
    /// </param>
    public void Encode(Stream input, TextWriter output, bool padding)
    {
        StreamHelper.Encode(input, output, (buffer, lastBlock) =>
        {
            bool usePadding = lastBlock && padding;
            return Encode(buffer.Span, usePadding);
        });
    }

    /// <summary>
    /// Encode a binary stream to a Base32 text stream without padding.
    /// </summary>
    /// <param name="input">Input bytes.</param>
    /// <param name="output">The writer the output is written to.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task EncodeAsync(Stream input, TextWriter output)
    {
        return EncodeAsync(input, output, padding: false);
    }

    /// <summary>
    /// Encode a binary stream to a Base32 text stream.
    /// </summary>
    /// <param name="input">Input bytes.</param>
    /// <param name="output">The writer the output is written to.</param>
    /// <param name="padding">
    ///     <see langword="true"/> if padding characters should be appended to <paramref name="output"/>,
    ///     <see langword="false"/> otherwise.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task EncodeAsync(Stream input, TextWriter output, bool padding)
    {
        await StreamHelper.EncodeAsync(input, output, (buffer, lastBlock) =>
        {
            bool usePadding = lastBlock && padding;
            return Encode(buffer.Span, usePadding);
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Decode a text stream into a binary stream.
    /// </summary>
    /// <param name="input">TextReader open on the stream.</param>
    /// <param name="output">Binary output stream.</param>
    public void Decode(TextReader input, Stream output)
    {
        StreamHelper.Decode(input, output, buffer => Decode(buffer.Span));
    }

    /// <summary>
    /// Decode a text stream into a binary stream.
    /// </summary>
    /// <param name="input">TextReader open on the stream.</param>
    /// <param name="output">Binary output stream.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task DecodeAsync(TextReader input, Stream output)
    {
        await StreamHelper.DecodeAsync(input, output, buffer => Decode(buffer.Span))
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public bool TryEncode(ReadOnlySpan<byte> bytes, Span<char> output, out int numCharsWritten)
    {
        return TryEncode(bytes, output, padding: false, out numCharsWritten);
    }

    /// <summary>
    /// Encode to the given preallocated buffer.
    /// </summary>
    /// <param name="bytes">Input bytes.</param>
    /// <param name="output">Output buffer.</param>
    /// <param name="padding">
    ///     <see langword="true"/> if padding characters should be appended to <paramref name="output"/>,
    ///     <see langword="false"/> otherwise.
    /// </param>
    /// <param name="numCharsWritten">Number of characters written to <paramref name="output"/>.</param>
    /// <returns><see langword="true"/> if encoding is successful, <see langword="false"/> if the output is invalid.</returns>
    public bool TryEncode(
        ReadOnlySpan<byte> bytes,
        Span<char> output,
        bool padding,
        out int numCharsWritten)
    {
        int bytesLen = bytes.Length;
        if (bytesLen == 0)
        {
            numCharsWritten = 0;
            return true;
        }

        return internalEncode(bytes, output, padding, out numCharsWritten);
    }

    /// <inheritdoc/>
    public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        int inputLen = input.Length - getPaddingCharCount(input);
        if (inputLen == 0)
        {
            bytesWritten = 0;
            return true;
        }

        int outputLen = output.Length;
        if (outputLen == 0)
        {
            bytesWritten = 0;
            return false;
        }

        return internalDecode(input[..inputLen], output, out bytesWritten) == DecodeResult.Success;
    }

    /// <summary>
    /// Perform Base32 with Digest and checksum encoding (Stacks Crockford Base32).
    /// </summary>
    /// <param name="input">Input buffer.</param>
    /// <param name="version">Version byte.</param>
    /// <returns>Encoded address with checksum.</returns>
    public string EncodeCheck(ReadOnlySpan<byte> input, byte version)
    {
        int bufferSize = 1 + input.Length + Sha256.DigestBytes;
        byte[] output = new byte[bufferSize];
        output[0] = version;
        input.CopyTo(output.AsSpan(1));
        Sha256.ComputeDigestTwice(output.AsSpan(0, input.Length + 1), output.AsSpan(input.Length + 1));
        return Encode(output);
    }

    /// <summary>
    /// Try decoding a Base32 encoded address with checksum.
    /// </summary>
    /// <param name="input">Encoded string.</param>
    /// <param name="decodedAddress">Buffer for the decoded address.</param>
    /// <param name="version">Version number extracted from <paramref name="input"/>.</param>
    /// <param name="addressLength">Written number of bytes to <paramref name="decodedAddress"/>.</param>
    /// <returns>
    ///     <see langword="true"/> if address is decoded successfully and checksum matches,
    ///     <see langword="false"/> otherwise.
    /// </returns>
    public bool TryDecodeCheck(ReadOnlySpan<char> input, Span<byte> decodedAddress, out byte version, out int addressLength)
    {
        int bufferSize = GetSafeByteCountForDecoding(input);
        var buffer = bufferSize < Bits.SafeStackMaxAllocSize ? stackalloc byte[bufferSize] : new byte[bufferSize];
        version = 0;
        addressLength = 0;
        if (!TryDecode(input, buffer, out int outputLength)
            || outputLength < Sha256.DigestBytes + 2
            || decodedAddress.Length < outputLength - Sha256.DigestBytes - 1)
        {
            return false;
        }

        Span<byte> digest = stackalloc byte[Sha256.DigestBytes];
        Sha256.ComputeDigestTwice(buffer[..^Sha256.DigestBytes], digest);
        if (!digest.SequenceEqual(buffer[^Sha256.DigestBytes..]))
        {
            return false;
        }

        buffer[1..^Sha256.DigestBytes].CopyTo(decodedAddress);
        addressLength = outputLength - Sha256.DigestBytes - 1;
        version = buffer[0];
        return true;
    }

    static int getAllocationByteCountForDecoding(int textLenWithoutPadding)
    {
        return textLenWithoutPadding * bitsPerChar / bitsPerByte;
    }

    bool internalEncode(
       ReadOnlySpan<byte> input,
       Span<char> output,
       bool padding,
       out int numCharsWritten)
    {
        string table = Alphabet.Value;

        int bitsLeft = bitsPerByte;
        int outputPad;
        int o = 0;
        int value = input[0];
        for (int i = 0; i < input.Length;)
        {
            if (bitsLeft > bitsPerChar)
            {
                bitsLeft -= bitsPerChar;
                outputPad = value >> bitsLeft;
                if (o >= output.Length)
                {
                    goto Overflow;
                }

                output[o++] = table[outputPad];
                value &= (1 << bitsLeft) - 1;
            }

            int nextBits = bitsPerChar - bitsLeft;
            bitsLeft = bitsPerByte - nextBits;
            outputPad = value << nextBits;
            if (++i < input.Length)
            {
                value = input[i];
                outputPad |= value >> bitsLeft;
                value &= (1 << bitsLeft) - 1;
            }

            if (o >= output.Length)
            {
                goto Overflow;
            }

            output[o++] = table[outputPad];
        }

        if (padding)
        {
            char paddingChar = Alphabet.PaddingChar;
            while (o < output.Length)
            {
                output[o++] = paddingChar;
            }
        }

        numCharsWritten = o;
        return true;
    Overflow:
        numCharsWritten = o;
        return false;
    }

    int getPaddingCharCount(ReadOnlySpan<char> text)
    {
        char paddingChar = Alphabet.PaddingChar;
        int result = 0;
        int textLen = text.Length;

        if (Alphabet.PaddingPosition == PaddingPosition.Start)
        {
            foreach (char c in text)
            {
                if (c != paddingChar)
                {
                    return result;
                }

                result++;
            }

            return result;
        }

        while (textLen > 0 && text[--textLen] == paddingChar)
        {
            result++;
        }

        return result;
    }

    DecodeResult internalDecode(
        ReadOnlySpan<char> input,
        Span<byte> output,
        out int bytesWritten)
    {
        var table = Alphabet.ReverseLookupTable;
        int outputPad = 0;
        int bitsLeft = bitsPerByte;

        bytesWritten = 0;
        int o = 0;
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            int b = table[c] - 1;
            if (b < 0)
            {
                bytesWritten = o;
                return DecodeResult.InvalidInput;
            }

            if (bitsLeft > bitsPerChar)
            {
                bitsLeft -= bitsPerChar;
                outputPad |= b << bitsLeft;
                continue;
            }

            int shiftBits = bitsPerChar - bitsLeft;
            outputPad |= b >> shiftBits;
            if (o >= output.Length)
            {
                return DecodeResult.OutputOverflow;
            }

            output[o++] = (byte)outputPad;
            b &= (1 << shiftBits) - 1;
            bitsLeft = bitsPerByte - shiftBits;
            outputPad = b << bitsLeft;
        }

        bytesWritten = o;
        return DecodeResult.Success;
    }
}
