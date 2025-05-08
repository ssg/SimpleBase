// <copyright file="Base58Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Base62 alphabet.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Base58Alphabet"/> class
/// using a custom alphabet.
/// </remarks>
/// <param name="alphabet">Alphabet to use.</param>
public sealed class Base62Alphabet(string alphabet) : CodingAlphabet(62, alphabet)
{
    static readonly Lazy<Base62Alphabet> standardAlphabet = new(()
        => new Base62Alphabet("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"));

    static readonly Lazy<Base62Alphabet> alternativeAlphabet = new(()
        => new Base62Alphabet("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"));

    /// <summary>
    /// Gets Standard alphabet.
    /// </summary>
    public static Base62Alphabet Standard => standardAlphabet.Value;

    /// <summary>
    /// Gets Alternative alphabet.
    /// </summary>
    public static Base62Alphabet Alternative => alternativeAlphabet.Value;

}
