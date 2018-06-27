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
        /// <summary>
        /// Encode to Base16 representation using uppercase lettering
        /// </summary>
        /// <param name="bytes">Bytes to encode</param>
        /// <returns>Base16 string</returns>
        public unsafe static string EncodeUpper(ReadOnlySpan<byte> bytes)
        {
            return encode(bytes, 'A');
        }

        /// <summary>
        /// Encode to Base16 representation using lowercase lettering
        /// </summary>
        /// <param name="bytes">Bytes to encode</param>
        /// <returns>Base16 string</returns>
        public unsafe static string EncodeLower(ReadOnlySpan<byte> bytes)
        {
            return encode(bytes, 'a');
        }

        private static unsafe string encode(ReadOnlySpan<byte> bytes, char baseChar)
        {
            int bytesLen = bytes.Length;
            if (bytesLen == 0)
            {
                return String.Empty;
            }
            var output = new String('\0', bytesLen << 1);
            fixed (char* outputPtr = output)
            fixed (byte* bytesPtr = bytes)
            {
                char* pOutput = outputPtr;
                byte* pInput = bytesPtr;

                char a = baseChar;

                byte hex(byte b) => (b < 10) ? (byte)('0' + b) : (byte)(a + b - 10);

                int octets = bytesLen / 2;
                for (int i = 0; i < octets; i++, pInput += 2, pOutput += 4)
                {
                    // reduce memory accesses by reading and writing 8 bytes at once
                    ushort pair = *(ushort*)pInput;
                    ulong pad = hex((byte)((pair >> 4) & 0x0F))
                            | ((ulong)hex((byte)(pair & 0x0F)) << 16)
                            | ((ulong)hex((byte)(pair >> 12)) << 32)
                            | ((ulong)hex((byte)((pair >> 8) & 0x0F)) << 48);
                    *((ulong*)pOutput) = pad;
                }
                if (bytesLen % 2 > 0)
                {
                    byte b = *pInput++;
                    *pOutput++ = (char)hex((byte)(b >> 4));
                    *pOutput++ = (char)hex((byte)(b & 0x0F));
                }
            }
            return output;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char getHexCharLower(int b) => (b < 10) ? (char)('0' + b) : (char)('a' + b);

        /// <summary>
        /// Decode Base16 text into bytes
        /// </summary>
        /// <param name="text">Base16 text</param>
        /// <returns>Decoded bytes</returns>
        public static unsafe Span<byte> Decode(string text)
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