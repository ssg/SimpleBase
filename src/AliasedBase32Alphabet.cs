// <copyright file="AliasedBase32Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System.Collections.Generic;

namespace SimpleBase;

/// <summary>
/// Case-insensitive Base32 alphabet with character aliases. This is useful for alphabets like Crockford or Base32H,
/// where certain characters are considered equivalent to others (e.g., 'O' is treated as '0', 'I' and 'L' are
/// treated as '1') to improve readability.
/// </summary>
sealed class AliasedBase32Alphabet : Base32Alphabet
{
    public AliasedBase32Alphabet(string alphabet, IEnumerable<CharMap> map)
        : base(alphabet)
    {
        setupMap(map);
    }

    public AliasedBase32Alphabet(
        string alphabet,
        char paddingChar,
        IEnumerable<CharMap> map)
        : base(alphabet, paddingChar)
    {
        setupMap(map);
    }

    void setupMap(IEnumerable<CharMap> map)
    {
        foreach (var (from, to) in map)
        {
            mapAlternate(from, to);
        }
    }

    void mapAlternate(char source, char destination)
    {
        int result = ReverseLookupTable[destination] - 1;
        Map(source, result);
        Map(char.ToLowerInvariant(source), result);
    }
}
