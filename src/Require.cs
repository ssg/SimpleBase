using System;
using System.Runtime.CompilerServices;

namespace SimpleBase
{
    static class Require
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void NotNull<T>(T value, string argName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(argName);
            }
        }
    }
}