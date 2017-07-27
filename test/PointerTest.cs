using System;
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest
{
    [TestFixture]
    internal class PointerTest
    {
        [Test]
        public unsafe void OffsetByte_NoOverflow_ReturnsOffset()
        {
            var buffer = new byte[1000];
            fixed (byte* ptr = buffer)
            {
                byte* result = Pointer.Offset(ptr, 1000);
                Assert.IsTrue(result > ptr);
            }
        }

        [Test]
        public unsafe void OffsetChar_NoOverflow_ReturnsOffset()
        {
            var buffer = new char[1000];
            fixed (char* ptr = buffer)
            {
                char* result = Pointer.Offset(ptr, 1000);
                Assert.IsTrue(result > ptr);
            }
        }

        [Test]
        public unsafe void OffsetByte_Overflow_Throws()
        {
            var buffer = new byte[1000];
            fixed (byte* ptr = buffer)
            {
                byte* endPtr = ptr;
                Assert.Throws<InvalidOperationException>(() => Pointer.Offset(endPtr, -1000));
            }
        }

        [Test]
        public unsafe void OffsetChar_Overflow_Throws()
        {
            var buffer = new char[1000];
            fixed (char* ptr = buffer)
            {
                char* endPtr = ptr;
                Assert.Throws<InvalidOperationException>(() => Pointer.Offset(endPtr, -1000));
            }
        }
    }
}