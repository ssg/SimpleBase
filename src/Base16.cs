// <copyright file="Base16.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleBase
{
    /// <summary>
    /// Hexadecimal encoding/decoding.
    /// </summary>
    public sealed class Base16
    {
        private static Lazy<Base16> upperCase = new Lazy<Base16>(() => new Base16(Base16Alphabet.UpperCase));
        private static Lazy<Base16> lowerCase = new Lazy<Base16>(() => new Base16(Base16Alphabet.LowerCase));
        private static Lazy<Base16> modHex = new Lazy<Base16>(() => new Base16(Base16Alphabet.ModHex));

        /// <summary>
        /// Initializes a new instance of the <see cref="Base16"/> class.
        /// </summary>
        /// <param name="alphabet">Alphabet to use.</param>
        public Base16(Base16Alphabet alphabet)
        {
            this.Alphabet = alphabet;
        }

        /// <summary>
        /// Gets upper case Base16 encoder. Decoding is case-insensitive.
        /// </summary>
        public static Base16 UpperCase => upperCase.Value;

        /// <summary>
        /// Gets lower case Base16 encoder. Decoding is case-insensitive.
        /// </summary>
        public static Base16 LowerCase => lowerCase.Value;

        /// <summary>
        /// Gets lower case Base16 encoder. Decoding is case-insensitive.
        /// </summary>
        public static Base16 ModHex => modHex.Value;

        /// <summary>
        /// Gets the alphabet used by the encoder.
        /// </summary>
        public Base16Alphabet Alphabet { get; }

        /// <summary>
        /// Encode to Base16 representation using uppercase lettering.
        /// </summary>
        /// <param name="bytes">Bytes to encode.</param>
        /// <returns>Base16 string.</returns>
        [Obsolete("Deprecated. Use Base16.UpperCase.Encode() instead")]
        public static unsafe string EncodeUpper(ReadOnlySpan<byte> bytes)
        {
            return UpperCase.Encode(bytes);
        }

        /// <summary>
        /// Encode to Base16 representation using lowercase lettering.
        /// </summary>
        /// <param name="bytes">Bytes to encode.</param>
        /// <returns>Base16 string.</returns>
        [Obsolete("Deprecated. Use Base16.LowerCase.Encode() instead")]
        public static unsafe string EncodeLower(ReadOnlySpan<byte> bytes)
        {
            return LowerCase.Encode(bytes);
        }

        /// <summary>
        /// Encodes stream of bytes into a Base16 text.
        /// </summary>
        /// <param name="input">Stream that provides bytes to be encoded.</param>
        /// <param name="output">Stream that the encoded text is written to.</param>
        [Obsolete("Deprecated. Use Base16.UpperCase.Encode() instead")]
        public static void EncodeUpper(Stream input, TextWriter output)
        {
            UpperCase.Encode(input, output);
        }

        /// <summary>
        /// Encodes stream of bytes into a Base16 text.
        /// </summary>
        /// <param name="input">Stream that provides bytes to be encoded.</param>
        /// <param name="output">Stream that the encoded text is written to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Obsolete("Deprecated. Use Base16.UpperCase.EncodeAsync instead")]
        public static Task EncodeUpperAsync(Stream input, TextWriter output)
        {
            return UpperCase.EncodeAsync(input, output);
        }

        /// <summary>
        /// Encodes stream of bytes into a Base16 text.
        /// </summary>
        /// <param name="input">Stream that provides bytes to be encoded.</param>
        /// <param name="output">Stream that the encoded text is written to.</param>
        [Obsolete("Deprecated. Use Base16.LowerCase.Encode() instead")]
        public static void EncodeLower(Stream input, TextWriter output)
        {
            LowerCase.Encode(input, output);
        }

        /// <summary>
        /// Encodes stream of bytes into a Base16 text.
        /// </summary>
        /// <param name="input">Stream that provides bytes to be encoded.</param>
        /// <param name="output">Stream that the encoded text is written to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Obsolete("Deprecated. Use Base16.LowerCase.EncodeLower instead")]
        public static Task EncodeLowerAsync(Stream input, TextWriter output)
        {
            return LowerCase.EncodeAsync(input, output);
        }

        /// <summary>
        /// Decode Base16 text through streams for generic use. Stream based variant tries to consume
        /// as little memory as possible, and relies of .NET's own underlying buffering mechanisms,
        /// contrary to their buffer-based versions.
        /// </summary>
        /// <param name="input">Stream that the encoded bytes would be read from.</param>
        /// <param name="output">Stream where decoded bytes will be written to.</param>
        /// <returns>Task that represents the async operation.</returns>
        public async Task DecodeAsync(TextReader input, Stream output)
        {
            await StreamHelper.DecodeAsync(input, output, buffer => this.Decode(buffer.Span).ToArray())
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Decode Base16 text into bytes.
        /// </summary>
        /// <param name="text">Base16 text.</param>
        /// <returns>Decoded bytes.</returns>
        public unsafe Span<byte> Decode(ReadOnlySpan<char> text)
        {
            int textLen = text.Length;
            if (textLen == 0)
            {
                return Array.Empty<byte>();
            }

            // remainder operator ("%") was unexpectedly slow here
            // that's why we're using "&" below
            if ((textLen & 1) != 0)
            {
                throw new ArgumentException("Text cannot be odd length", nameof(text));
            }

            var table = Alphabet.ReverseLookupTable;

            byte[] output = new byte[textLen >> 1];
            fixed (byte* outputPtr = output)
            fixed (char* textPtr = text)
            {
                byte* pOutput = outputPtr;
                char* pInput = textPtr;
                char* pEnd = pInput + textLen;
                while (pInput != pEnd)
                {
                    int b1 = table[pInput[0]] - 1;
                    if (b1 < 0)
                    {
                        throw new ArgumentException($"Invalid hex character: {pInput[0]}");
                    }

                    int b2 = table[pInput[1]] - 1;
                    if (b2 < 0)
                    {
                        throw new ArgumentException($"Invalid hex character: {pInput[1]}");
                    }

                    *pOutput++ = (byte)(b1 << 4 | b2);
                    pInput += 2;
                }
            }

            return output;
        }

        /// <summary>
        /// Decode Base16 text through streams for generic use. Stream based variant tries to consume
        /// as little memory as possible, and relies of .NET's own underlying buffering mechanisms,
        /// contrary to their buffer-based versions.
        /// </summary>
        /// <param name="input">Stream that the encoded bytes would be read from.</param>
        /// <param name="output">Stream where decoded bytes will be written to.</param>
        public void Decode(TextReader input, Stream output)
        {
            StreamHelper.Decode(input, output, buffer => this.Decode(buffer.Span).ToArray());
        }

        /// <summary>
        /// Encodes stream of bytes into a Base16 text.
        /// </summary>
        /// <param name="input">Stream that provides bytes to be encoded.</param>
        /// <param name="output">Stream that the encoded text is written to.</param>
        public void Encode(Stream input, TextWriter output)
        {
            StreamHelper.Encode(input, output, (buffer, lastBlock) => Encode(buffer.Span));
        }

        /// <summary>
        /// Encodes stream of bytes into a Base16 text.
        /// </summary>
        /// <param name="input">Stream that provides bytes to be encoded.</param>
        /// <param name="output">Stream that the encoded text is written to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task EncodeAsync(Stream input, TextWriter output)
        {
            await StreamHelper.EncodeAsync(input, output, (buffer, lastBlock) =>
                Encode(buffer.Span)).ConfigureAwait(false);
        }

        /// <summary>
        /// Encode to Base16 representation.
        /// </summary>
        /// <param name="bytes">Bytes to encode.</param>
        /// <returns>Base16 string.</returns>
        public unsafe string Encode(ReadOnlySpan<byte> bytes)
        {
            int bytesLen = bytes.Length;
            if (bytesLen == 0)
            {
                return string.Empty;
            }

            string alphabet = Alphabet.Value;

            var output = new string('\0', bytesLen << 1);
            fixed (char* outputPtr = output)
            fixed (byte* bytesPtr = bytes)
            {
                char* pOutput = outputPtr;
                byte* pInput = bytesPtr;

                int octets = bytesLen / sizeof(ulong);
                for (int i = 0; i < octets; i++, pInput += sizeof(ulong))
                {
                    // read bigger chunks
                    ulong input = *(ulong*)pInput;
                    for (int j = 0; j < sizeof(ulong) / 2; j++, input >>= 16)
                    {
                        ushort pair = (ushort)input;

                        // use cpu pipeline to parallelize writes
                        pOutput[0] = alphabet[(pair >> 4) & 0x0F];
                        pOutput[1] = alphabet[pair & 0x0F];
                        pOutput[2] = alphabet[pair >> 12];
                        pOutput[3] = alphabet[(pair >> 8) & 0x0F];
                        pOutput += 4;
                    }
                }

                for (int remaining = bytesLen % sizeof(ulong); remaining > 0; remaining--)
                {
                    byte b = *pInput++;
                    pOutput[0] = alphabet[b >> 4];
                    pOutput[1] = alphabet[b & 0x0F];
                    pOutput += 2;
                }
            }

            return output;
        }
    }
}