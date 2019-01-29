// <copyright file="Base58Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

/*
     Copyright 2014-2016 Sedat Kapanoglu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

namespace SimpleBase
{
    /// <summary>
    /// Base58 alphabet
    /// </summary>
    public sealed class Base58Alphabet : EncodingAlphabet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Base58Alphabet"/> class
        /// using a custom alphabet.
        /// </summary>
        /// <param name="alphabet">Alphabet to use</param>
        public Base58Alphabet(string alphabet)
            : base(58, alphabet)
        {
        }

        /// <summary>
        /// Gets Bitcoin alphabet
        /// </summary>
        public static Base58Alphabet Bitcoin { get; }
            = new Base58Alphabet("123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz");

        /// <summary>
        /// Gets Base58 alphabet
        /// </summary>
        public static Base58Alphabet Ripple { get; }
            = new Base58Alphabet("rpshnaf39wBUDNEGHJKLM4PQRST7VWXYZ2bcdeCg65jkm8oFqi1tuvAxyz");

        /// <summary>
        /// Gets Flickr alphabet
        /// </summary>
        public static Base58Alphabet Flickr { get; }
            = new Base58Alphabet("123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ");
    }
}