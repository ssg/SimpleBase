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

using System;

namespace SimpleBase
{
    public class Base32Alphabet : EncodingAlphabet
    {
        public static Base32Alphabet Crockford { get; } = new CrockfordBase32Alphabet();

        public static Base32Alphabet Rfc4648 { get; }
            = new Base32Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ234567");

        public static Base32Alphabet ExtendedHex { get; }
            = new Base32Alphabet("0123456789ABCDEFGHIJKLMNOPQRSTUV");

        public Base32Alphabet(string alphabet) : base(32, alphabet)
        {
            mapLowerCaseCounterparts(alphabet);
        }

        private void mapLowerCaseCounterparts(string alphabet)
        {
            foreach (char c in alphabet)
            {
                if (Char.IsUpper(c))
                {
                    Map(Char.ToLowerInvariant(c), ReverseLookupTable[c] - 1);
                }
            }
        }
    }
}