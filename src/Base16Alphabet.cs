// <copyright file="Base16Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Alphabet representation for Base16 encodings.
/// </summary>
public class Base16Alphabet : CodingAlphabet
{
    private static readonly Lazy<Base16Alphabet> upperCaseAlphabet = new(() => new Base16Alphabet("0123456789ABCDEF"));

    private static readonly Lazy<Base16Alphabet> lowerCaseAlphabet = new(() => new Base16Alphabet("0123456789abcdef"));

    private static readonly Lazy<Base16Alphabet> modHexAlphabet = new(() => new Base16Alphabet("cbdefghijklnrtuv"));

    /// <summary>
    /// Initializes a new instance of the <see cref="Base16Alphabet"/> class with
    /// case insensitive semantics.
    /// </summary>
    /// <param name="alphabet">Encoding alphabet.</param>
    public Base16Alphabet(string alphabet)
        : this(alphabet, caseSensitive: false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Base16Alphabet"/> class.
    /// </summary>
    /// <param name="alphabet">Encoding alphabet.</param>
    /// <param name="caseSensitive">If the decoding should be performed case sensitive.</param>
    public Base16Alphabet(string alphabet, bool caseSensitive)
        : base(16, alphabet)
    {
        if (!caseSensitive)
        {
            mapCounterparts();
        }
    }

    /// <summary>
    /// Gets upper case Base16 alphabet.
    /// </summary>
    public static Base16Alphabet UpperCase { get; } = upperCaseAlphabet.Value;

    /// <summary>
    /// Gets lower case Base16 alphabet.
    /// </summary>
    public static Base16Alphabet LowerCase { get; } = lowerCaseAlphabet.Value;

    /// <summary>
    /// Gets ModHex Base16 alphabet, used by Yubico apps.
    /// </summary>
    public static Base16Alphabet ModHex { get; } = modHexAlphabet.Value;

    /// <summary>
    /// Gets a value indicating whether the decoding should be performed in a case sensitive fashion.
    /// The default is false.
    /// </summary>
    public bool CaseSensitive { get; }

    private void mapCounterparts()
    {
        int alphaLen = Value.Length;
        for (int i = 0; i < alphaLen; i++)
        {
            char c = Value[i];
            if (char.IsLetter(c))
            {
                if (char.IsUpper(c))
                {
                    Map(char.ToLowerInvariant(c), i);
                }

                if (char.IsLower(c))
                {
                    Map(char.ToUpperInvariant(c), i);
                }
            }
        }
    }
}
