using NUnit.Framework;
using SimpleBase;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleBaseTest.Base16Test
{
    [TestFixture]
    [Parallelizable]
    internal class ModHexTest
    {
        private static readonly TestCaseData[] testData = new[]
        {
            new TestCaseData(new byte[] { }, ""),
            new TestCaseData(new byte[] { 0xAB }, "ln"),
            new TestCaseData(new byte[] { 0x00, 0x01, 0x02, 0x03 }, "cccbcdce"),
            new TestCaseData(new byte[] { 0x10, 0x11, 0x12, 0x13 }, "bcbbbdbe"),
            new TestCaseData(new byte[] { 0xAB, 0xCD, 0xEF, 0xBA }, "lnrtuvnl"),
            new TestCaseData(new byte[] { 0xAB, 0xCD, 0xEF, 0xBA, 0xAB, 0xCD, 0xEF, 0xBA }, "lnrtuvnllnrtuvnl"),
        };

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_Stream(byte[] expectedOutput, string input)
        {
            using var memoryStream = new MemoryStream();
            using var reader = new StringReader(input);
            Base16.ModHex.Decode(reader, memoryStream);
            CollectionAssert.AreEqual(expectedOutput, memoryStream.ToArray());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Encode_Stream(byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
            Base16.ModHex.Encode(inputStream, writer);
            Assert.AreEqual(expectedOutput, writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public async Task DecodeAsync_Stream(byte[] expectedOutput, string input)
        {
            using var memoryStream = new MemoryStream();
            using var reader = new StringReader(input);
            await Base16.ModHex.DecodeAsync(reader, memoryStream);
            CollectionAssert.AreEqual(expectedOutput, memoryStream.ToArray());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public async Task EncodeAsync_StreamAsync(byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
            await Base16.ModHex.EncodeAsync(inputStream, writer);
            Assert.AreEqual(expectedOutput, writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Encode(byte[] input, string expectedOutput)
        {
            var result = Base16.ModHex.Encode(input);
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode(byte[] expectedOutput, string input)
        {
            var result = Base16.ModHex.Decode(input);
            CollectionAssert.AreEqual(expectedOutput, result.ToArray());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_OtherCase_StillPasses(byte[] expectedOutput, string input)
        {
            var result = Base16.ModHex.Decode(input.ToUpperInvariant());
            CollectionAssert.AreEqual(expectedOutput, result.ToArray());
        }

        [TestCase("AZ12")]
        [TestCase("ZAAA")]
        [TestCase("!AAA")]
        [TestCase("=AAA")]
        public void Decode_InvalidChar_Throws(string input)
        {
            Assert.Throws<ArgumentException>(() => Base16.ModHex.Decode(input));
        }

        [TestCase("12345")]
        [TestCase("ABC")]
        public void Decode_InvalidLength_Throws(string input)
        {
            Assert.Throws<ArgumentException>(() => Base16.ModHex.Decode(input));
        }
    }
}