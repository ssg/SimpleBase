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
using System.Threading;

namespace SimpleBase
{
    public sealed class Base32
    {
        /// <summary>
        /// Douglas Crockford's Base32 flavor with substitution characters.
        /// </summary>
        public static Base32 Crockford
        {
            get
            {
                LazyInitializer.EnsureInitialized(ref crockford, () => new Base32(new CrockfordBase32Alphabet()));
                return crockford;
            }
        }

        /// <summary>
        /// RFC 4648 variant of Base32 converter
        /// </summary>
        public static Base32 Rfc4648
        {
            get
            {
                LazyInitializer.EnsureInitialized(ref rfc4648, () => new Base32(Base32Alphabet.Rfc4648));
                return rfc4648;
            }
        }

        /// <summary>
        /// Extended Hex variant of Base32 converter
        /// </summary>
        /// <remarks>Also from RFC 4648</remarks>
        public static Base32 ExtendedHex
        {
            get
            {
                LazyInitializer.EnsureInitialized(ref extendedHex, () => new Base32(Base32Alphabet.ExtendedHex));
                return extendedHex;
            }
        }

        private static Base32 crockford;
        private static Base32 rfc4648;
        private static Base32 extendedHex;

        private const int bitsPerByte = 8;
        private const int bitsPerChar = 5;
        private const char paddingChar = '=';

        private Base32Alphabet alphabet;

        private Base32(Base32Alphabet alphabet)
        {
            this.alphabet = alphabet;
        }

        /// <summary>
        /// Encode a byte array into a Base32 string
        /// </summary>
        /// <param name="bytes">Buffer to be encoded</param>
        /// <param name="padding">Append padding characters in the output</param>
        /// <returns>Encoded string</returns>
        public string Encode(byte[] bytes, bool padding)
        {
            Require.NotNull(bytes, "bytes");
            int len = bytes.Length;
            if (len == 0)
            {
                return String.Empty;
            }

            var encodingTable = alphabet.EncodingTable;
            int bitsLeft = bitsPerByte;
            int i = 0;
            int b = bytes[i];
            int outputPos = 0;
            int outputLen = getEncodingOutputLength(len, padding);
            var outputBuffer = new char[outputLen];
            while (i < len)
            {
                int output;
                if (bitsLeft > bitsPerChar)
                {
                    bitsLeft -= bitsPerChar;
                    output = b >> bitsLeft;
                    outputBuffer[outputPos++] = encodingTable[output];
                    b &= (1 << bitsLeft) - 1;
                }
                int nextBits = bitsPerChar - bitsLeft;
                output = b << nextBits;
                bitsLeft = bitsPerByte - nextBits;
                i++;
                if (i < len)
                {
                    b = bytes[i];
                    output |= b >> bitsLeft;
                    b &= (1 << bitsLeft) - 1;
                }
                outputBuffer[outputPos++] = encodingTable[output];
            }
            if (padding)
            {
                while (outputPos < outputLen)
                {
                    outputBuffer[outputPos++] = paddingChar;
                }
            }
            return new string(outputBuffer, 0, outputPos);
        }

        private static readonly int[] paddingRemainders = new int[] { 0, 2, 4, 5, 7 };

        private static int getEncodingOutputLength(int len, bool padding)
        {
            if (padding)
            {
                return (((len - 1) / bitsPerChar) + 1) * bitsPerByte;
            }
            int outputLen = len * bitsPerByte / bitsPerChar;
            return outputLen + paddingRemainders[outputLen % bitsPerChar];
        }

        /// <summary>
        /// Decode a Base32 encoded string into a byte array.
        /// </summary>
        /// <param name="base32">Encoded Base32 string</param>
        /// <returns>Decoded byte array</returns>
        public byte[] Decode(string base32)
        {
            if (base32 == null)
            {
                throw new ArgumentNullException("base32");
            }
            base32 = base32.TrimEnd(paddingChar);
            int len = base32.Length;
            if (len == 0)
            {
                return new byte[0];
            }
            var decodingTable = alphabet.DecodingTable;
            int decodingTableLen = decodingTable.Length;
            int outputPos = 0;
            int output = 0;
            int i = 0;
            int bitsLeft = bitsPerByte;
            byte[] outputBuffer = createDecodingOutput(len);
            while (i < len)
            {
                char c = base32[i];
                if (c >= decodingTableLen)
                {
                    throw invalidInput(c);
                }
                int b = decodingTable[c] - 1;
                if (b < 0)
                {
                    throw invalidInput(c);
                }
                if (bitsLeft > bitsPerChar)
                {
                    bitsLeft -= bitsPerChar;
                    output |= b << bitsLeft;
                    i++;
                    continue;
                }
                int shiftBits = bitsPerChar - bitsLeft;
                output |= b >> shiftBits;
                outputBuffer[outputPos] = (byte)output;
                outputPos++;
                b &= (1 << shiftBits) - 1;
                bitsLeft = bitsPerByte - shiftBits;
                output = b << bitsLeft;
                i++;
            }
            return outputBuffer;
        }

        private static ArgumentException invalidInput(char c)
        {
            return new ArgumentException(String.Format("Invalid character value in input: 0x{0:X}", (int)c), "base32");
        }

        private static byte[] createDecodingOutput(int len)
        {
            int outputLen = len * bitsPerChar / bitsPerByte;
            return new byte[outputLen];
        }
    }
}