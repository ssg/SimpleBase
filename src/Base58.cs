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
using System.Numerics;

namespace SimpleBase
{
    public sealed class Base58
    {
        public static readonly Base58 Bitcoin = new Base58(Base58Alphabet.Bitcoin);
        public static readonly Base58 Ripple = new Base58(Base58Alphabet.Ripple);
        public static readonly Base58 Flickr = new Base58(Base58Alphabet.Flickr);

        private Base58Alphabet alphabet;

        public Base58(Base58Alphabet alphabet)
        {
            Require.NotNull(alphabet, "alphabet");
            this.alphabet = alphabet;
        }

        private static readonly BigInteger baseLength = Base58Alphabet.Length;

        /// <summary>
        /// Encode to Base58 representation
        /// </summary>
        /// <param name="bytes">Bytes to encode</param>
        /// <returns>Encoded string</returns>
        public string Encode(byte[] bytes)
        {
            const int growthPercentage = 138;

            Require.NotNull(bytes, "buffer");
            int buflen = bytes.Length;
            if (buflen == 0)
            {
                return String.Empty;
            }
            int numZeroes = 0;
            while (numZeroes < buflen && bytes[numZeroes] == 0)
            {
                numZeroes++;
            }
            string zeroes = new String(alphabet[0], numZeroes);
            if (numZeroes == buflen)
            {
                return zeroes;
            }
            var newBuffer = bytes;
            unchecked
            {
                int newLen = buflen - numZeroes;
                newBuffer = new byte[newLen + 1];
                Array.Copy(bytes, numZeroes, newBuffer, 0, newLen);
                Reverse(newBuffer, newLen);
                char[] output = new char[buflen * growthPercentage / 100 + 1];
                int outputLen = output.Length;
                int outputPos = outputLen - 1;
                var num = new BigInteger(newBuffer);
                while (num > 0)
                {
                    BigInteger remainder;
                    num = BigInteger.DivRem(num, baseLength, out remainder);
                    output[outputPos--] = alphabet[(int)remainder];
                }
                outputPos++;
                return zeroes + new String(output, outputPos, outputLen - outputPos);
            }
        }

        internal static unsafe void Reverse(byte[] buffer, int length)
        {
            fixed (byte* inputPtr = buffer)
            {
                Reverse(inputPtr, length);
            }
        }

        internal static unsafe void Reverse(byte* inputPtr, int length)
        {
            if (length < 2)
            {
                return;
            }
            for (byte* pStart = inputPtr, pEnd = inputPtr + length - 1; pStart < pEnd; pStart++, pEnd--)
            {
                byte tmp = *pEnd;
                *pEnd = *pStart;
                *pStart = tmp;
            }
        }

        /// <summary>
        /// Decode a Base58 representation
        /// </summary>
        /// <param name="text">Base58 encoded text</param>
        /// <returns>Array of decoded bytes</returns>
        public unsafe byte[] Decode(string text)
        {
            Require.NotNull(text, "text");
            var textLen = text.Length;
            if (textLen == 0)
            {
                return new byte[] { };
            }

            fixed (char* inputPtr = text)
            {
                char* pEnd = inputPtr + textLen;
                char* pInput = inputPtr;
                char zeroChar = alphabet[0];
                while (*pInput == zeroChar && pInput != pEnd)
                {
                    pInput++;
                }

                int numZeroes = (int)(pInput - inputPtr);
                if (pInput == pEnd)
                {
                    return new byte[numZeroes]; // initialized to zero
                }

                int outputLen = textLen * 733 / 1000 + 1; // https://github.com/bitcoin/bitcoin/blob/master/src/base58.cpp
                byte[] output = new byte[outputLen]; 
                fixed (byte* outputPtr = output)
                {
                    byte* pOutputEnd = outputPtr + outputLen - 1;
                    while (pInput != pEnd)
                    {
                        int carry = alphabet[*pInput++];
                        for (byte* pDigit = pOutputEnd; pDigit >= outputPtr; pDigit--)
                        {
                            carry += 58 * (*pDigit);
                            *pDigit = (byte)carry;
                            carry /= 256;
                        }
                    }

                    byte* pOutput = outputPtr;
                    while (pOutput != pOutputEnd && *pOutput == 0)
                    {
                        pOutput++;
                    }

                    int resultLen = (int)(pOutputEnd - pOutput) + 1;
                    byte[] result = new byte[numZeroes + resultLen];                    
                    Array.Copy(output, (int)(pOutput - outputPtr), result, numZeroes, resultLen);
                    return result;
                }
            }
        }
    }
}