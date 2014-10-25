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
using System.Linq;
using System.Collections.Generic;

namespace SimpleBase32
{
    internal sealed class CrockfordBase32Alphabet : Base32Alphabet
    {
        public CrockfordBase32Alphabet()
            : base("0123456789ABCDEFGHJKMNPQRSTVWXYZ")
        {
            var buf = DecodingTable;
            map(buf, 'O', '0');
            map(buf, 'I', '1');
            map(buf, 'L', '1');
        }

        private static void map(byte[] buffer, char source, char destination)
        {
            var result = buffer[destination];
            buffer[source] = result;
            buffer[Char.ToLowerInvariant(source)] = result;
        }
    }
}