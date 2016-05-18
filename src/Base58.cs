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
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

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
                reverse(ref newBuffer, newLen);
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

        private static void reverse(ref byte[] newBuffer, int length)
        {
            int endPos = length - 1;
            for (int n = 0; n <= endPos / 2; n++)
            {
                byte tmp = newBuffer[n];
                newBuffer[n] = newBuffer[endPos - n];
                newBuffer[endPos - n] = tmp;
            }
        }

        public byte[] Decode(string text)
        {
            Require.NotNull(text, "text");
            char zeroVal = alphabet[0];
            var textLen = text.Length;
            if (textLen == 0)
            {
                return new byte[] { };
            }

            int numZeroes = 0;
            while (numZeroes < textLen && text[numZeroes] == zeroVal)
            {
                numZeroes++;
            }
            if (numZeroes == textLen)
            {
                return new byte[numZeroes]; // initialized to zero
            }
            BigInteger num = 0; 
            for (int i = numZeroes; i < textLen; i++)
            {
                char c = text[i];
                int val = alphabet[c];
                num = num * 58 + val;
            }

            var bytes = num.ToByteArray();
            int resultLen = bytes.Length;
            reverse(ref bytes, resultLen);
            int resultIndex = 0;
            while (resultIndex < resultLen)
            {
                if (bytes[resultIndex] != 0)
                {
                    break;
                }
                resultIndex++;
            }
            int outputLen = resultLen + numZeroes - resultIndex;
            byte[] result = new byte[outputLen];
            Array.Copy(bytes, resultIndex, result, numZeroes, resultLen - resultIndex);
            return result;
        }
    }
}