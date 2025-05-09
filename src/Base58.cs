// <copyright file="Base58.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace SimpleBase;

/// <summary>
/// Base58 Encoding/Decoding implementation.
/// </summary>
/// <remarks>
/// Base58 doesn't implement a Stream-based interface because it's not feasible to use
/// on large buffers.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="Base58"/> class
/// using a custom alphabet.
/// </remarks>
/// <param name="alphabet">Alphabet to use.</param>
public sealed class Base58(Base58Alphabet alphabet) : IBaseCoder, INonAllocatingBaseCoder
{
    const int reductionFactor = 733; // https://github.com/bitcoin/bitcoin/blob/master/src/base58.cpp#L48
    const int divisor = 58;
    const int maxCheckPayloadLength = 256;
    const int sha256Bytes = 32;
    const int sha256DigestBytes = 4;
    static readonly Lazy<Base58> bitcoin = new(() => new Base58(Base58Alphabet.Bitcoin));
    static readonly Lazy<Base58> ripple = new(() => new Base58(Base58Alphabet.Ripple));
    static readonly Lazy<Base58> flickr = new(() => new Base58(Base58Alphabet.Flickr));
    static readonly Lazy<MoneroBase58> monero = new(() => new MoneroBase58());

    /// <summary>
    /// Gets Bitcoin flavor.
    /// </summary>
    public static Base58 Bitcoin => bitcoin.Value;

    /// <summary>
    /// Gets Ripple flavor.
    /// </summary>
    public static Base58 Ripple => ripple.Value;

    /// <summary>
    /// Gets Flickr flavor.
    /// </summary>
    public static Base58 Flickr => flickr.Value;

    /// <summary>
    /// Gets Monero flavor.
    /// </summary>
    /// <remarks>This uses a different algorithm for Base58 encoding. See <see cref="MoneroBase58"/> for details.</remarks>
    public static MoneroBase58 Monero => monero.Value;

    /// <summary>
    /// Gets the encoding alphabet.
    /// </summary>
    public Base58Alphabet Alphabet { get; } = alphabet;

    /// <summary>
    /// Gets the character for zero.
    /// </summary>
    public char ZeroChar { get; } = alphabet.Value[0];

    /// <summary>
    /// Retrieve safe byte count while avoiding multiple counting operations.
    /// </summary>
    /// <param name="textLen">Length of text.</param>
    /// <param name="numZeroes">Number of prefix zeroes.</param>
    /// <returns>Length of safe allocation.</returns>
    public static int GetSafeByteCountForDecoding(int textLen, int numZeroes)
    {
        Debug.Assert(textLen >= numZeroes, "Number of zeroes cannot be longer than text length");
        return numZeroes + ((textLen - numZeroes + 1) * reductionFactor / 1000) + 1;
    }

    /// <inheritdoc/>
    public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        int textLen = text.Length;
        return GetSafeByteCountForDecoding(textLen, getPrefixCount(text, ZeroChar));
    }

    /// <inheritdoc/>
    public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> bytes)
    {
        int bytesLen = bytes.Length;
        int numZeroes = getZeroCount(bytes);

        return getSafeCharCountForEncoding(bytesLen, numZeroes);
    }

    /// <summary>
    /// Generate a Base58Check string out of a version and payload.
    /// </summary>
    /// <param name="payload">Address data.</param>
    /// <param name="version">Address version.</param>
    /// <returns>Base58Check address.</returns>
    public string EncodeCheck(ReadOnlySpan<byte> payload, byte version)
    {
        if (payload.Length > maxCheckPayloadLength)
        {
            throw new ArgumentException($"Payload length {payload.Length} is greater than {maxCheckPayloadLength}", nameof(payload));
        }

        int versionPlusPayloadLen = payload.Length + 1;
        int outputLen = versionPlusPayloadLen + sha256DigestBytes;
        Span<byte> output = (outputLen < Bits.SafeStackMaxAllocSize) ? stackalloc byte[outputLen] : new byte[outputLen];
        output[0] = version;
        payload.CopyTo(output[1..]);
        Span<byte> sha256 = stackalloc byte[sha256Bytes];
        computeDoubleSha256(output[..versionPlusPayloadLen], sha256);
        sha256[..sha256DigestBytes].CopyTo(output[versionPlusPayloadLen..]);
        return Encode(output);
    }

    /// <summary>
    /// Try to decode and verify a Base58Check address.
    /// </summary>
    /// <param name="address">Address string.</param>
    /// <param name="payload">Output address buffer.</param>
    /// <param name="version">Address version.</param>
    /// <param name="bytesWritten">Number of bytes written in the output payload.</param>
    /// <returns>True if address was decoded successfully and passed validation. False, otherwise.</returns>
    public bool TryDecodeCheck(
        ReadOnlySpan<char> address,
        Span<byte> payload,
        out byte version,
        out int bytesWritten)
    {
        Span<byte> buffer = stackalloc byte[maxCheckPayloadLength + sha256DigestBytes + 1];
        if (!TryDecode(address, buffer, out bytesWritten) || bytesWritten < 5)
        {
            version = 0;
            return false;
        }

        buffer = buffer[..bytesWritten];
        version = buffer[0];
        Span<byte> sha256 = stackalloc byte[sha256Bytes];
        computeDoubleSha256(buffer[..^sha256DigestBytes], sha256);
        if (!sha256[..sha256DigestBytes].SequenceEqual(buffer[^sha256DigestBytes..]))
        {
            version = 0;
            return false;
        }

        var finalBuffer = buffer[1..^sha256DigestBytes];
        version = buffer[0];
        finalBuffer.CopyTo(payload);
        bytesWritten = finalBuffer.Length;
        return true;
    }

    /// <summary>
    /// Generate an Avalanche CB58 string out of a version and payload.
    /// </summary>
    /// <param name="payload">Address data.</param>
    /// <returns>CB58 address.</returns>
    public string EncodeCb58(ReadOnlySpan<byte> payload)
    {
        if (payload.Length > maxCheckPayloadLength)
        {
            throw new ArgumentException($"Payload length {payload.Length} is greater than {maxCheckPayloadLength}", nameof(payload));
        }

        int outputLen = payload.Length + sha256DigestBytes;
        Span<byte> output = (outputLen < Bits.SafeStackMaxAllocSize) ? stackalloc byte[outputLen] : new byte[outputLen];
        payload.CopyTo(output);
        Span<byte> sha256 = stackalloc byte[sha256Bytes];
        computeSha256(output[..payload.Length], sha256);

        sha256[^sha256DigestBytes..].CopyTo(output[payload.Length..]);
        return Encode(output);
    }

    /// <summary>
    /// Try to decode and verify an Avalanche CB58 address.
    /// </summary>
    /// <param name="address">Address string.</param>
    /// <param name="payload">Output address buffer.</param>
    /// <param name="bytesWritten">Number of bytes written in the output payload.</param>
    /// <returns>True if address was decoded successfully and passed validation. False, otherwise.</returns>
    public bool TryDecodeCb58(
        ReadOnlySpan<char> address,
        Span<byte> payload,
        out int bytesWritten)
    {
        Span<byte> buffer = stackalloc byte[maxCheckPayloadLength + sha256DigestBytes];
        if (!TryDecode(address, buffer, out bytesWritten) || bytesWritten < 4)
        {
            return false;
        }

        buffer = buffer[..bytesWritten];
        Span<byte> sha256 = stackalloc byte[sha256Bytes];
        computeSha256(buffer[..^sha256DigestBytes], sha256);

        if (!sha256[^sha256DigestBytes..].SequenceEqual(buffer[^sha256DigestBytes..]))
        {
            return false;
        }

        var finalBuffer = buffer[..^sha256DigestBytes];
        finalBuffer.CopyTo(payload);
        bytesWritten = finalBuffer.Length;
        return true;
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

        int numZeroes = getZeroCount(bytes);
        int outputLen = getSafeCharCountForEncoding(bytes.Length, numZeroes);

        // we can't use `String.Create` here to reduce allocations because
        // Spans aren't supported in lambda expressions.
        Span<char> output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc char[outputLen] : new char[outputLen];

        return internalEncode(bytes, output, numZeroes, out int numCharsWritten)
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

        char zeroChar = ZeroChar;
        int numZeroes = getPrefixCount(text, zeroChar);
        int outputLen = GetSafeByteCountForDecoding(text.Length, numZeroes);
        Span<byte> output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc byte[outputLen] : new byte[outputLen];
        var result = internalDecode(text, output, numZeroes, out Range rangeWritten);

        return result switch
        {
            (DecodeResult.InvalidCharacter, char c) => throw CodingAlphabet.InvalidCharacter(c),
            (DecodeResult.InsufficientOutputBuffer, _) => throw new InvalidOperationException("Output buffer was too small while decoding Base58"),
            (DecodeResult.Success, _) => output[rangeWritten].ToArray(),
            _ => throw new InvalidOperationException("This should be never hit - probably a bug"),
        };
    }

    /// <inheritdoc/>
    public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int numCharsWritten)
    {
        int numZeroes = getZeroCount(input);
        return internalEncode(input, output, numZeroes, out numCharsWritten);
    }

    /// <inheritdoc/>
    public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        if (input.Length == 0)
        {
            bytesWritten = 0;
            return true;
        }

        int zeroCount = getPrefixCount(input, ZeroChar);
        var result = internalDecode(
            input,
            output,
            zeroCount,
            out Range rangeWritten);

        output[rangeWritten].CopyTo(output);
        bytesWritten = rangeWritten.End.Value - rangeWritten.Start.Value;
        return result is (DecodeResult.Success, _);
    }

    static void computeDoubleSha256(ReadOnlySpan<byte> buffer, Span<byte> output)
    {
        Span<byte> tempResult = stackalloc byte[sha256Bytes];
        computeSha256(buffer, tempResult);
        computeSha256(tempResult, output);
    }

    static void computeSha256(ReadOnlySpan<byte> buffer, Span<byte> output)
    {
        if (!SHA256.TryHashData(buffer, output, out int bytesWritten))
        {
            throw new InvalidOperationException("Couldn't compute SHA256");
        }

        if (bytesWritten != sha256Bytes)
        {
            throw new InvalidOperationException("Invalid SHA256 length");
        }
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

    static bool encodeAllZeroes(
        Span<char> output,
        int numZeroes,
        out int numCharsWritten,
        char zeroChar)
    {
        if (output.Length < numZeroes)
        {
            // insufficient output buffer length
            numCharsWritten = 0;
            return false;
        }

        output[..numZeroes].Fill(zeroChar);
        numCharsWritten = numZeroes;
        return true;
    }

    static int getZeroCount(ReadOnlySpan<byte> bytes)
    {
        int count = 0;
        for (; count < bytes.Length && bytes[count] == 0; count++)
        {
        }

        return count;
    }

    // we can't make this a generic method and reuse it with getZeroCount()
    // because IEquatable<T> is way slower than equality operator.
    static int getPrefixCount(ReadOnlySpan<char> input, char value)
    {
        int count = 0;
        for (; count < input.Length && input[count] == value; count++)
        {
        }

        return count;
    }

    static int getSafeCharCountForEncoding(int bytesLen, int numZeroes)
    {
        const int growthPercentage = 138;

        return numZeroes + ((bytesLen - numZeroes) * growthPercentage / 100) + 1;
    }

    bool internalEncode(
        ReadOnlySpan<byte> input,
        Span<char> output,
        int numZeroes,
        out int numCharsWritten)
    {
        if (input.Length == 0)
        {
            numCharsWritten = 0;
            return true;
        }

        ReadOnlySpan<char> alphabet = Alphabet.Value;
        if (numZeroes == input.Length)
        {
            return encodeAllZeroes(output, numZeroes, out numCharsWritten, alphabet[0]);
        }

        int numDigits = 0;
        int index = numZeroes;
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

        translatedCopy(output[^numDigits..], output[numZeroes..], alphabet);
        if (numZeroes > 0)
        {
            output[..numZeroes].Fill(alphabet[0]);
        }

        numCharsWritten = numZeroes + numDigits;
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
        int numZeroes,
        out Range rangeWritten)
    {
        if (numZeroes == input.Length)
        {
            var result = decodeZeroes(output, numZeroes, out int numBytesWritten);
            rangeWritten = ..numBytesWritten;
            return (result, null);
        }

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
                carry += 58 * output[o];
                output[o] = (byte)carry;
                if (min > o && carry != 0)
                {
                    min = o;
                }

                carry >>= 8;
            }
        }

        rangeWritten = (min - numZeroes)..output.Length;
        return (DecodeResult.Success, null);
    }
}
