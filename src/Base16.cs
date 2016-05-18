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

        public static byte[] Decode(string text)
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
            unchecked
            {
                byte[] output = new byte[textLen / 2];
                for (int i = 0, j = 0; i < textLen; j++)
                {
                    char c1 = text[i++];
                    if (!isValidHexChar(c1))
                    {
                        throw new InvalidOperationException(String.Format("Invalid hex character: ", c1));
                    }
                    int b1 = getHexByte(c1);

                    char c2 = text[i++];
                    if (!isValidHexChar(c2))
                    {
                        throw new InvalidOperationException(String.Format("Invalid hex character: ", c2));
                    }
                    int b2 = getHexByte(c2);
                    output[j] = (byte)(b1 << 4 | b2);
                }
                return output;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int getHexByte(int c)
        {
            c -= numberOffset;
            if (c < 10)
            {
                return c;
            }
            c -= upperNumberDiff;
            if (c < 16)
            {
                return c;
            }
            return c - lowerUpperDiff;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool isValidHexChar(int c)
        {
            return (c >= '0' && c <= '9')
                || (c >= 'A' && c <= 'F')
                || (c >= 'a' && c <= 'f');
        }
    }
}