using NUnit.Framework;
using SimpleBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBaseTest.Base16Test
{
    [TestFixture]
    [Parallelizable]
    internal class Base16Test
    {
        private static readonly Base16[] encoders = new[]
        {
            Base16.LowerCase,
            Base16.UpperCase,
            Base16.ModHex
        };

        private static readonly object[][] testCases = new[]
        {                                                                                   // LowerCase        // UpperCase        // ModHex
            new object[] { new byte[] { },                                                  "",                 "",                 ""                  },
            new object[] { new byte[] { 0xAB },                                             "ab",               "AB",               "ln"                },
            new object[] { new byte[] { 0x00, 0x01, 0x02, 0x03 },                           "00010203",         "00010203",         "cccbcdce"          },
            new object[] { new byte[] { 0x10, 0x11, 0x12, 0x13 },                           "10111213",         "10111213",         "bcbbbdbe"          },
            new object[] { new byte[] { 0xAB, 0xCD, 0xEF, 0xBA },                           "abcdefba",         "ABCDEFBA",         "lnrtuvnl"          },
            new object[] { new byte[] { 0xAB, 0xCD, 0xEF, 0xBA, 0xAB, 0xCD, 0xEF, 0xBA },   "abcdefbaabcdefba", "ABCDEFBAABCDEFBA", "lnrtuvnllnrtuvnl"  },
        };

        private static IEnumerable<TestCaseData> testData
        {
            get
            {
                foreach (var pair in encoders.Select((encoder, index) => (encoder, index)))
                    foreach (var testRow in testCases)
                    {
                        var testValue = testRow[pair.index + 1];
                        yield return new TestCaseData(pair.encoder, testRow[0], testValue)
                            .SetName($"{pair.encoder.Alphabet}_{testValue}");
                    }
            }
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_Stream(Base16 encoder, byte[] expectedOutput, string input)
        {
            using var memoryStream = new MemoryStream();
            using var reader = new StringReader(input);
            encoder.Decode(reader, memoryStream);
            CollectionAssert.AreEqual(expectedOutput, memoryStream.ToArray());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Encode_Stream(Base16 encoder, byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
            encoder.Encode(inputStream, writer);
            Assert.AreEqual(expectedOutput, writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public async Task DecodeAsync_Stream(Base16 encoder, byte[] expectedOutput, string input)
        {
            using var memoryStream = new MemoryStream();
            using var reader = new StringReader(input);
            await encoder.DecodeAsync(reader, memoryStream);
            CollectionAssert.AreEqual(expectedOutput, memoryStream.ToArray());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public async Task EncodeAsync_StreamAsync(Base16 encoder, byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
            await encoder.EncodeAsync(inputStream, writer);
            Assert.AreEqual(expectedOutput, writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Encode(Base16 encoder, byte[] input, string expectedOutput)
        {
            var result = encoder.Encode(input);
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void TryEncode_RegularInput_Succeeds(Base16 encoder, byte[] input, string expectedOutput)
        {
            var output = new char[input.Length * 2];
            Assert.IsTrue(encoder.TryEncode(input, output, out int numCharsWritten));
            Assert.AreEqual(output.Length, numCharsWritten);
            Assert.AreEqual(expectedOutput, new string(output));
        }

        [Test]
        [TestCaseSource(nameof(encoders))]
        public void TryEncode_SmallerOutput_Fails(Base16 encoder)
        {
            var input = new byte[4];
            var output = new char[0];
            Assert.IsFalse(encoder.TryEncode(input, output, out int numCharsWritten));
            Assert.AreEqual(0, numCharsWritten);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode(Base16 encoder, byte[] expectedOutput, string input)
        {
            var result = encoder.Decode(input);
            CollectionAssert.AreEqual(expectedOutput, result.ToArray());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void TryDecode_RegularInput_Succeeds(Base16 encoder, byte[] expectedOutput, string input)
        {
            var output = new byte[expectedOutput.Length];
            Assert.IsTrue(encoder.TryDecode(input, output, out int numBytesWritten));
            Assert.AreEqual(output.Length, numBytesWritten);
            CollectionAssert.AreEqual(expectedOutput, output);
        }

        [Test]
        [TestCaseSource(nameof(encoders))]
        public void TryDecode_SmallOutputBuffer_Fails(Base16 encoder)
        {
            var input = "1234";
            var output = new byte[1];
            Assert.IsFalse(encoder.TryDecode(input, output, out int numBytesWritten));
            Assert.AreEqual(0, numBytesWritten);
        }

        [Test]
        [TestCaseSource(nameof(encoders))]
        public void TryDecode_UnevenInputBuffer_Fails(Base16 encoder)
        {
            var input = "123";
            var output = new byte[1];
            Assert.IsFalse(encoder.TryDecode(input, output, out int numBytesWritten));
            Assert.AreEqual(0, numBytesWritten);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_OtherCase_StillPasses(Base16 encoder, byte[] expectedOutput, string input)
        {
            var result = encoder.Decode(input.ToUpperInvariant());
            CollectionAssert.AreEqual(expectedOutput, result.ToArray());
        }

        [Test]
        public void Decode_InvalidChar_Throws(
            [ValueSource(nameof(encoders))]Base16 encoder,
            [Values("AZ12", "ZAAA", "!AAA", "=AAA")]string input)
        {            
            Assert.Throws<ArgumentException>(() => encoder.Decode(input));
        }

        [Test]
        public void Decode_InvalidLength_Throws(
            [ValueSource(nameof(encoders))]Base16 encoder,
            [Values("123", "12345")]string input)
        {
            Assert.Throws<ArgumentException>(() => encoder.Decode(input));
        }

        [Test]
        [TestCaseSource(nameof(encoders))]
        public void GetSafeCharCountForEncoding_ReturnsCorrectValue(Base16 encoder)
        {
            var input = new byte[5];
            Assert.AreEqual(10, encoder.GetSafeCharCountForEncoding(input));
        }

        [Test]
        [TestCaseSource(nameof(encoders))]
        public void GetSafeByteCountForDecoding_ReturnsCorrectValues(Base16 encoder)
        {
            var input = new char[10];
            Assert.AreEqual(5, encoder.GetSafeByteCountForDecoding(input));
        }

        [Test]
        [TestCaseSource(nameof(encoders))]
        public void GetSafeByteCountForDecoding_InvalidBufferSize_ReturnsZero(Base16 encoder)
        {
            var input = new char[11];
            Assert.AreEqual(0, encoder.GetSafeByteCountForDecoding(input));
        }

        [Test]
        public void CustomCtor()
        {
            var encoder = new Base16(new Base16Alphabet("abcdefghijklmnop"));
            var result = encoder.Encode(new byte[] { 0, 1, 16, 128, 255 });
            Assert.AreEqual("aaabbaiapp", result);
        }

        [Test]
        public void ToString_ReturnsNameWithAlphabet([ValueSource(nameof(encoders))]Base16 encoder)
        {
            Assert.AreEqual($"Base16_{encoder.Alphabet}", encoder.ToString());
        }

        [Test]
        public void GetHashCode_ReturnsAlphabetHashCode([ValueSource(nameof(encoders))]Base16 encoder)
        {
            Assert.AreEqual(encoder.Alphabet.GetHashCode(), encoder.GetHashCode());
        }
    }
}