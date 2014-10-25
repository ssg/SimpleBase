/*
     Copyright 2014 Sedat Kapanoglu

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
using System.Collections.Generic;
using System.Linq;

namespace SimpleBase32
{
    internal class Base32Alphabet
    {
        public const int Length = 32;
        const char highestAsciiCharSupported = 'z';

        public static Base32Alphabet Crockford = new CrockfordBase32Alphabet();
        public static Base32Alphabet Rfc4648 = new Base32Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ234567");
        public static Base32Alphabet ExtendedHex = new Base32Alphabet("0123456789ABCDEFGHIJKLMNOPQRSTUV");

        public string EncodingTable { get; private set; }
        public byte[] DecodingTable { get; protected set; }

        public Base32Alphabet(string chars)
        {
            this.EncodingTable = chars;
            createDecodingTable(chars);
        }

        private void createDecodingTable(string chars)
        {
            var bytes = new byte[highestAsciiCharSupported + 1];
            int len = chars.Length;
            for (int n = 0; n < len; n++)
            {
                char c = chars[n];
                byte b = (byte)(n + 1);
                bytes[c] = b;
                bytes[Char.ToLowerInvariant(c)] = b;
            }
            this.DecodingTable = bytes;
        }
    }
}