// <copyright file="CrockfordBase32Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase
{
    internal sealed class CrockfordBase32Alphabet : Base32Alphabet
    {
        public CrockfordBase32Alphabet()
            : base("0123456789ABCDEFGHJKMNPQRSTVWXYZ")
        {
            this.mapAlternate('O', '0');
            this.mapAlternate('I', '1');
            this.mapAlternate('L', '1');
        }

        private void mapAlternate(char source, char destination)
        {
            int result = this.ReverseLookupTable[destination] - 1;
            this.Map(source, result);
            this.Map(char.ToLowerInvariant(source), result);
        }
    }
}