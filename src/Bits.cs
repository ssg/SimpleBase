// <copyright file="Bits.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Runtime.CompilerServices;

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
        /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/unsafe-code/best-practices#:~:text=For%20example%2C%201024%20bytes%20could%20be%20considered%20a%20reasonable%20upper%20bound." />
        internal const int SafeStackMaxAllocSize = 1024;

        /// <summary>
        /// Max decimal digits possible in an unsigned long (64-bit) number.
        /// </summary>
        internal const int MaxUInt64Digits = 20;

        /// <summary>
        /// Converts a variable length byte array to a 64-bit unsigned integer.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>Unsigned integer representation of the bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        /// <summary>
        /// Count the number of consecutive zero bytes at the beginning of the given buffer.
        /// </summary>
        /// <param name="bytes">Buffer for prefixing zeroes to be counted.</param>
        /// <returns>Number of zeroes at the beginning of the buffer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CountPrefixingZeroes(ReadOnlySpan<byte> bytes)
        {
            int i = 0;
            for (; i < bytes.Length && bytes[i] == 0; i++)               
            {
            }
            return i;
        }
    }
}
