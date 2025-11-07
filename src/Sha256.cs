// <copyright file="Sha256.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Security.Cryptography;

namespace SimpleBase;

/// <summary>
/// Internal SHA256 helpers.
/// </summary>
class Sha256
{
    public const int Bytes = 32;
    public const int DigestBytes = 4;

    internal static void ComputeTwice(ReadOnlySpan<byte> buffer, Span<byte> output)
    {
        Span<byte> tempResult = stackalloc byte[Bytes];
        Compute(buffer, tempResult);
        Compute(tempResult, output);
    }

    /// <summary>
    /// Compute 4-byte digest using double SHA256.
    /// </summary>
    /// <param name="buffer">Input buffer.</param>
    /// <param name="output">Output digest. (Must be 4 bytes long)</param>
    internal static void ComputeDigestTwice(ReadOnlySpan<byte> buffer, Span<byte> output)
    {
        if (output.Length != DigestBytes)
        {
            throw new ArgumentException($"Output must be {DigestBytes} bytes long", nameof(output));
        }

        Span<byte> tempResult = stackalloc byte[Bytes];
        ComputeTwice(buffer, tempResult);
        tempResult[..DigestBytes].CopyTo(output);
    }

    /// <summary>
    /// Compute 4-byte digest using single SHA256.
    /// </summary>
    /// <param name="buffer">Input buffer.</param>
    /// <param name="output">Output digest. (Must be 4 bytes long)</param>
    /// <exception cref="ArgumentException"></exception>
    internal static void ComputeDigest(ReadOnlySpan<byte> buffer, Span<byte> output)
    {
        if (output.Length != DigestBytes)
        {
            throw new ArgumentException($"Output must be {DigestBytes} bytes long", nameof(output));
        }

        Span<byte> tempResult = stackalloc byte[Bytes];
        Compute(buffer, tempResult);
        tempResult[..DigestBytes].CopyTo(output);
    }

    internal static void Compute(ReadOnlySpan<byte> buffer, Span<byte> output)
    {
        if (!SHA256.TryHashData(buffer, output, out int bytesWritten))
        {
            throw new InvalidOperationException("Couldn't compute SHA256");
        }

        if (bytesWritten != Bytes)
        {
            throw new InvalidOperationException("Invalid SHA256 length");
        }
    }
}
