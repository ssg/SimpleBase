using System;
using System.Collections.Generic;
using System.Text;

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
        /// Converts a byte array to a hexadecimal string.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static ulong BigEndianBytesToUInt64(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length > sizeof(ulong))
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "Byte array too long to convert to UInt64");
            }

            ulong result = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                result = (result << 8) | bytes[i];
            }
            return result;
        }

        internal static void UInt64ToBigEndianBytes(ulong value, Span<byte> output)
        {
            if (output.Length < sizeof(ulong))
            {
                throw new ArgumentException("Output is too small", nameof(output));
            }
            int byteCount = sizeof(ulong);
            for (int i = byteCount - 1; i >= 0; i--)
            {
                output[i] = (byte)(value & 0xFF);
                value >>= 8;
            }
        }
    }
}
