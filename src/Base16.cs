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
using System.Runtime.CompilerServices;

namespace SimpleBase
{
    /// <summary>
    /// Hexadecimal encoding/decoding
    /// </summary>
    public static class Base16
    {
        private const string lowerAlphabet = "0123456789abcdef";
        private const string upperAlphabet = "0123456789ABCDEF";

        /// <summary>
        /// Encode to Base16 representation using uppercase lettering
        /// </summary>
        /// <param name="bytes">Bytes to encode</param>
        /// <returns>Base16 string</returns>
        public static string EncodeUpper(byte[] bytes)
        {
            return encode(bytes, upperAlphabet);
        }

        /// <summary>
        /// Encode to Base16 representation using lowercase lettering
        /// </summary>
        /// <param name="bytes">Bytes to encode</param>
        /// <returns>Base16 string</returns>
        public static string EncodeLower(byte[] bytes)
        {
            return encode(bytes, lowerAlphabet);
        }

        private static unsafe string encode(byte[] bytes, string alphabet)
        {
            Require.NotNull(bytes, nameof(bytes));
            int bytesLen = bytes.Length;
            if (bytesLen == 0)
            {
                return String.Empty;
            }
            var output = new String('\0', bytesLen * 2);
            fixed (char *outputPtr = output)
            fixed (byte *bytesPtr = bytes)
            fixed (char *alphabetPtr = alphabet)
            {
                char* pOutput = outputPtr;
                byte* pInput = bytesPtr;
                byte* pEnd = pInput + bytesLen;
                while (pInput != pEnd)
                {
                    int b = *pInput++;
                    char c1 = alphabetPtr[((uint)b) >> 4];
                    char c2 = alphabetPtr[b & 0x0F];
                    *pOutput++ = c1;
                    *pOutput++ = c2;
                }
            }
            return output;
        }

        public static unsafe byte[] Decode(string text)
        {
            Require.NotNull(text, nameof(text));
            int textLen = text.Length;
            if (textLen == 0)
            {
                return new byte[0];
            }
            if ((textLen & 1) != 0) // remainder ("%") was unexpectedly slow here
            {
                throw new ArgumentException("Text cannot be odd length", nameof(text));
            }
            byte[] output = new byte[textLen >> 1];
            fixed (byte* outputPtr = output)
            fixed (char* textPtr = text)
            {
                byte* pOutput = outputPtr;
                char* pInput = textPtr;
                char* pEnd = pInput + textLen;
                while (pInput != pEnd)
                {
                    char c1 = *pInput++;
                    int b1 = getHexByte(c1);
                    char c2 = *pInput++;
                    int b2 = getHexByte(c2);
                    *pOutput = (byte)(b1 << 4 | b2);
                    pOutput++;
                }            
            }
            return output;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int getHexByte(int c)
        {
            int n = c - '0';
            if (n < 0)
            {
                goto Error;
            }
            if (n < 10)
            {
                return n;
            }
            n = (c | ' ') - 'a' + 10;
            if (n < 0)
            {
                goto Error;
            }
            if (n <= 'z' - 'a')
            {
                return n;
            }
        Error:
            throw new ArgumentException($"Invalid hex character: {c}");
        }
    }
}