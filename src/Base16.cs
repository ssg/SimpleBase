using System;
using System.Runtime.CompilerServices;

namespace SimpleBase
{
    public enum Base16Style
    {
        Lowercase,
        Uppercase
    }

    public static class Base16
    {
        private const byte upperCaseOffset = 55;
        private const byte lowerCaseOffset = 87;
        private const byte numberOffset = 48;

        private const byte upperNumberDiff = 7;
        private const byte lowerUpperDiff = 32;

        private const string lowerAlphabet = "0123456789abcdef";
        private const string upperAlphabet = "0123456789ABCDEF";

        /// <summary>
        /// Encode using uppercase lettering
        /// </summary>
        public static string EncodeUpper(byte[] bytes)
        {
            return encode(bytes, upperAlphabet);
        }

        public static string EncodeLower(byte[] bytes)
        {
            return encode(bytes, lowerAlphabet);
        }

        private static string encode(byte[] bytes, string alphabet)
        {
            Require.NotNull(bytes, "bytes");
            int bytesLen = bytes.Length;
            if (bytesLen == 0)
            {
                return String.Empty;
            }
            unchecked
            {
                char[] output = new char[bytesLen * 2];
                for (int i = 0, j = 0; i < bytesLen; i++)
                {
                    int b = bytes[i];
                    output[j++] = alphabet[b >> 4];
                    output[j++] = alphabet[b & 0x0F];
                }
                return new String(output);
            }
        }

        public static unsafe byte[] Decode(string text)
        {
            Require.NotNull(text, "text");
            int textLen = text.Length;
            if (textLen == 0)
            {
                return new byte[] { };
            }
            if (textLen % 2 != 0)
            {
                throw new ArgumentException("Text cannot be odd length", "text");
            }
            byte[] output = new byte[textLen / 2];
            fixed (byte* outputPtr = output)
            fixed (char* textPtr = text)
            {
                byte* op = outputPtr;                
                for (char* ip = textPtr, ep = ip + textLen; ip != ep; op++)
                {
                    char c1 = *ip++;
                    validateHex(c1);
                    var b1 = getHexByte(c1);
                    char c2 = *ip++;
                    validateHex(c2);
                    var b2 = getHexByte(c2);
                    *op = (byte)(b1 << 4 | b2);
                }            
            }
            return output;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int getHexByte(int character)
        {
            int c = character - numberOffset;
            if (c < 10) // is number?
            {
                return c;
            }
            c -= upperNumberDiff;
            if (c < 16) // is uppercase?
            {
                return c;
            }
            return c - lowerUpperDiff;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void validateHex(char c)
        {
            if (!((c >= '0' && c <= '9')
                || (c >= 'A' && c <= 'F')
                || (c >= 'a' && c <= 'f')))
            {
                throw new InvalidOperationException(String.Format("Invalid hex character: ", c));
            }
        }
    }
}