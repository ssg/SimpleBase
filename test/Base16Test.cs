using NUnit.Framework;
using SimpleBase;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleBaseTest
{
    [TestFixture]
    [Parallelizable]
    internal class Base16Test
    {
        private static readonly TestCaseData[] testData = new[]
        {
            new TestCaseData(new byte[] { }, ""),
            new TestCaseData(new byte[] { 0xAB }, "AB"),
            new TestCaseData(new byte[] { 0x00, 0x01, 0x02, 0x03 }, "00010203"),
            new TestCaseData(new byte[] { 0x10, 0x11, 0x12, 0x13 }, "10111213"),
            new TestCaseData(new byte[] { 0xAB, 0xCD, 0xEF, 0xBA }, "ABCDEFBA"),
        };

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_Stream(byte[] expectedOutput, string input)
        {
            using var memoryStream = new MemoryStream();
            using var reader = new StringReader(input);
            Base16.UpperCase.Decode(reader, memoryStream);
            CollectionAssert.AreEqual(expectedOutput, memoryStream.ToArray());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void EncodeUpper_Stream(byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
            Base16.UpperCase.Encode(inputStream, writer);
            Assert.AreEqual(expectedOutput, writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void EncodeLower_Stream(byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
            Base16.LowerCase.Encode(inputStream, writer);
            Assert.AreEqual(expectedOutput.ToLowerInvariant(), writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public async Task DecodeAsync_Stream(byte[] expectedOutput, string input)
        {
            using var memoryStream = new MemoryStream();
            using var reader = new StringReader(input);
            await Base16.UpperCase.DecodeAsync(reader, memoryStream);
            CollectionAssert.AreEqual(expectedOutput, memoryStream.ToArray());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public async Task EncodeAsync_StreamAsync(byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
            await Base16.UpperCase.EncodeAsync(inputStream, writer);
            Assert.AreEqual(expectedOutput, writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public async Task EncodeAsync_LowerCase_StreamAsync(byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
            await Base16.LowerCase.EncodeAsync(inputStream, writer);
            Assert.AreEqual(expectedOutput.ToLowerInvariant(), writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void EncodeUpper(byte[] input, string expectedOutput)
        {
            var result = Base16.UpperCase.Encode(input);
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void EncodeLower(byte[] input, string expectedOutput)
        {
            var result = Base16.LowerCase.Encode(input);
            Assert.AreEqual(expectedOutput.ToLowerInvariant(), result);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode(byte[] expectedOutput, string input)
        {
            var result = Base16.UpperCase.Decode(input);
            CollectionAssert.AreEqual(expectedOutput, result.ToArray());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_LowerCase(byte[] expectedOutput, string input)
        {
            var result = Base16.LowerCase.Decode(input.ToLowerInvariant());
            CollectionAssert.AreEqual(expectedOutput, result.ToArray());
        }

        [TestCase("AZ12")]
        [TestCase("ZAAA")]
        [TestCase("!AAA")]
        [TestCase("=AAA")]
        public void Decode_InvalidChar_Throws(string input)
        {
            Assert.Throws<ArgumentException>(() => Base16.UpperCase.Decode(input));
        }

        [TestCase("12345")]
        [TestCase("ABC")]
        public void Decode_InvalidLength_Throws(string input)
        {
            Assert.Throws<ArgumentException>(() => Base16.LowerCase.Decode(input));
        }
    }
}