using NUnit.Framework;
using SimpleBase;
using System;

namespace SimpleBaseTest.Base85Test
{
    class Z85Test
    {
        private static readonly TestCaseData[] testVectors = new[]
        {
            new TestCaseData(new byte[] { 0x86, 0x4F, 0xD2, 0x6F, 0xB5, 0x59, 0xF7, 0x5B }, "HelloWorld"),
            new TestCaseData(new byte[] { 0x11, 0x22, 0x33, 0x44 }, "5H620"),
        };

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public void Encode_TestVectors_ShouldEncodeCorrectly(byte[] input, string expectedOutput)
        {
            var result = Base85.Z85.Encode(input);
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        public void Encode_NullBuffer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Base85.Z85.Encode(null));
        }

        [Test]
        public void Decode_NullText_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Base85.Z85.Decode(null));
        }

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public void Decode_TestVectors_ShouldDecodeCorrectly(byte[] expectedOutput, string input)
        {
            var result = Base85.Z85.Decode(input);
            CollectionAssert.AreEqual(expectedOutput, result);
        }
    }
}