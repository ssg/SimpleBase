// <copyright file="Base45Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Base45 coding alphabet.
/// </summary>
/// <param name="alphabet">The characters that build up the Base45 alphabet.</param>
public class Base45Alphabet(string alphabet) : CodingAlphabet(45, alphabet)
{
    static readonly Lazy<Base45Alphabet> @default = new
        (() => new ("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:"));

    /// <summary>
    /// Default Base45 alphabet per RFC 9285.
    /// </summary>
    public static Base45Alphabet Default => @default.Value;
}
