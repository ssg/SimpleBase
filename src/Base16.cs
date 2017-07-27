using System;
using System.Runtime.CompilerServices;

namespace SimpleBase
{
    /// <summary>
    /// Hexadecimal encoding/decoding
    /// </summary>
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
            Require.NotNull(bytes, "bytes");
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
                char* pAlphabet = alphabetPtr;
                byte* pInput = bytesPtr;
                byte* pEnd = Pointer.Offset(pInput, bytesLen);
                while (pInput != pEnd)
                {
                    int b = *pInput;
                    *pOutput++ = pAlphabet[b >> 4];
                    *pOutput++ = pAlphabet[b & 0x0F];
                    pInput++;
                }
            }
            return output;
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
                byte* pOutput = outputPtr;
                char* pInput = textPtr;
                char* pEnd = Pointer.Offset(pInput, textLen);
                while (pInput != pEnd)
                {
                    char c1 = *pInput++;
                    validateHex(c1);
                    var b1 = getHexByte(c1);
                    char c2 = *pInput++;
                    validateHex(c2);
                    var b2 = getHexByte(c2);
                    *pOutput = (byte)(b1 << 4 | b2);
                    pOutput++;
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