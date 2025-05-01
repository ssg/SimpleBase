// <copyright file="Base58Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Base58 alphabet.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Base58Alphabet"/> class
/// using a custom alphabet.
/// </remarks>
/// <param name="alphabet">Alphabet to use.</param>
public sealed class Base58Alphabet(string alphabet) : CodingAlphabet(58, alphabet)
{
    static readonly Lazy<Base58Alphabet> bitcoinAlphabet = new(()
        => new Base58Alphabet("123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"));

    static readonly Lazy<Base58Alphabet> rippleAlphabet = new(()
        => new Base58Alphabet("rpshnaf39wBUDNEGHJKLM4PQRST7VWXYZ2bcdeCg65jkm8oFqi1tuvAxyz"));

    static readonly Lazy<Base58Alphabet> flickrAlphabet = new(()
        => new Base58Alphabet("123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ"));

    /// <summary>
    /// Gets Bitcoin alphabet. Monero also uses this alphabet but only works with MoneroBase58 encoding.
    /// </summary>
    public static Base58Alphabet Bitcoin => bitcoinAlphabet.Value;

    /// <summary>
    /// Gets Base58 alphabet.
    /// </summary>
    public static Base58Alphabet Ripple => rippleAlphabet.Value;

    /// <summary>
    /// Gets Flickr alphabet.
    /// </summary>
    public static Base58Alphabet Flickr => flickrAlphabet.Value;
}
