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
    public abstract class EncodingAlphabet
    {
        public int Length { get; }

        public string Value { get; }

        /// <summary>
        /// Specifies the highest possible char value in an encoding alphabet
        /// Any char above with would raise an exception
        /// </summary>
        private const int lookupLength = 127;

        /// <summary>
        /// Holds a mapping from character to an actual byte value
        /// The values are held as "value + 1" so a zero would denote "not set"
        /// and would cause an exception.
        /// </summary>
        /// byte[] has no discernible perf impact and saves memory
        internal readonly byte[] ReverseLookupTable = new byte[lookupLength]; 

        public Exception InvalidCharacter(char c)
        {
            return new ArgumentException($"Invalid character: {c}");
        }

        public EncodingAlphabet(int length, string alphabet)
        {
            Require.NotNull(alphabet, nameof(alphabet));
            if (alphabet.Length != length)
            {
                throw new ArgumentException($"Required alphabet length is {length} but provided alphabet is "
                                          + $"{alphabet.Length} characters long");
            }
            Length = length;
            Value = alphabet;

            for (short i = 0; i < length; i++)
            {
                Map(alphabet[i], i);
            }
        }

        protected void Map(char c, int value)
        {
            if (c >= lookupLength)
            {
                throw new InvalidOperationException($"Alphabet contains character above {lookupLength}");
            }
            ReverseLookupTable[c] = (byte)(value + 1);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}