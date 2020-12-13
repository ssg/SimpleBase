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
        private const int encodedBlockSize = 7;
        private const int byteBlockSize = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="Base32H"/> class.
        /// </summary>
        /// <param name="alphabet">Encoding alphabet to be used.</param>
        public Base32H(Base32Alphabet alphabet)
        {
            if (alphabet.PaddingPosition != PaddingPosition.Start)
            {
                throw new ArgumentException(
                    "Only alphabets with paddings at the start are supported by this implementation",
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
            if (text.Length % encodedBlockSize != 0)
            {
                throw new ArgumentException($"Length isn't multiple of {encodedBlockSize}", nameof(text));
            }

            int outputLen = text.Length * byteBlockSize / encodedBlockSize;
            var output = new byte[outputLen];

            for (int i = 0; i < text.Length; i += encodedBlockSize)
            {
                var block = text.Slice(i, encodedBlockSize);
            }

            return output;
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