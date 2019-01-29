// <copyright file="CrockfordBase32Alphabet.cs" company="Sedat Kapanoglu">
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
    using System;

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