// <copyright file="Base85Ipv6.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Net;
using System.Net.Sockets;

namespace SimpleBase;

/// <summary>
/// Base85 implementation with additional IPv6 coding functions.
/// </summary>
public class Base85Ipv6 : Base85
{
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
        return ip.AddressFamily == AddressFamily.InterNetworkV6
            ? Encode(ip.GetAddressBytes())
            : throw new ArgumentException(
                "Invalid IP address type. RFC 1924 is only defined for IPv6 addresses");
    }

    /// <summary>
    /// Decode an RFC 1924 encoded text into an IPv6 address.
    /// </summary>
    /// <param name="text">Encoded text.</param>
    /// <returns>Decoded IPv6 address.</returns>
    public IPAddress DecodeIpv6(string text)
    {
        var bytes = Decode(text);
        return bytes.Length == 16
            ? new IPAddress(bytes)
            : throw new ArgumentException(
                "Invalid encoded IP address. RFC 1924 is only defined for IPv6 addresses");
    }
}
