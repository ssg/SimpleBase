// <copyright file="AliasedBase32Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System.Collections.Generic;

namespace SimpleBase
{
    internal sealed class AliasedBase32Alphabet : Base32Alphabet
    {
        public AliasedBase32Alphabet(string alphabet, IEnumerable<(char from, char to)> map)
            : base(alphabet)
        {
            foreach (var (from, to) in map)
            {
                mapAlternate(from, to);
            }
        }

        private void mapAlternate(char source, char destination)
        {
            int result = this.ReverseLookupTable[destination] - 1;
            this.Map(source, result);
            this.Map(char.ToLowerInvariant(source), result);
        }
    }
}