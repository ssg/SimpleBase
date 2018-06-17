using NUnit.Framework;
using SimpleBase;
using System;

namespace SimpleBaseTest
{
    [TestFixture]
    [Parallelizable]
    internal class Base16Test
    {
        private static TestCaseData[] testData = new[]
        {
            new TestCaseData(new byte[] { }, ""),
            new TestCaseData(new byte[] { 0xAB }, "AB"),
            new TestCaseData(new byte[] { 0x00, 0x01, 0x02, 0x03 }, "00010203"),
            new TestCaseData(new byte[] { 0x10, 0x11, 0x12, 0x13 }, "10111213"),
            new TestCaseData(new byte[] { 0xAB, 0xCD, 0xEF, 0xBA }, "ABCDEFBA"),
        };

        [Test]
        [TestCaseSource(nameof(testData))]
        public void EncodeUpper(byte[] input, string expectedOutput)
        {
            var result = Base16.EncodeUpper(input);
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void EncodeLower(byte[] input, string expectedOutput)
        {
            var result = Base16.EncodeLower(input);
            Assert.AreEqual(expectedOutput.ToLowerInvariant(), result);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode(byte[] expectedOutput, string input)
        {
            var result = Base16.Decode(input);
            CollectionAssert.AreEqual(expectedOutput, result);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_LowerCase(byte[] expectedOutput, string input)
        {
            var result = Base16.Decode(input.ToLowerInvariant());
            CollectionAssert.AreEqual(expectedOutput, result);
        }

        [TestCase("AZ12")]
        [TestCase("ZAAA")]
        public void Decode_InvalidChar_Throws(string input)
        {
            Assert.Throws<ArgumentException>(() => Base16.Decode(input));
        }

        [TestCase("12345")]
        [TestCase("ABC")]
        public void Decode_InvalidLength_Throws(string input)
        {
            Assert.Throws<ArgumentException>(() => Base16.Decode(input));
        }
    }
}