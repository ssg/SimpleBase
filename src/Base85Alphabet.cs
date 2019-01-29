// <copyright file="Base85Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

/*
     Copyright 2018 Sedat Kapanoglu

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
    public sealed class Base85Alphabet: EncodingAlphabet
    {
        public const char NoShortcut = '\xFFFF';

        public char AllZeroShortcut { get; }  = NoShortcut;
        public char AllSpaceShortcut { get; } = NoShortcut;

        public bool HasShortcut => AllSpaceShortcut != NoShortcut || AllZeroShortcut != NoShortcut;

        /// <summary>
        /// ZeroMQ Z85 Alphabet
        /// </summary>
        public static Base85Alphabet Z85 { get; } 
            = new Base85Alphabet("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ.-:+=^!/*?&<>()[]{}@%$#");

        /// <summary>
        /// Adobe Ascii85 Alphabet (each character is directly produced by raw value + 33)
        /// </summary>
        public static Base85Alphabet Ascii85 { get; }
            = new Base85Alphabet("!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstu", 
                allZeroShortcut: 'z', allSpaceShortcut: 'y');

        public Base85Alphabet(string alphabet, char allZeroShortcut = NoShortcut,
            char allSpaceShortcut = NoShortcut): base(85, alphabet)
        {
            AllZeroShortcut = allZeroShortcut;
            AllSpaceShortcut = allSpaceShortcut;
        }
    }
}
