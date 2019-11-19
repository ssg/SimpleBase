﻿using NUnit.Framework;
using SimpleBase;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleBaseTest.Base85Test
{
    internal class Ascii85Test
    {
        private static readonly object[][] testVectors = new object[][]
        {
            new object[] { new byte[] { }, "" },
            new object[] { new byte[] { 0, 0, 0, 0 }, "z" },
            new object[] { new byte[] { 0x20, 0x20, 0x20, 0x20 }, "y" },
            new object[] { new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 }, "5sdq,70" },
            new object[] { new byte[] { 0x86, 0x4F, 0xD2, 0x6F, 0xB5, 0x59, 0xF7, 0x5B }, "L/669[9<6." },
            new object[] { new byte[] { 0x11, 0x22, 0x33 }, "&L'\"" },
            new object[] { new byte[] { 77, 97, 110, 32 }, "9jqo^" },
        };

        [Test]
        public void Decode_InvalidShortcut_ThrowsArgumentException()
        {
            const string input = "9zjqo";
            Assert.Throws<ArgumentException>(() => Base85.Ascii85.Decode(input));
        }

        [Test]
        public void Decode_InvalidCharacter_ThrowsArgumentException()
        {
            const string input = "~!@#()(";
            Assert.Throws<ArgumentException>(() => Base85.Ascii85.Decode(input));
        }

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public void Decode_Whitespace_IsIgnored(byte[] expectedOutput, string input)
        {
            string actualInput = String.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                actualInput += "  " + input[i];
            }
            actualInput += " ";
            var result = Base85.Ascii85.Decode(actualInput);
            CollectionAssert.AreEqual(expectedOutput, result.ToArray());
        }

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public void Encode_TestVectorsOnStream_ShouldEncodeCorrectly(byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
            Base85.Ascii85.Encode(inputStream, writer);
            Assert.AreEqual(expectedOutput, writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public async Task EncodeAsync_TestVectorsOnStream_ShouldEncodeCorrectly(byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
            await Base85.Ascii85.EncodeAsync(inputStream, writer);
            Assert.AreEqual(expectedOutput, writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public void Encode_TestVectors_ShouldEncodeCorrectly(byte[] input, string expectedOutput)
        {
            var result = Base85.Ascii85.Encode(input);
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public void TryEncode_TestVectors_ShouldEncodeCorrectly(byte[] input, string expectedOutput)
        {
            var output = new char[Base85.Ascii85.GetSafeCharCountForEncoding(input)];
            Assert.IsTrue(Base85.Ascii85.TryEncode(input, output, out int numCharsWritten));
            Assert.AreEqual(expectedOutput, new string(output[..numCharsWritten]));
        }

        [Test]
        public void Encode_UnevenBuffer_DoesNotThrowArgumentException()
        {
            Assert.DoesNotThrow(() => Base85.Ascii85.Encode(new byte[3]));
        }

        [Test]
        public void Encode_NullBuffer_ReturnsEmptyString()
        {
            Assert.AreEqual(String.Empty, Base85.Ascii85.Encode(null));
        }

        [Test]
        public void Decode_UnevenText_DoesNotThrowArgumentException()
        {
            Assert.DoesNotThrow(() => Base85.Ascii85.Decode("hebe"));
        }

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public void Decode_TestVectorsWithStream_ShouldDecodeCorrectly(byte[] expectedOutput, string input)
        {
            using var inputStream = new StringReader(input);
            using var writer = new MemoryStream();
            Base85.Ascii85.Decode(inputStream, writer);
            CollectionAssert.AreEqual(expectedOutput, writer.ToArray());
        }

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public async Task DecodeAsync_TestVectorsWithStream_ShouldDecodeCorrectly(byte[] expectedOutput, string input)
        {
            using var inputStream = new StringReader(input);
            using var writer = new MemoryStream();
            await Base85.Ascii85.DecodeAsync(inputStream, writer);
            CollectionAssert.AreEqual(expectedOutput, writer.ToArray());
        }

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public void Decode_TestVectors_ShouldDecodeCorrectly(byte[] expectedOutput, string input)
        {
            var result = Base85.Ascii85.Decode(input);
            CollectionAssert.AreEqual(expectedOutput, result.ToArray());
        }
    }
}