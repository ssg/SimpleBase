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
        public unsafe string Encode(byte[] bytes)
        {
            const int growthPercentage = 138;

            Require.NotNull(bytes, "buffer");
            int bytesLen = bytes.Length;
            if (bytesLen == 0)
            {
                return String.Empty;
            }
            fixed (byte* inputPtr = bytes)
            fixed (char* alphabetPtr = alphabet.Value)
            {
                byte* pInput = inputPtr;
                byte* pEnd = inputPtr + bytesLen;
                while (pInput != pEnd && *pInput == 0)
                {
                    pInput++;
                }
                int numZeroes = (int)(pInput - inputPtr);
                char zeroChar = alphabetPtr[0];
                if (pInput == pEnd)
                {
                    return new String(zeroChar, numZeroes);
                }

                int outputLen = bytesLen * growthPercentage / 100 + 1;
                int length = 0;
                byte[] output = new byte[outputLen];
                fixed (byte* outputPtr = output)
                {
                    byte* pOutputEnd = outputPtr + outputLen - 1;
                    while (pInput != pEnd)
                    {
                        int carry = *pInput;
                        int i = 0;
                        for (byte* pDigit = pOutputEnd; (carry != 0 || i < length) 
                            && pDigit >= outputPtr; pDigit--, i++)
                        {
                            carry += 256 * (*pDigit);
                            *pDigit = (byte)(carry % 58);
                            carry /= 58;
                        }
                        length = i;
                        pInput++;
                    }

                    pOutputEnd++;
                    byte* pOutput = outputPtr;
                    while (pOutput != pOutputEnd && *pOutput == 0)
                    {
                        pOutput++;
                    }

                    int resultLen = numZeroes + (int)(pOutputEnd - pOutput);
                    string result = new String(zeroChar, resultLen);
                    fixed (char* resultPtr = result)
                    {
                        char* pResult = resultPtr + numZeroes;
                        while (pOutput != pOutputEnd)
                        {
                            *pResult++ = alphabetPtr[*pOutput++];
                        }
                    }
                    return result;
                }
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
            const int reductionFactor = 733; // https://github.com/bitcoin/bitcoin/blob/master/src/base58.cpp

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

                int outputLen = textLen * reductionFactor / 1000 + 1;
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
                    if (resultLen == outputLen)
                    {
                        return output;
                    }
                    byte[] result = new byte[numZeroes + resultLen];                    
                    Array.Copy(output, (int)(pOutput - outputPtr), result, numZeroes, resultLen);
                    return result;
                }
            }
        }
    }
}