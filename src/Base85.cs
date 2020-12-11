// <copyright file="Base85.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SimpleBase
{
    /// <summary>
    /// Base58 encoding/decoding class.
    /// </summary>
    public sealed class Base85 : IBaseEncoder, IBaseStreamEncoder, INonAllocatingBaseEncoder
    {
        private const int baseLength = 85;
        private const int byteBlockSize = 4;
        private const int stringBlockSize = 5;
        private const long allSpace = 0x20202020;
        private const int decodeBufferSize = 5120; // don't remember what was special with this number

        private static Lazy<Base85> z85 = new Lazy<Base85>(() => new Base85(Base85Alphabet.Z85));
        private static Lazy<Base85> ascii85 = new Lazy<Base85>(() => new Base85(Base85Alphabet.Ascii85));

        /// <summary>
        /// Initializes a new instance of the <see cref="Base85"/> class
        /// using a custom alphabet.
        /// </summary>
        /// <param name="alphabet">Alphabet to use.</param>
        public Base85(Base85Alphabet alphabet)
        {
            this.Alphabet = alphabet;
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
        /// Gets the encoding alphabet.
        /// </summary>
        public Base85Alphabet Alphabet { get; }

        /// <inheritdoc/>
        public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
        {
            bool usingShortcuts = Alphabet.AllZeroShortcut is object || Alphabet.AllSpaceShortcut is object;
            return getSafeByteCountForDecoding(text.Length, usingShortcuts);
        }

        /// <inheritdoc/>
        public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> bytes)
        {
            return getSafeCharCountForEncoding(bytes.Length);
        }

        /// <summary>
        /// Encode the given bytes into Base85.
        /// </summary>
        /// <param name="bytes">Bytes to encode.</param>
        /// <returns>Encoded text.</returns>
        public unsafe string Encode(ReadOnlySpan<byte> bytes)
        {
            int inputLen = bytes.Length;
            if (inputLen == 0)
            {
                return string.Empty;
            }

            int outputLen = GetSafeCharCountForEncoding(bytes);
            string output = new string('\0', outputLen);

            fixed (byte* inputPtr = bytes)
            fixed (char* outputPtr = output)
            {
                if (!internalEncode(inputPtr, inputLen, outputPtr, outputLen, out int numCharsWritten))
                {
                    throw new InvalidOperationException("Insufficient output buffer size while encoding Base85");
                }

                return output[..numCharsWritten];
            }
        }

        /// <inheritdoc/>
        public unsafe bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int numCharsWritten)
        {
            int inputLen = input.Length;
            if (inputLen == 0)
            {
                numCharsWritten = 0;
                return true;
            }

            fixed (byte* inputPtr = input)
            fixed (char* outputPtr = output)
            {
                return internalEncode(inputPtr, inputLen, outputPtr, output.Length, out numCharsWritten);
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

            bool usingShortcuts = Alphabet.HasShortcut;

            // allocate a larger buffer if we're using shortcuts
            int decodeBufferLen = getSafeByteCountForDecoding(textLen, usingShortcuts);
            byte[] decodeBuffer = new byte[decodeBufferLen];
            fixed (char* inputPtr = text)
            fixed (byte* decodeBufferPtr = decodeBuffer)
            {
                internalDecode(inputPtr, textLen, decodeBufferPtr, decodeBufferLen, out int numBytesWritten);
                return decodeBuffer.AsSpan()[..numBytesWritten];
            }
        }

        /// <inheritdoc/>
        public unsafe bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int numBytesWritten)
        {
            fixed (char* inputPtr = input)
            fixed (byte* outputPtr = output)
            {
                return internalDecode(inputPtr, input.Length, outputPtr, output.Length, out numBytesWritten);
            }
        }

        private unsafe bool internalEncode(
            byte* inputPtr,
            int inputLen,
            char* outputPtr,
            int outputLen,
            out int numCharsWritten)
        {
            bool usesZeroShortcut = Alphabet.AllZeroShortcut is object;
            bool usesSpaceShortcut = Alphabet.AllSpaceShortcut is object;
            string table = Alphabet.Value;
            int fullLen = (inputLen >> 2) << 2; // size of whole 4-byte blocks

            char* pOutput = outputPtr;
            char* pOutputEnd = pOutput + outputLen;
            byte* pInput = inputPtr;
            byte* pInputEnd = pInput + fullLen;
            while (pInput != pInputEnd)
            {
                // build a 32-bit representation of input
                long input = ((uint)*pInput++ << 24)
                    | ((uint)*pInput++ << 16)
                    | ((uint)*pInput++ << 8)
                    | *pInput++;

                if (!writeEncodedValue(
                    input,
                    ref pOutput,
                    pOutputEnd,
                    table,
                    stringBlockSize,
                    usesZeroShortcut,
                    usesSpaceShortcut))
                {
                    numCharsWritten = 0;
                    return false;
                }
            }

            // check if a part is remaining
            int remainingBytes = inputLen - fullLen;
            if (remainingBytes > 0)
            {
                long input = 0;
                for (int n = 0; n < remainingBytes; n++)
                {
                    input |= (uint)*pInput++ << ((3 - n) << 3);
                }

                if (!writeEncodedValue(
                    input,
                    ref pOutput,
                    pOutputEnd,
                    table,
                    remainingBytes + 1,
                    usesZeroShortcut,
                    usesSpaceShortcut))
                {
                    numCharsWritten = 0;
                    return false;
                }
            }

            numCharsWritten = (int)(pOutput - outputPtr);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe bool writeEncodedValue(
            long input,
            ref char* pOutput,
            char* pOutputEnd,
            string table,
            int stringLength,
            bool usesZeroShortcut,
            bool usesSpaceShortcut)
        {
            // handle shortcuts
            if (input == 0 && usesZeroShortcut)
            {
                if (pOutput >= pOutputEnd)
                {
                    return false;
                }

                *pOutput++ = this.Alphabet.AllZeroShortcut ?? '!'; // guaranteed to be non-null
                return true;
            }

            if (input == allSpace && usesSpaceShortcut)
            {
                if (pOutput >= pOutputEnd)
                {
                    return false;
                }

                *pOutput++ = this.Alphabet.AllSpaceShortcut ?? '!'; // guaranteed to be non-null
                return true;
            }

            if (pOutput >= pOutputEnd - stringLength)
            {
                return false;
            }

            // map the 4-byte packet to to 5-byte octets
            for (int i = stringBlockSize - 1; i >= 0; i--)
            {
                input = Math.DivRem(input, baseLength, out long result);
                if (i < stringLength)
                {
                    pOutput[i] = table[(int)result];
                }
            }

            pOutput += stringLength;
            return true;
        }

        private unsafe bool internalDecode(
           char* inputPtr,
           int inputLen,
           byte* outputPtr,
           int outputLen,
           out int numBytesWritten)
        {
            char? allZeroChar = Alphabet.AllZeroShortcut;
            char? allSpaceChar = Alphabet.AllSpaceShortcut;
            bool checkZero = allZeroChar is object;
            bool checkSpace = allSpaceChar is object;

            var table = this.Alphabet.ReverseLookupTable;
            byte* pOutput = outputPtr;
            char* pInput = inputPtr;
            char* pInputEnd = pInput + inputLen;
            byte* pOutputEnd = pOutput + outputLen;

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
                    if (!writeShortcut(ref pOutput, pOutputEnd, ref blockIndex, 0))
                    {
                        goto Error;
                    }

                    continue;
                }

                if (checkSpace && c == allSpaceChar)
                {
                    if (!writeShortcut(ref pOutput, pOutputEnd, ref blockIndex, allSpace))
                    {
                        goto Error;
                    }

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
                    if (!writeDecodedValue(ref pOutput, pOutputEnd, value, byteBlockSize))
                    {
                        goto Error;
                    }

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

                if (!writeDecodedValue(ref pOutput, pOutputEnd, value, blockIndex - 1))
                {
                    goto Error;
                }
            }

            numBytesWritten = (int)(pOutput - outputPtr);
            return true;
            Error:
            numBytesWritten = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool writeDecodedValue(
            ref byte* pOutput,
            byte* pOutputEnd,
            long value,
            int numBytesToWrite)
        {
            if (pOutput + numBytesToWrite > pOutputEnd)
            {
                Debug.WriteLine("Buffer overrun while decoding Base85");
                return false;
            }

            for (int i = byteBlockSize - 1; i >= 0 && numBytesToWrite > 0; i--, numBytesToWrite--)
            {
                byte b = (byte)((value >> (i << 3)) & 0xFF);
                *pOutput++ = b;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool isWhiteSpace(char c)
        {
            return c == ' ' || c == 0x85 || c == 0xA0 || (c >= 0x09 && c <= 0x0D);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool writeShortcut(
            ref byte* pOutput,
            byte* pOutputEnd,
            ref int blockIndex,
            long value)
        {
            if (blockIndex != 0)
            {
                throw new ArgumentException(
                    $"Unexpected shortcut character in the middle of a regular block");
            }

            blockIndex = 0; // restart block after the shortcut character
            return writeDecodedValue(ref pOutput, pOutputEnd, value, byteBlockSize);
        }

        private int getSafeCharCountForEncoding(int bytesLength)
        {
            if (bytesLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesLength));
            }

            if (bytesLength == 0)
            {
                return 0;
            }

            return (bytesLength + byteBlockSize - 1) * stringBlockSize / byteBlockSize;
        }

        private int getSafeByteCountForDecoding(int textLength, bool usingShortcuts)
        {
            if (usingShortcuts)
            {
                return textLength * byteBlockSize; // max possible size using shortcuts
            }

            // max possible size without shortcuts
            return (((textLength - 1) / stringBlockSize) + 1) * byteBlockSize;
        }
    }
}