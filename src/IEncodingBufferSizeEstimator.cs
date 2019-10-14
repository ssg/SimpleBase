// <copyright file="IEncodingBufferSizeEstimator.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase
{
    /// <summary>
    /// Functionality to handle computation of encoding and decoding buffer sizes. Because
    /// these calculations might depend on how shortcut characters are used, it is usually
    /// implemented in the alphabet classes.
    /// </summary>
    public interface IEncodingBufferSizeEstimator
    {
        /// <summary>
        /// Gets a safe estimation about how many bytes decoding will take without performing
        /// the actual decoding operation. The estimation can be slightly larger than the actual
        /// output size.
        /// </summary>
        /// <param name="text">Text to be decoded.</param>
        /// <returns>Number of estimated bytes, or zero if the input length is invalid.</returns>
        int GetSafeByteCountForDecoding(ReadOnlySpan<char> text);

        /// <summary>
        /// Gets a safe estimation about how many characters encoding a buffer will take without
        /// performing the actual encoding operation. The estimation can be slightly larger than the
        /// actual output size.
        /// </summary>
        /// <param name="buffer">Bytes to be encoded.</param>
        /// <returns>Number of estimated characters, or zero if the input length is invalid.</returns>
        int GetSafeCharCountForEncoding(ReadOnlySpan<byte> buffer);
    }
}
