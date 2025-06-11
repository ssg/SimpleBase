// <copyright file="Bits.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase
{
    /// <summary>
    /// Helper functions for bit operations.
    /// </summary>
    static class Bits
    {
        /// <summary>
        /// Safe one-shot maximum amount to be allocated on stack for temporary buffers and alike.
        /// </summary>
        internal const int SafeStackMaxAllocSize = 1024;

        /// <summary>
        /// Max decimal digits possible in an unsigned long (64-bit) number.
        /// </summary>
        internal const int MaxUInt64Digits = 20;

        /// <summary>
        /// Converts a byte array to a hexadecimal string.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static ulong PartialBigEndianBytesToUInt64(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length > sizeof(ulong))
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "Span too long to convert to UInt64");
            }

            ulong result = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                result = (result << 8) | bytes[i];
            }
            return result;
        }
    }
}
