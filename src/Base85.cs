// <copyright file="Base85.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SimpleBase
{
    /// <summary>
    /// Base58 encoding/decoding class.
    /// </summary>
    public sealed class Base85
    {
        private const int baseLength = 85;
        private const int byteBlockSize = 4;
        private const int stringBlockSize = 5;
        private const long allSpace = 0x20202020;
        private const int decodeBufferSize = 5120; // don't remember what was special with this number

        private static Lazy<Base85> z85 = new Lazy<Base85>(() => new Base85(Base85Alphabet.Z85));
        private static Lazy<Base85> ascii85 = new Lazy<Base85>(() => new Base85(Base85Alphabet.Ascii85));

        private readonly Base85Alphabet alphabet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Base85"/> class
        /// using a custom alphabet.
        /// </summary>
        /// <param name="alphabet">Alphabet to use.</param>
        public Base85(Base85Alphabet alphabet)
        {
            this.alphabet = alphabet;
        }

        /// <summary>
        /// Gets Z85 flavor of Base85.
        /// </summary>
        public static Base85 Z85 => z85.Value;

        /// <summary>
        /// Gets Ascii85 flavor of Base85.
        /// </summary>
        public static Base85 Ascii85 => ascii85.Value;

        /// <summary>
        /// Encode the given bytes into Base85.
        /// </summary>
        /// <param name="bytes">Bytes to encode.</param>
        /// <returns>Encoded text.</returns>
        public unsafe string Encode(ReadOnlySpan<byte> bytes)
        {
            int bytesLen = bytes.Length;
            if (bytesLen == 0)
            {
                return string.Empty;
            }

            bool usesZeroShortcut = this.alphabet.AllZeroShortcut.HasValue;
            bool usesSpaceShortcut = this.alphabet.AllSpaceShortcut.HasValue;

            // adjust output length based on prefix and suffix settings
            int maxOutputLen = (bytesLen * stringBlockSize / byteBlockSize) + 1;

            char[] output = new char[maxOutputLen];
            int fullLen = (bytesLen >> 2) << 2; // rounded

            string table = this.alphabet.Value;

            fixed (byte* inputPtr = bytes)
            fixed (char* outputPtr = output)
            {
                char* pOutput = outputPtr;
                byte* pInput = inputPtr;
                byte* pInputEnd = pInput + fullLen;
                while (pInput != pInputEnd)
                {
                    // build a 32-bit representation of input
                    long input = ((uint)*pInput++ << 24)
                        | ((uint)*pInput++ << 16)
                        | ((uint)*pInput++ << 8)
                        | *pInput++;

                    writeOutput(
                        ref pOutput,
                        table,
                        input,
                        stringBlockSize,
                        usesZeroShortcut,
                        usesSpaceShortcut);
                }

                // check if a part is remaining
                int remainingBytes = bytesLen - fullLen;
                if (remainingBytes > 0)
                {
                    long input = 0;
                    for (int n = 0; n < remainingBytes; n++)
                    {
                        input |= (uint)*pInput++ << ((3 - n) << 3);
                    }

                    writeOutput(
                        ref pOutput,
                        table,
                        input,
                        remainingBytes + 1,
                        usesZeroShortcut,
                        usesSpaceShortcut);
                }

                int outputLen = (int)(pOutput - outputPtr);
                return new string(outputPtr, 0, outputLen);
            }
        }

        /// <summary>
        /// Encode a given stream into a text writer.
        /// </summary>
        /// <param name="input">Input stream.</param>
        /// <param name="output">Output writer.</param>
        public void Encode(Stream input, TextWriter output)
        {
            StreamHelper.Encode(input, output, (buffer, lastBlock) => Encode(buffer.Span));
        }

        /// <summary>
        /// Encode a given stream into a text writer.
        /// </summary>
        /// <param name="input">Input stream.</param>
        /// <param name="output">Output writer.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task EncodeAsync(Stream input, TextWriter output)
        {
            await StreamHelper.EncodeAsync(input, output, (buffer, lastBlock) => Encode(buffer.Span))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Decode a text reader into a stream.
        /// </summary>
        /// <param name="input">Input reader.</param>
        /// <param name="output">Output stream.</param>
        public void Decode(TextReader input, Stream output)
        {
            StreamHelper.Decode(input, output, (text) => Decode(text.Span).ToArray(), decodeBufferSize);
        }

        /// <summary>
        /// Decode a text reader into a stream.
        /// </summary>
        /// <param name="input">Input reader.</param>
        /// <param name="output">Output stream.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DecodeAsync(TextReader input, Stream output)
        {
            await StreamHelper.DecodeAsync(input, output, (text) => Decode(text.Span).ToArray(), decodeBufferSize)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Decode a given text into a span.
        /// </summary>
        /// <param name="text">Input text.</param>
        /// <returns>Output span.</returns>
        public Span<byte> Decode(string text)
        {
            return Decode(text.AsSpan());
        }

        /// <summary>
        /// Decode given characters into bytes.
        /// </summary>
        /// <param name="text">Characters to decode.</param>
        /// <returns>Decoded bytes.</returns>
        public unsafe Span<byte> Decode(ReadOnlySpan<char> text)
        {
            int textLen = text.Length;
            if (textLen == 0)
            {
                return Array.Empty<byte>();
            }

            char? allZeroChar = this.alphabet.AllZeroShortcut;
            char? allSpaceChar = this.alphabet.AllSpaceShortcut;
            bool checkZero = allZeroChar.HasValue;
            bool checkSpace = allSpaceChar.HasValue;
            bool usingShortcuts = checkZero || checkSpace;

            // allocate a larger buffer if we're using shortcuts
            int decodeBufferLen = getDecodeBufferLength(textLen, usingShortcuts);
            byte[] decodeBuffer = new byte[decodeBufferLen];
            var table = this.alphabet.ReverseLookupTable;
            fixed (char* inputPtr = text)
            fixed (byte* decodeBufferPtr = decodeBuffer)
            {
                byte* pDecodeBuffer = decodeBufferPtr;
                char* pInput = inputPtr;
                char* pInputEnd = pInput + textLen;

                int blockIndex = 0;
                long value = 0;
                while (pInput != pInputEnd)
                {
                    char c = *pInput++;

                    if (isWhiteSpace(c))
                    {
                        continue;
                    }

                    // handle shortcut characters
                    if (checkZero && c == allZeroChar)
                    {
                        writeShortcut(ref pDecodeBuffer, ref blockIndex, 0);
                        continue;
                    }

                    if (checkSpace && c == allSpaceChar)
                    {
                        writeShortcut(ref pDecodeBuffer, ref blockIndex, allSpace);
                        continue;
                    }

                    // handle regular blocks
                    int x = table[c] - 1; // map character to byte value
                    if (x < 0)
                    {
                        throw EncodingAlphabet.InvalidCharacter(c);
                    }

                    value = (value * baseLength) + x;
                    blockIndex += 1;
                    if (blockIndex == stringBlockSize)
                    {
                        writeDecodedValue(ref pDecodeBuffer, value, byteBlockSize);
                        blockIndex = 0;
                        value = 0;
                    }
                }

                if (blockIndex > 0)
                {
                    // handle padding by treating the rest of the characters
                    // as "u"s. so both big endianness and bit weirdness work out okay.
                    for (int i = 0; i < stringBlockSize - blockIndex; i++)
                    {
                        value = (value * baseLength) + (baseLength - 1);
                    }

                    writeDecodedValue(ref pDecodeBuffer, value, blockIndex - 1);
                }

                int actualOutputLength = (int)(pDecodeBuffer - decodeBufferPtr);
                return new Span<byte>(decodeBufferPtr, actualOutputLength);
            }
        }

        private static unsafe int getDecodeBufferLength(int textLen, bool usingShortcuts)
        {
            if (usingShortcuts)
            {
                return textLen * byteBlockSize; // max possible size using shortcuts
            }

            // max possible size without shortcuts
            return (((textLen - 1) / stringBlockSize) + 1) * byteBlockSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void writeShortcut(ref byte* pOutput, ref int blockIndex, long value)
        {
            if (blockIndex != 0)
            {
                throw new ArgumentException(
                    $"Unexpected shortcut character in the middle of a regular block");
            }

            writeDecodedValue(ref pOutput, value, byteBlockSize);
            blockIndex = 0; // restart block after the shortcut character
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void writeDecodedValue(ref byte* pOutput, long value, int numBytesToWrite)
        {
            for (int i = byteBlockSize - 1; i >= 0 && numBytesToWrite > 0; i--, numBytesToWrite--)
            {
                byte b = (byte)((value >> (i << 3)) & 0xFF);
                *pOutput++ = b;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool isWhiteSpace(char c)
        {
            return c == ' ' || c == 0x85 || c == 0xA0 || (c >= 0x09 && c <= 0x0D);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void writeOutput(
            ref char* pOutput,
            string table,
            long input,
            int stringLength,
            bool usesZeroShortcut,
            bool usesSpaceShortcut)
        {
            // handle shortcuts
            if (input == 0 && usesZeroShortcut)
            {
                *pOutput++ = this.alphabet.AllZeroShortcut ?? '!'; // guaranteed to be non-null
                return;
            }

            if (input == allSpace && usesSpaceShortcut)
            {
                *pOutput++ = this.alphabet.AllSpaceShortcut ?? '!'; // guaranteed to be non-null
                return;
            }

            // map the 4-byte packet to to 5-byte octets
            for (int i = stringBlockSize - 1; i >= 0; i--)
            {
                input = Math.DivRem(input, baseLength, out long result);
                pOutput[i] = table[(int)result];
            }

            pOutput += stringLength;
        }
    }
}