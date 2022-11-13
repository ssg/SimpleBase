// <copyright file="Base32Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Base32 alphabet flavors.
/// </summary>
public class Base32Alphabet : CodingAlphabet
{
    private static readonly Lazy<Base32Alphabet> rfc4648Alphabet = new
        (() => new Base32Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"));

    private static readonly Lazy<Base32Alphabet> extendedHexAlphabet = new
        (() => new Base32Alphabet("0123456789ABCDEFGHIJKLMNOPQRSTUV"));

    private static readonly Lazy<Base32Alphabet> zBase32Alphabet = new
        (() => new Base32Alphabet("ybndrfg8ejkmcpqxot1uwisza345h769"));

    private static readonly Lazy<Base32Alphabet> geohashAlphabet = new
        (() => new Base32Alphabet("0123456789bcdefghjkmnpqrstuvwxyz"));

    private static readonly Lazy<Base32Alphabet> bech32Alphabet = new
        (() => new Base32Alphabet("qpzry9x8gf2tvdw0s3jn54khce6mua7l"));

    private static readonly Lazy<Base32Alphabet> fileCoinAlphabet = new
        (() => new Base32Alphabet("abcdefghijklmnopqrstuvwxyz234567"));

    private static readonly Lazy<AliasedBase32Alphabet> crockfordAlphabet = new
        (() => new AliasedBase32Alphabet(
            "0123456789ABCDEFGHJKMNPQRSTVWXYZ",
            new CharMap[]
            {
                new('O', '0'),
                new('I', '1'),
                new('L', '1'),
            }));

    private static readonly Lazy<AliasedBase32Alphabet> base32HAlphabet = new(
        () => new AliasedBase32Alphabet(
            "0123456789ABCDEFGHJKLMNPQRTVWXYZ",
            paddingChar: '0',
            PaddingPosition.Start,
            new CharMap[]
            {
                new('O', '0'),
                new('I', '1'),
                new('S', '5'),
                new('U', 'V'),
            }));

    /// <summary>
    /// Initializes a new instance of the <see cref="Base32Alphabet"/> class.
    /// </summary>
    /// <param name="alphabet">Characters.</param>
    public Base32Alphabet(string alphabet)
        : base(32, alphabet)
    {
        mapLowerCaseCounterparts(alphabet);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Base32Alphabet"/> class.
    /// </summary>
    /// <param name="alphabet">Encoding alphabet to use.</param>
    /// <param name="paddingChar">Padding character.</param>
    /// <param name="paddingPosition">Position of the padding characters in the encoder output.</param>
    public Base32Alphabet(string alphabet, char paddingChar, PaddingPosition paddingPosition)
        : this(alphabet)
    {
        PaddingChar = paddingChar;
        PaddingPosition = paddingPosition;
    }

    /// <summary>
    /// Gets Crockford alphabet.
    /// </summary>gpg
    public static Base32Alphabet Crockford => crockfordAlphabet.Value;

    /// <summary>
    /// Gets RFC4648 alphabet.
    /// </summary>
    public static Base32Alphabet Rfc4648 => rfc4648Alphabet.Value;

    /// <summary>
    /// Gets Extended Hex alphabet.
    /// </summary>
    public static Base32Alphabet ExtendedHex => extendedHexAlphabet.Value;

    /// <summary>
    /// Gets z-base-32 alphabet.
    /// </summary>
    public static Base32Alphabet ZBase32 => zBase32Alphabet.Value;

    /// <summary>
    /// Gets Geohash alphabet.
    /// </summary>
    public static Base32Alphabet Geohash => geohashAlphabet.Value;

    /// <summary>
    /// Gets FileCoin alphabet.
    /// </summary>
    public static Base32Alphabet FileCoin => fileCoinAlphabet.Value;

    /// <summary>
    /// Gets Base32H alphabet.
    /// </summary>
    public static Base32Alphabet Base32H => base32HAlphabet.Value;

    /// <summary>
    /// Gets Bech32 alphabet.
    /// </summary>
    public static Base32Alphabet Bech32 => bech32Alphabet.Value;

    /// <summary>
    /// Gets the padding character used in encoding.
    /// </summary>
    public char PaddingChar { get; } = '=';

    /// <summary>
    /// Gets the position of the padding characters in the encoder output.
    /// </summary>
    public PaddingPosition PaddingPosition { get; } = PaddingPosition.End;

    private void mapLowerCaseCounterparts(string alphabet)
    {
        foreach (char c in alphabet)
        {
            if (char.IsUpper(c))
            {
                Map(char.ToLowerInvariant(c), ReverseLookupTable[c] - 1);
            }
        }
    }
}
