// <copyright file="Base85Ipv6.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace SimpleBase;

/// <summary>
/// Base85 implementation with additional IPv6 coding functions.
/// </summary>
/// <remarks>
/// RFC 1924 sucks, arguably because it's a very early proposal in the history of IPv6:
/// - It contains special chars: It's prone to be confused with other syntactical elements.
///   It can even cause security issues due to poor escaping, let alone UX problems.
/// - Length gains are usually marginal: IPv6 uses zero elimination to reduce the address representation.
/// - Slow. The algorithm is division based, instead of faster bitwise operations.
/// So, that's why I only included a proof of concept implementation instead of working on optimizing it.
/// RFC 1924 should die, and this code should only be used to support some obscure standard or code somewhere.
/// </remarks>
public class Base85Ipv6 : Base85
{
    private const int ipv6bytes = 16;
    private const int ipv6chars = 20;
    private static readonly BigInteger divisor = new(85);

    /// <summary>
    /// Initializes a new instance of the <see cref="Base85Ipv6"/> class.
    /// </summary>
    /// <param name="alphabet">Coding alphabet.</param>
    public Base85Ipv6(Base85Alphabet alphabet)
        : base(alphabet)
    {
    }

    /// <summary>
    /// Encode IPv6 address into RFC 1924 Base85 text.
    /// </summary>
    /// <param name="ip">IPv6 address.</param>
    /// <returns>Encoded text.</returns>
    public string EncodeIpv6(IPAddress ip)
    {
        if (ip.AddressFamily != AddressFamily.InterNetworkV6)
        {
            throw new ArgumentException("Invalid IP address type. RFC 1924 is only defined for IPv6 addresses");
        }

        Span<byte> buffer = stackalloc byte[ipv6bytes];
        if (!ip.TryWriteBytes(buffer, out int bytesWritten))
        {
            throw new InvalidOperationException($"Buffer is too small for this IP address: {ip}");
        }

        var num = new BigInteger(buffer, isUnsigned: true, isBigEndian: true);
        Span<char> str = new char[Base85Ipv6.ipv6chars];
        for (int n = 0, o = ipv6chars - 1; n < ipv6chars; n++, o--)
        {
            num = BigInteger.DivRem(num, divisor, out var remainder);
            str[o] = Alphabet.Value[(int)remainder];
        }

        return str.ToString();
    }

    /// <summary>
    /// Decode an RFC 1924 encoded text into an IPv6 address.
    /// </summary>
    /// <param name="text">Encoded text.</param>
    /// <returns>Decoded IPv6 address.</returns>
    public IPAddress DecodeIpv6(string text)
    {
        if (text.Length != ipv6chars)
        {
            throw new ArgumentException("Invalid encoded IPv6 text length");
        }

        BigInteger num = 0;
        for (int n = 0; n < ipv6chars; n++)
        {
            char c = text[n];
            int value = Alphabet.ReverseLookupTable[c] - 1;
            if (value < 0)
            {
                throw new InvalidOperationException($"Invalid character: {c}");
            }

            num = (num * divisor) + value;
        }

        Span<byte> buffer = stackalloc byte[ipv6bytes];
        return num.TryWriteBytes(buffer, out int bytesWritten, isUnsigned: false, isBigEndian: true)
            ? new IPAddress(buffer)
            : throw new InvalidOperationException("Destination buffer is too small");
    }
}
