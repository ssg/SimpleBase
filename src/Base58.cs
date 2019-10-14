// <copyright file="Base58.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase
{
    /// <summary>
    /// Base58 Encoding/Decoding implementation.
    /// </summary>
    /// <remarks>
    /// Base58 doesn't implement a Stream-based interface because it's not feasible to use
    /// on large buffers.
    /// </remarks>
    public sealed class Base58 : IBaseEncoder
    {
        private static Lazy<Base58> bitcoin = new Lazy<Base58>(() => new Base58(Base58Alphabet.Bitcoin));
        private static Lazy<Base58> ripple = new Lazy<Base58>(() => new Base58(Base58Alphabet.Ripple));
        private static Lazy<Base58> flickr = new Lazy<Base58>(() => new Base58(Base58Alphabet.Flickr));

        private Base58Alphabet alphabet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Base58"/> class
        /// using a custom alphabet.
        /// </summary>
        /// <param name="alphabet">Alphabet to use.</param>
        public Base58(Base58Alphabet alphabet)
        {
            this.alphabet = alphabet;
        }

        /// <summary>
        /// Gets Bitcoin flavor.
        /// </summary>
        public static Base58 Bitcoin => bitcoin.Value;

        /// <summary>
        /// Gets Ripple flavor.
        /// </summary>
        public static Base58 Ripple => ripple.Value;

        /// <summary>
        /// Gets Flickr flavor.
        /// </summary>
        public static Base58 Flickr => flickr.Value;

        /// <summary>
        /// Encode to Base58 representation.
        /// </summary>
        /// <param name="bytes">Bytes to encode.</param>
        /// <returns>Encoded string.</returns>
        public unsafe string Encode(ReadOnlySpan<byte> bytes)
        {
            int bytesLen = bytes.Length;
            if (bytesLen == 0)
            {
                return string.Empty;
            }

            fixed (byte* inputPtr = bytes)
            fixed (char* alphabetPtr = this.alphabet.Value)
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
                    return new string(zeroChar, numZeroes);
                }

                // we can safely use char count for allocating a byte buffer for the numeric representation
                // because each digit will map to a single character later in the encoding process.
                int outputLen = Base58Alphabet.GetAllocationCharCountForEncoding(bytesLen, numZeroes);
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
                            carry = Math.DivRem(carry, 58, out int remainder);
                            *pDigit = (byte)remainder;
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
                    string result = new string(zeroChar, resultLen);
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

        /// <summary>
        /// Decode a Base58 representation.
        /// </summary>
        /// <param name="text">Encoded text.</param>
        /// <returns>Decoded bytes.</returns>
        public Span<byte> Decode(string text)
        {
            return Decode(text.AsSpan());
        }

        /// <summary>
        /// Decode a Base58 representation.
        /// </summary>
        /// <param name="text">Base58 encoded text.</param>
        /// <returns>Array of decoded bytes.</returns>
        public unsafe Span<byte> Decode(ReadOnlySpan<char> text)
        {
            int textLen = text.Length;
            if (textLen == 0)
            {
                return Array.Empty<byte>();
            }

            fixed (char* inputPtr = text)
            {
                char* pEnd = inputPtr + textLen;
                char* pInput = inputPtr;
                char zeroChar = this.alphabet.Value[0];
                while (*pInput == zeroChar && pInput != pEnd)
                {
                    pInput++;
                }

                int numZeroes = (int)(pInput - inputPtr);
                if (pInput == pEnd)
                {
                    return new byte[numZeroes]; // initialized to zero
                }

                int outputLen = alphabet.GetAllocationByteCountForDecoding(text);
                var table = this.alphabet.ReverseLookupTable;
                byte[] output = new byte[outputLen];
                fixed (byte* outputPtr = output)
                {
                    byte* pOutputEnd = outputPtr + outputLen - 1;
                    byte* pMinOutput = pOutputEnd;
                    while (pInput != pEnd)
                    {
                        char c = *pInput;
                        int carry = table[c] - 1;
                        if (carry < 0)
                        {
                            throw EncodingAlphabet.InvalidCharacter(c);
                        }

                        byte* pOutput = pOutputEnd;
                        for (; pOutput >= outputPtr; pOutput--)
                        {
                            carry += 58 * (*pOutput);
                            *pOutput = (byte)carry;
                            if (pMinOutput > pOutput && carry != 0)
                            {
                                pMinOutput = pOutput;
                            }

                            carry /= 256;
                        }

                        pInput++;
                    }

                    pMinOutput -= numZeroes;
                    return output.AsSpan((int)(pMinOutput - outputPtr));
                }
            }
        }
    }
}