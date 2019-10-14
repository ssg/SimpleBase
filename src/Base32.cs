// <copyright file="Base32.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleBase
{
    /// <summary>
    /// Base32 encoding/decoding functions.
    /// </summary>
    public sealed class Base32 : IBaseEncoder, IBaseStreamEncoder
    {
        private const int bitsPerByte = 8;
        private const int bitsPerChar = 5;

        private static Lazy<Base32> crockford = new Lazy<Base32>(() => new Base32(Base32Alphabet.Crockford));
        private static Lazy<Base32> rfc4648 = new Lazy<Base32>(() => new Base32(Base32Alphabet.Rfc4648));
        private static Lazy<Base32> extendedHex = new Lazy<Base32>(() => new Base32(Base32Alphabet.ExtendedHex));
        private static Lazy<Base32> zBase32 = new Lazy<Base32>(() => new Base32(Base32Alphabet.ZBase32));
        private static Lazy<Base32> geohash = new Lazy<Base32>(() => new Base32(Base32Alphabet.Geohash));

        /// <summary>
        /// Initializes a new instance of the <see cref="Base32"/> class with a
        /// custom alphabet.
        /// </summary>
        /// <param name="alphabet">Alphabet to use.</param>
        public Base32(Base32Alphabet alphabet)
        {
            Alphabet = alphabet;
        }

        /// <summary>
        /// Gets Douglas Crockford's Base32 flavor with substitution characters.
        /// </summary>
        public static Base32 Crockford => crockford.Value;

        /// <summary>
        /// Gets RFC 4648 variant of Base32 coder.
        /// </summary>
        public static Base32 Rfc4648 => rfc4648.Value;

        /// <summary>
        /// Gets Extended Hex variant of Base32 coder.
        /// </summary>
        /// <remarks>Also from RFC 4648.</remarks>
        public static Base32 ExtendedHex => extendedHex.Value;

        /// <summary>
        /// Gets z-base-32 variant of Base32 coder.
        /// </summary>
        /// <remarks>This variant is used in Mnet, ZRTP and Tahoe-LAFS.</remarks>
        public static Base32 ZBase32 => zBase32.Value;

        /// <summary>
        /// Gets Geohash variant of Base32 coder.
        /// </summary>
        public static Base32 Geohash => geohash.Value;

        /// <summary>
        /// Gets the encoding alphabet.
        /// </summary>
        public Base32Alphabet Alphabet { get; }

        /// <summary>
        /// Encode a byte array into a Base32 string without padding.
        /// </summary>
        /// <param name="bytes">Buffer to be encoded.</param>
        /// <returns>Encoded string.</returns>
        public string Encode(ReadOnlySpan<byte> bytes)
        {
            return Encode(bytes, padding: false);
        }

        /// <summary>
        /// Encode a byte array into a Base32 string.
        /// </summary>
        /// <param name="bytes">Buffer to be encoded.</param>
        /// <param name="padding">Append padding characters in the output.</param>
        /// <returns>Encoded string.</returns>
        public unsafe string Encode(ReadOnlySpan<byte> bytes, bool padding)
        {
            int bytesLen = bytes.Length;
            if (bytesLen == 0)
            {
                return string.Empty;
            }

            // we are ok with slightly larger buffer since the output string will always
            // have the exact length of the output produced.
            int outputLen = Alphabet.GetSafeCharCountForEncoding(bytes);
            string output = new string('\0', outputLen);

            fixed (byte* inputPtr = bytes)
            fixed (char* outputPtr = output)
            fixed (char* tablePtr = this.Alphabet.Value)
            {
                char* pOutput = outputPtr;
                byte* pInput = inputPtr;
                byte* pEnd = pInput + bytesLen;

                for (int bitsLeft = bitsPerByte, currentByte = *pInput, outputPad; pInput != pEnd;)
                {
                    if (bitsLeft > bitsPerChar)
                    {
                        bitsLeft -= bitsPerChar;
                        outputPad = currentByte >> bitsLeft;
                        *pOutput++ = tablePtr[outputPad];
                        currentByte &= (1 << bitsLeft) - 1;
                    }

                    int nextBits = bitsPerChar - bitsLeft;
                    bitsLeft = bitsPerByte - nextBits;
                    outputPad = currentByte << nextBits;
                    if (++pInput != pEnd)
                    {
                        currentByte = *pInput;
                        outputPad |= currentByte >> bitsLeft;
                        currentByte &= (1 << bitsLeft) - 1;
                    }

                    *pOutput++ = tablePtr[outputPad];
                }

                if (padding)
                {
                    char paddingChar = Alphabet.PaddingChar;

                    for (char* pOutputEnd = outputPtr + outputLen; pOutput != pOutputEnd; pOutput++)
                    {
                        *pOutput = paddingChar;
                    }
                }

                int finalOutputLen = (int)(pOutput - outputPtr);
                if (finalOutputLen == outputLen)
                {
                    return output; // avoid unnecessary copying
                }

                return new string(outputPtr, 0, finalOutputLen);
            }
        }

        /// <summary>
        /// Decode a Base32 encoded string into a byte array.
        /// </summary>
        /// <param name="text">Encoded Base32 characters.</param>
        /// <returns>Decoded byte array.</returns>
        public Span<byte> Decode(string text)
        {
            return Decode(text.AsSpan());
        }

        /// <summary>
        /// Decode a Base32 encoded string into a byte array.
        /// </summary>
        /// <param name="text">Encoded Base32 string.</param>
        /// <returns>Decoded byte array.</returns>
        public unsafe Span<byte> Decode(ReadOnlySpan<char> text)
        {
            int textLen = text.Length;

            int bitsLeft = bitsPerByte;
            textLen -= Alphabet.GetPaddingCharCount(text);
            int outputLen = Base32Alphabet.GetAllocationByteCountForDecoding(textLen);
            if (outputLen == 0)
            {
                return Array.Empty<byte>();
            }

            var outputBuffer = new byte[outputLen];
            int outputPad = 0;
            var table = this.Alphabet.ReverseLookupTable;

            fixed (byte* outputPtr = outputBuffer)
            fixed (char* inputPtr = text)
            {
                byte* pOutput = outputPtr;
                char* pInput = inputPtr;
                char* pEnd = inputPtr + textLen;
                while (pInput != pEnd)
                {
                    char c = *pInput++;
                    int b = table[c] - 1;
                    if (b < 0)
                    {
                        throw EncodingAlphabet.InvalidCharacter(c);
                    }

                    if (bitsLeft > bitsPerChar)
                    {
                        bitsLeft -= bitsPerChar;
                        outputPad |= b << bitsLeft;
                        continue;
                    }

                    int shiftBits = bitsPerChar - bitsLeft;
                    outputPad |= b >> shiftBits;
                    *pOutput++ = (byte)outputPad;
                    b &= (1 << shiftBits) - 1;
                    bitsLeft = bitsPerByte - shiftBits;
                    outputPad = b << bitsLeft;
                }
            }

            return outputBuffer;
        }

        /// <summary>
        /// Encode a binary stream to a Base32 text stream without padding.
        /// </summary>
        /// <param name="input">Input bytes.</param>
        /// <param name="output">The writer the output is written to.</param>
        public void Encode(Stream input, TextWriter output)
        {
            Encode(input, output, padding: false);
        }

        /// <summary>
        /// Encode a binary stream to a Base32 text stream.
        /// </summary>
        /// <param name="input">Input bytes.</param>
        /// <param name="output">The writer the output is written to.</param>
        /// <param name="padding">Whether to use padding at the end of the output.</param>
        public void Encode(Stream input, TextWriter output, bool padding)
        {
            StreamHelper.Encode(input, output, (buffer, lastBlock) =>
            {
                bool usePadding = lastBlock ? padding : false;
                return Encode(buffer.Span, usePadding);
            });
        }

        /// <summary>
        /// Encode a binary stream to a Base32 text stream without padding.
        /// </summary>
        /// <param name="input">Input bytes.</param>
        /// <param name="output">The writer the output is written to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task EncodeAsync(Stream input, TextWriter output)
        {
            return EncodeAsync(input, output, padding: false);
        }

        /// <summary>
        /// Encode a binary stream to a Base32 text stream.
        /// </summary>
        /// <param name="input">Input bytes.</param>
        /// <param name="output">The writer the output is written to.</param>
        /// <param name="padding">Whether to use padding at the end of the output.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task EncodeAsync(Stream input, TextWriter output, bool padding)
        {
            await StreamHelper.EncodeAsync(input, output, (buffer, lastBlock) =>
            {
                bool usePadding = lastBlock ? padding : false;
                return Encode(buffer.Span, usePadding);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Decode a text stream into a binary stream.
        /// </summary>
        /// <param name="input">TextReader open on the stream.</param>
        /// <param name="output">Binary output stream.</param>
        public void Decode(TextReader input, Stream output)
        {
            StreamHelper.Decode(input, output, buffer => Decode(buffer.Span).ToArray());
        }

        /// <summary>
        /// Decode a text stream into a binary stream.
        /// </summary>
        /// <param name="input">TextReader open on the stream.</param>
        /// <param name="output">Binary output stream.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DecodeAsync(TextReader input, Stream output)
        {
            await StreamHelper.DecodeAsync(input, output, buffer => Decode(buffer.Span).ToArray())
                .ConfigureAwait(false);
        }
    }
}