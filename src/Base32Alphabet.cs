// <copyright file="Base32Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase
{
    /// <summary>
    /// Base32 alphabet flavors.
    /// </summary>
    public class Base32Alphabet : EncodingAlphabet
    {
        private static Lazy<CrockfordBase32Alphabet> crockfordAlphabet = new Lazy<CrockfordBase32Alphabet>(
            () => new CrockfordBase32Alphabet());

        private static Lazy<Base32Alphabet> rfc4648Alphabet = new Lazy<Base32Alphabet>(
            () => new Base32Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"));

        private static Lazy<Base32Alphabet> extendedHexAlphabet = new Lazy<Base32Alphabet>(
            () => new Base32Alphabet("0123456789ABCDEFGHIJKLMNOPQRSTUV"));

        private static Lazy<Base32Alphabet> zBase32Alphabet = new Lazy<Base32Alphabet>(
            () => new Base32Alphabet("ybndrfg8ejkmcpqxot1uwisza345h769"));

        private static Lazy<Base32Alphabet> geohashAlphabet = new Lazy<Base32Alphabet>(
            () => new Base32Alphabet("0123456789bcdefghjkmnpqrstuvwxyz"));

        /// <summary>
        /// Initializes a new instance of the <see cref="Base32Alphabet"/> class.
        /// </summary>
        /// <param name="alphabet">Characters.</param>
        public Base32Alphabet(string alphabet)
            : base(32, alphabet)
        {
            if (alphabet is null)
            {
                throw new ArgumentNullException(nameof(alphabet));
            }

            this.mapLowerCaseCounterparts(alphabet);
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