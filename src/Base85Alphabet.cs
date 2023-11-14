// <copyright file="Base85Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Base85 Alphabet.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Base85Alphabet"/> class
/// using custom settings.
/// </remarks>
/// <param name="alphabet">Alphabet to use.</param>
/// <param name="allZeroShortcut">Character to substitute for all zero.</param>
/// <param name="allSpaceShortcut">Character to substitute for all space.</param>
public sealed class Base85Alphabet(
    string alphabet,
    char? allZeroShortcut = null,
    char? allSpaceShortcut = null) : CodingAlphabet(85, alphabet)
{
    private static readonly Lazy<Base85Alphabet> z85 = new(() => new Base85Alphabet(
            "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ.-:+=^!/*?&<>()[]{}@%$#"));

    private static readonly Lazy<Base85Alphabet> ascii85 = new(() => new Base85Alphabet(
            "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstu",
            allZeroShortcut: 'z',
            allSpaceShortcut: 'y'));

    private static readonly Lazy<Base85Alphabet> rfc1924 = new(() => new Base85Alphabet(
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!#$%&()*+-;<=>?@^_`{|}~"));

    /// <summary>
    /// Gets ZeroMQ Z85 Alphabet.
    /// </summary>
    public static Base85Alphabet Z85 => z85.Value;

    /// <summary>
    /// Gets Adobe Ascii85 Alphabet (each character is directly produced by raw value + 33),
    /// also known as "btoa" encoding.
    /// </summary>
    public static Base85Alphabet Ascii85 => ascii85.Value;

    /// <summary>
    /// Gets Base85 encoding defined in RFC 1924.
    /// </summary>
    public static Base85Alphabet Rfc1924 => rfc1924.Value;

    /// <summary>
    /// Gets the character to be used for "all zeros".
    /// </summary>
    public char? AllZeroShortcut { get; } = allZeroShortcut;

    /// <summary>
    /// Gets the character to be used for "all spaces".
    /// </summary>
    public char? AllSpaceShortcut { get; } = allSpaceShortcut;

    /// <summary>
    /// Gets a value indicating whether the alphabet uses one of shortcut characters for all spaces
    /// or all zeros.
    /// </summary>
    public bool HasShortcut => AllSpaceShortcut.HasValue || AllZeroShortcut.HasValue;
}
