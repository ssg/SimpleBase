// <copyright file="Base58Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Base58 alphabet.
/// </summary>
public sealed class Base58Alphabet : CodingAlphabet
{
    private static readonly Lazy<Base58Alphabet> bitcoinAlphabet = new(()
        => new Base58Alphabet("123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"));

    private static readonly Lazy<Base58Alphabet> rippleAlphabet = new(()
        => new Base58Alphabet("rpshnaf39wBUDNEGHJKLM4PQRST7VWXYZ2bcdeCg65jkm8oFqi1tuvAxyz"));

    private static readonly Lazy<Base58Alphabet> flickrAlphabet = new(()
        => new Base58Alphabet("123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ"));

    /// <summary>
    /// Initializes a new instance of the <see cref="Base58Alphabet"/> class
    /// using a custom alphabet.
    /// </summary>
    /// <param name="alphabet">Alphabet to use.</param>
    public Base58Alphabet(string alphabet)
        : base(58, alphabet)
    {
    }

    /// <summary>
    /// Gets Bitcoin alphabet.
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
