// <copyright file="Base36.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Base36 encoding/decoding alphabet.
/// </summary>
/// <param name="alphabet">Alphabet to use.</param>
public class Base36Alphabet(string alphabet) : CodingAlphabet(36, alphabet, caseInsensitive: true)
{
    static readonly Lazy<Base36Alphabet> upperAlphabet = new(() => new Base36Alphabet("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));

    static readonly Lazy<Base36Alphabet> lowerAlphabet = new(() => new Base36Alphabet("0123456789abcdefghijklmnopqrstuvwxyz"));

    /// <summary>
    /// Base36 alphabet with numbers and uppercase letters.
    /// </summary>
    public static Base36Alphabet Upper => upperAlphabet.Value;

    /// <summary>
    /// Base36 alphabet with numbers and lowercase letters.
    /// </summary>
    public static Base36Alphabet Lower => lowerAlphabet.Value;
}