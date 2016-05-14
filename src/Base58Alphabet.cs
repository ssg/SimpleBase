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
    public sealed class Base58Alphabet
    {
        public const int Length = 58;

        public string Value { get; private set; }

        public Base58Alphabet(string text)
        {
            Require.NotNull(text, "text");
            if (text.Length != Length)
            {
                throw new ArgumentException("Base58 alphabets need to be 58-characters long", "text");
            }
            this.Value = text;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}