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

namespace SimpleBase
{
    public sealed class Base32
    {
        /// <summary>
        /// Douglas Crockford's Base32 flavor with substitution characters.
        /// </summary>
        public static readonly Base32 Crockford = new Base32(Base32Alphabet.Crockford);

        /// <summary>
        /// RFC 4648 variant of Base32 converter
        /// </summary>
        public static readonly Base32 Rfc4648 = new Base32(Base32Alphabet.Rfc4648);

        /// <summary>
        /// Extended Hex variant of Base32 converter
        /// </summary>
        /// <remarks>Also from RFC 4648</remarks>
        public static readonly Base32 ExtendedHex = new Base32(Base32Alphabet.ExtendedHex);

        private const int bitsPerByte = 8;
        private const int bitsPerChar = 5;
        private const char paddingChar = '=';

        private readonly Base32Alphabet alphabet;

        public Base32(Base32Alphabet alphabet)
        {
            this.alphabet = alphabet;
        }

        /// <summary>
        /// Encode a byte array into a Base32 string
        /// </summary>
        /// <param name="bytes">Buffer to be encoded</param>
        /// <param name="padding">Append padding characters in the output</param>
        /// <returns>Encoded string</returns>
        public unsafe string Encode(byte[] bytes, bool padding)
        {
            Require.NotNull(bytes, nameof(bytes));
            int bytesLen = bytes.Length;
            if (bytesLen == 0)
            {
                return String.Empty;
            }

            // we are ok with slightly larger buffer since the output string will always
            // have the exact length of the output produced.
            int outputLen = (((bytesLen - 1) / bitsPerChar) + 1) * bitsPerByte;
            var outputBuffer = new char[outputLen];

            string table = alphabet.Value;
            fixed (byte* inputPtr = bytes)
            fixed (char* outputPtr = outputBuffer)
            {
                char* pOutput = outputPtr;
                char* pOutputEnd = outputPtr + outputLen;
                byte* pInput = inputPtr;

                int bitsLeft = bitsPerByte;
                int currentByte = *pInput;
                byte* pEnd = pInput + bytesLen;
                while (pInput != pEnd)
                {
                    int outputPad;
                    if (bitsLeft > bitsPerChar)
                    {
                        bitsLeft -= bitsPerChar;
                        outputPad = currentByte >> bitsLeft;
                        *pOutput++ = table[outputPad];
                        currentByte &= (1 << bitsLeft) - 1;
                    }
                    int nextBits = bitsPerChar - bitsLeft;
                    bitsLeft = bitsPerByte - nextBits;
                    outputPad = currentByte << nextBits;
                    if (++pInput != pEnd)
                    {
                        currentByte = *pInput;
                        outputPad |= currentByte >> bitsLeft;
                        currentByte &= (1 << bitsLeft) - 1;
                    }
                    *pOutput++ = alphabet.Value[outputPad];
                }
                if (padding)
                {
                    while (pOutput != pOutputEnd)
                    {
                        *pOutput++ = paddingChar;
                    }
                }
                return new String(outputPtr, 0, (int)(pOutput - outputPtr));
            }
        }

        /// <summary>
        /// Decode a Base32 encoded string into a byte array.
        /// </summary>
        /// <param name="text">Encoded Base32 string</param>
        /// <returns>Decoded byte array</returns>
        public unsafe byte[] Decode(string text)
        {
            Require.NotNull(text, nameof(text));
            text = text.TrimEnd(paddingChar);
            int textLen = text.Length;
            if (textLen == 0)
            {
                return new byte[0];
            }
            int bitsLeft = bitsPerByte;
            int outputLen = textLen * bitsPerChar / bitsPerByte;
            var outputBuffer = new byte[outputLen];
            int outputPad = 0;
            byte[] table = alphabet.ReverseLookupTable;

            fixed (byte* outputPtr = outputBuffer)
            fixed (char* inputPtr = text)
            {
                byte* pOutput = outputPtr;
                char* pInput = inputPtr;
                char* pEnd = inputPtr + textLen;
                while (pInput != pEnd)
                {
                    char c = *pInput++;
                    int b = table[c] - 1;
                    if (b < 0)
                    {
                        throw alphabet.InvalidCharacter(c);
                    }
                    if (bitsLeft > bitsPerChar)
                    {
                        bitsLeft -= bitsPerChar;
                        outputPad |= b << bitsLeft;
                        continue;
                    }
                    int shiftBits = bitsPerChar - bitsLeft;
                    outputPad |= b >> shiftBits;
                    *pOutput++ = (byte)outputPad;
                    b &= (1 << shiftBits) - 1;
                    bitsLeft = bitsPerByte - shiftBits;
                    outputPad = b << bitsLeft;
                }
            }
            return outputBuffer;
        }
    }
}