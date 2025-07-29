// <copyright file="Base58.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Diagnostics;

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

        ReadOnlySpan<byte> prefix = [version];
        return EncodeCheck(payload, prefix);
    }

    /// <summary>
    /// Generate a Base58Check string out of a prefix buffer and payload.
    /// </summary>
    /// <param name="payload">Address data.</param>
    /// <param name="prefix">Prefix buffer.</param>
    /// <returns>Base58Check address.</returns>
    /// <exception cref="ArgumentException">If <paramref name="payload"/> is empty.</exception>
    public string EncodeCheck(ReadOnlySpan<byte> payload, ReadOnlySpan<byte> prefix)
    {
        int totalLength = prefix.Length + payload.Length;
        int outputLen = totalLength + Sha256.DigestBytes;
        Span<byte> output = (outputLen < Bits.SafeStackMaxAllocSize) ? stackalloc byte[outputLen] : new byte[outputLen];
        prefix.CopyTo(output);
        payload.CopyTo(output[prefix.Length..]);
        Span<byte> sha256 = stackalloc byte[Sha256.Bytes];
        Sha256.ComputeTwice(output[..totalLength], sha256);
        sha256[..Sha256.DigestBytes].CopyTo(output[totalLength..]);
        return Encode(output);
    }

    /// <summary>
    /// Generate a Base58Check string out of a prefix buffer and payload
    /// by skipping leading zeroes in <paramref name="payload"/>.
    /// Platforms like Tezos expects this behavior.
    /// </summary>
    /// <param name="payload">Address data.</param>
    /// <param name="prefix">Prefix buffer.</param>
    /// <returns>Base58Check address.</returns>
    /// <exception cref="ArgumentException">
    ///     If <paramref name="payload"/> is empty or only contains zeroes.
    /// </exception>
    public string EncodeCheckSkipZeroes(ReadOnlySpan<byte> payload, ReadOnlySpan<byte> prefix)
    {
        int firstNonZeroIndex = payload.IndexOfAnyExcept((byte)0);
        if (firstNonZeroIndex < 0)
        {
            throw new ArgumentException("Payload cannot be empty or all zeroes", nameof(payload));
        }

        if (firstNonZeroIndex > 0)
        {
            payload = payload[firstNonZeroIndex..];
        }

        return EncodeCheck(payload, prefix);
    }

    /// <summary>
    /// Try to decode and verify a Base58Check address.
    /// </summary>
    /// <param name="address">Address string.</param>
    /// <param name="payload">Output address buffer.</param>
    /// <param name="version">Address version.</param>
    /// <param name="bytesWritten">Number of bytes written to <paramref name="payload"/>.</param>
    /// <returns>
    ///     <see langword="true"/> if address was decoded successfully and passed validation.
    ///     <see langword="false"/> otherwise.
    /// </returns>
    public bool TryDecodeCheck(
        ReadOnlySpan<char> address,
        Span<byte> payload,
        out byte version,
        out int bytesWritten)
    {
        Span<byte> versionBuffer = stackalloc byte[1];
        bool result = TryDecodeCheck(address, payload, versionBuffer, out bytesWritten);
        version = versionBuffer[0];
        return result;
    }

    /// <summary>
    /// Try to decode and verify a Base58Check address.
    /// </summary>
    /// <param name="address">Address string.</param>
    /// <param name="payload">Output address buffer.</param>
    /// <param name="prefix">Prefix decoded, must have the exact size of the expected prefix.</param>
    /// <param name="payloadBytesWritten">Number of bytes written in the output payload.</param>
    /// <returns><see langword="true"/> if address was decoded successfully and passed validation, <see langword="false"/> otherwise.</returns>
    public bool TryDecodeCheck(
        ReadOnlySpan<char> address,
        Span<byte> payload,
        Span<byte> prefix,
        out int payloadBytesWritten)
    {
        payloadBytesWritten = 0;
        Span<byte> buffer = stackalloc byte[maxCheckPayloadLength + Sha256.DigestBytes + 1];
        if (!TryDecode(address, buffer, out int decodedBufferSize))
        {
            return false;
        }

        buffer = buffer[..decodedBufferSize];
        buffer[..prefix.Length].CopyTo(prefix);
        Span<byte> sha256 = stackalloc byte[Sha256.Bytes];
        Sha256.ComputeTwice(buffer[..^Sha256.DigestBytes], sha256);
        if (!sha256[..Sha256.DigestBytes].SequenceEqual(buffer[^Sha256.DigestBytes..]))
        {
            return false;
        }

        var finalBuffer = buffer[1..^Sha256.DigestBytes];
        finalBuffer.CopyTo(payload);
        payloadBytesWritten = finalBuffer.Length;
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

        int outputLen = payload.Length + Sha256.DigestBytes;
        Span<byte> output = (outputLen < Bits.SafeStackMaxAllocSize) ? stackalloc byte[outputLen] : new byte[outputLen];
        payload.CopyTo(output);
        Span<byte> sha256 = stackalloc byte[Sha256.Bytes];
        Sha256.Compute(output[..payload.Length], sha256);

        sha256[^Sha256.DigestBytes..].CopyTo(output[payload.Length..]);
        return Encode(output);
    }

    /// <summary>
    /// Try to decode and verify an Avalanche CB58 address.
    /// </summary>
    /// <param name="address">Address string.</param>
    /// <param name="payload">Output address buffer.</param>
    /// <param name="bytesWritten">Number of bytes written to <paramref name="payload"/>.</param>
    /// <returns><see langword="true"/> if address was decoded successfully and passed validation. <see langword="false"/> otherwise.</returns>
    public bool TryDecodeCb58(
        ReadOnlySpan<char> address,
        Span<byte> payload,
        out int bytesWritten)
    {
        Span<byte> buffer = stackalloc byte[maxCheckPayloadLength + Sha256.DigestBytes];
        if (!TryDecode(address, buffer, out bytesWritten) || bytesWritten < 4)
        {
            return false;
        }

        buffer = buffer[..bytesWritten];
        Span<byte> sha256 = stackalloc byte[Sha256.Bytes];
        Sha256.Compute(buffer[..^Sha256.DigestBytes], sha256);

        if (!sha256[^Sha256.DigestBytes..].SequenceEqual(buffer[^Sha256.DigestBytes..]))
        {
            return false;
        }

        var finalBuffer = buffer[..^Sha256.DigestBytes];
        finalBuffer.CopyTo(payload);
        bytesWritten = finalBuffer.Length;
        return true;
    }

}
