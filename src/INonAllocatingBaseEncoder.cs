// <copyright file="INonAllocatingBaseEncoder.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase
{
    /// <summary>
    /// Efficient encoding functionality using pre-allocated memory buffers by the callers.
    /// </summary>
    public interface INonAllocatingBaseEncoder
    {
        /// <summary>
        /// Encode a buffer into a base-encoded representation using pre-allocated buffers.
        /// </summary>
        /// <param name="input">Bytes to encode.</param>
        /// <param name="output">Output buffer.</param>
        /// <param name="numCharsWritten">Actual number of characters written to the output.</param>
        /// <returns>Whether encoding was successful or not. If false, <paramref name="numCharsWritten"/>
        /// will be zero and the content of <paramref name="output"/> will be undefined.</returns>
        bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int numCharsWritten);

        /// <summary>
        /// Decode an encoded character buffer into a pre-allocated output buffer.
        /// </summary>
        /// <param name="input">Encoded text.</param>
        /// <param name="output">Output buffer.</param>
        /// <param name="numBytesWritten">Actual number of bytes written to the output.</param>
        /// <returns>Whether decoding was successful. If false, the value of <paramref name="numBytesWritten"/>
        /// will be zero and the content of <paramref name="output"/> will be undefined.</returns>
        bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int numBytesWritten);
    }
}
