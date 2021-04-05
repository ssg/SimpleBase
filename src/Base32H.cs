// <copyright file="Base32H.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase
{
    /// <summary>
    /// Coding class for Base32H encoding scheme.
    /// </summary>
    /// <remarks>
    /// We implement a separate class for Base32H for several reasons:
    /// - Its implementation is significantly different than other Base32 flavors. For example, the padding
    ///   is at the beginning of the buffer instead of the end and works with the reverse endianness.
    /// - .NET 5.0 brought in many perf improvements for Span(T) operations, this can act as a testbed for
    ///   those operations.
    /// </remarks>
    public class Base32H : IBaseCoder, INonAllocatingBaseCoder,
        INumericBaseCoder
    {
        private const int textBlockSize = 8;
        private const int byteBlockSize = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="Base32H"/> class.
        /// </summary>
        /// <param name="alphabet">Encoding alphabet to be used.</param>
        public Base32H(Base32Alphabet alphabet)
        {
            if (alphabet.PaddingPosition != PaddingPosition.Start)
            {
                throw new ArgumentException(
                    "Only alphabets with paddings at the start are supported by Base32H",
                    nameof(alphabet));
            }

            Alphabet = alphabet;
        }

        /// <summary>
        /// Gets the coding alphabet.
        /// </summary>
        public Base32Alphabet Alphabet { get; }

        /// <inheritdoc/>
        public Span<byte> Decode(ReadOnlySpan<char> text)
        {
            int outputLen = text.Length * byteBlockSize / textBlockSize;
            var output = new byte[outputLen].AsSpan();
            var inputBytes = new byte[textBlockSize].AsSpan();
            if (outputLen == 0)
            {
                return output;
            }

            int inputBlockSize = Math.Min(text.Length, textBlockSize);
            var table = Alphabet.ReverseLookupTable;
            char padChar = Alphabet.PaddingChar;

            for (int i = 0, j = 0; i < text.Length; i += inputBlockSize, j += byteBlockSize)
            {
                var inputBlock = text.Slice(i, inputBlockSize);
                if (inputBlockSize < textBlockSize)
                {
                    inputBlock = createPaddedBlock(inputBlock, inputBlockSize, padChar);
                    inputBlockSize = textBlockSize;
                }

                var outputBlock = output.Slice(j, byteBlockSize);
                int remainingBits = 8;
                for (int n = 0; n < textBlockSize; n++)
                {
                    byte b = table[inputBlock[n]];
                    if (b == 0)
                    {
                        throw new ArgumentException($"Invalid character: {inputBlock[n]}");
                    }

                    b = (byte)(b - 1); // normalize the value
                    byte outputByte;
                    while (remainingBits > 0)
                    {
                        int bitsToMove = Math.Min(remainingBits, 5);
                        int mask = (1 << bitsToMove) - 1;
                        outputByte |= 
                    }
                }
            }

            return output;
        }

        private static ReadOnlySpan<char> createPaddedBlock(
            ReadOnlySpan<char> inputBlock,
            int inputBlockSize,
            char padChar)
        {
            var buffer = new char[textBlockSize].AsSpan();
            int padLength = textBlockSize - inputBlockSize;
            buffer.Slice(0, padLength).Fill(padChar);
            inputBlock.CopyTo(buffer[padLength..]);
            return buffer;
        }

        /// <inheritdoc/>
        public long DecodeInt64(string text)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ulong DecodeUInt64(string text)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string Encode(ReadOnlySpan<byte> bytes)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string Encode(long number)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string Encode(ulong number)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int numBytesWritten)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int numCharsWritten)
        {
            throw new NotImplementedException();
        }
    }
}