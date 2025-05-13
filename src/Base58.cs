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
public sealed class Base58(Base58Alphabet alphabet) : DividingCoder<Base58Alphabet>(alphabet)
{
    // this is normally automatically calculated by DividingCoder, but we keep it here
    // to support the legacy function GetSafeByteCountForDecoding().
    const int reductionFactor = 733;

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
}
