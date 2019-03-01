// <copyright file="Base32Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

namespace SimpleBase
{
    using System;
    using System.Threading;

    /// <summary>
    /// Base32 alphabet flavors
    /// </summary>
    public class Base32Alphabet : EncodingAlphabet
    {
        private static CrockfordBase32Alphabet crockford;
        private static Base32Alphabet rfc4648;
        private static Base32Alphabet extendedHex;
        private static Base32Alphabet zBase32;
        private static Base32Alphabet geohash;

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
        public static Base32Alphabet Crockford => LazyInitializer.EnsureInitialized(
            ref crockford,
            () => new CrockfordBase32Alphabet());

        /// <summary>
        /// Gets RFC4648 alphabet
        /// </summary>
        public static Base32Alphabet Rfc4648 => LazyInitializer.EnsureInitialized(
            ref rfc4648,
            () => new Base32Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"));

        /// <summary>
        /// Gets Extended Hex alphabet
        /// </summary>
        public static Base32Alphabet ExtendedHex => LazyInitializer.EnsureInitialized(
            ref extendedHex,
            () => new Base32Alphabet("0123456789ABCDEFGHIJKLMNOPQRSTUV"));

        /// <summary>
        /// Gets z-base-32 alphabet
        /// </summary>
        public static Base32Alphabet ZBase32 => LazyInitializer.EnsureInitialized(
            ref zBase32,
            () => new Base32Alphabet("ybndrfg8ejkmcpqxot1uwisza345h769"));

        /// <summary>
        /// Gets Geohash alphabet
        /// </summary>
        public static Base32Alphabet Geohash => LazyInitializer.EnsureInitialized(
            ref geohash,
            () => new Base32Alphabet("0123456789bcdefghjkmnpqrstuvwxyz"));

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