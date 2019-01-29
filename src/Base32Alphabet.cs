// <copyright file="Base32Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

namespace SimpleBase
{
    /// <summary>
    /// Base32 alphabet flavors
    /// </summary>
    public class Base32Alphabet : EncodingAlphabet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Base32Alphabet"/> class.
        /// </summary>
        /// <param name="alphabet">Characters</param>
        public Base32Alphabet(string alphabet)
            : base(32, alphabet)
        {
            this.mapLowerCaseCounterparts(alphabet);
        }

        /// <summary>
        /// Gets Crockford alphabet
        /// </summary>
        public static Base32Alphabet Crockford { get; } = new CrockfordBase32Alphabet();

        /// <summary>
        /// Gets RFC4648 alphabet
        /// </summary>
        public static Base32Alphabet Rfc4648 { get; }
            = new Base32Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ234567");

        /// <summary>
        /// Gets Extended Hex alphabet
        /// </summary>
        public static Base32Alphabet ExtendedHex { get; }
            = new Base32Alphabet("0123456789ABCDEFGHIJKLMNOPQRSTUV");

        private void mapLowerCaseCounterparts(string alphabet)
        {
            foreach (char c in alphabet)
            {
                if (char.IsUpper(c))
                {
                    this.Map(char.ToLowerInvariant(c), this.ReverseLookupTable[c] - 1);
                }
            }
        }
    }
}