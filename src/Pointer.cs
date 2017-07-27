using System;
using System.Runtime.CompilerServices;

namespace SimpleBase
{
    internal static class Pointer
    {
#if !DEBUG
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe byte* Offset(byte* ptr, int length)
        {
            byte* endPtr = ptr + length;
            if (length < 0 || endPtr < ptr)
            {
                throw new InvalidOperationException("Buffer overflow -- buffer too large?");
            }
            return endPtr;
        }

#if !DEBUG
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe char* Offset(char* ptr, int length)
        {
            char* endPtr = ptr + length;
            if (length < 0 || endPtr < ptr)
            {
                throw new InvalidOperationException("Buffer overflow -- buffer too large?");
            }
            return endPtr;
        }
    }
}