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
using System.Text;

namespace SimpleBase
{
    public static class Base58
    {
        public static readonly Base58Alphabet BitcoinAlphabet = new Base58Alphabet("123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz");
        public static readonly Base58Alphabet RippleAlphabet = new Base58Alphabet("rpshnaf39wBUDNEGHJKLM4PQRST7VWXYZ2bcdeCg65jkm8oFqi1tuvAxyz");
        public static readonly Base58Alphabet FlickrAlphabet = new Base58Alphabet("123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ");

        public static string Encode(byte[] bytes, Base58Alphabet alphabet)
        {            
            Require.NotNull(bytes, "buffer");
            Require.NotNull(alphabet, "alphabet");
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
            if (numZeroes == buflen)
            {
                return new String(alphabet.Value[0], numZeroes);
            }
            var newBuffer = bytes;
            if (numZeroes > 0)
            {
                int newLen = buflen - numZeroes;
                newBuffer = new byte[newLen + 1];
                Array.Copy(bytes, numZeroes, newBuffer, 0, newLen);
                convertToLittleEndian(ref newBuffer, newLen);
            }
            var builder = new StringBuilder(buflen * 2);
            var num = new BigInteger(newBuffer);            
            while (num > 0)
            {
                int remainder = (int)(num % Base58Alphabet.Length);
                builder.Append(alphabet.Value[remainder]);
                num /= Base58Alphabet.Length;
            }
            if (numZeroes > 0)
            {
                builder.Append(new String(alphabet.Value[0], numZeroes));
            }
            return reverse(builder.ToString());
        }

        private static void convertToLittleEndian(ref byte[] newBuffer, int length)
        {
            int endPos = length - 1;
            for (int n = 0; n <= endPos / 2; n++)
            {
                byte tmp = newBuffer[n];
                newBuffer[n] = newBuffer[endPos - n];
                newBuffer[endPos - n] = tmp;
            }
        }

        private static string reverse(string input)
        {
            var builder = new StringBuilder();
            for (int n = input.Length - 1; n >= 0; n--)
            {
                builder.Append(input[n]);
            }
            return builder.ToString();
        }

        public static byte[] Decode(string text, string alphabet)
        {
            return null;
        }
    }
}