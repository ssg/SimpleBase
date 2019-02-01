/*
     Copyright 2014-2016 Sedat Kapanoglu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using System;
using System.Text;
using SimpleBase;
using NUnit.Framework;
using System.IO;

namespace SimpleBaseTest.Base32Test
{
    [TestFixture]
    class ExtendedHexTest
    {
        private static readonly string[][] testData = new[]
        {
            new[] { "", "" },
            new[] { "f", "CO======" },
            new[] { "fo", "CPNG====" },
            new[] { "foo", "CPNMU===" },
            new[] { "foob", "CPNMUOG=" },
            new[] { "fooba", "CPNMUOJ1" },
            new[] { "foobar", "CPNMUOJ1E8======" },
            new[] { "1234567890123456789012345678901234567890", "64P36D1L6ORJGE9G64P36D1L6ORJGE9G64P36D1L6ORJGE9G64P36D1L6ORJGE9G" },
        };

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Encode_Stream_ReturnsExpectedValues(string input, string expectedOutput)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            using (var inputStream = new MemoryStream(bytes))
            using (var writer = new StringWriter())
            {
                Base32.ExtendedHex.Encode(inputStream, writer, padding: true);
                Assert.AreEqual(expectedOutput, writer.ToString());
            }
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_Stream_ReturnsExpectedValues(string expectedOutput, string input)
        {
            // upper case
            using (var inputStream = new StringReader(input))
            using (var outputStream = new MemoryStream())
            {
                Base32.ExtendedHex.Decode(inputStream, outputStream);
                string result = Encoding.ASCII.GetString(outputStream.ToArray());
                Assert.AreEqual(expectedOutput, result);
            }

            // lower case
            using (var inputStream = new StringReader(input.ToLowerInvariant()))
            using (var outputStream = new MemoryStream())
            {
                Base32.ExtendedHex.Decode(inputStream, outputStream);
                string result = Encoding.ASCII.GetString(outputStream.ToArray());
                Assert.AreEqual(expectedOutput, result);
            }
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Encode_ReturnsExpectedValues(string input, string expectedOutput)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            string result = Base32.ExtendedHex.Encode(bytes, padding: true);
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_ReturnsExpectedValues(string expectedOutput, string input)
        {
            var bytes = Base32.ExtendedHex.Decode(input);
            string result = Encoding.ASCII.GetString(bytes.ToArray());
            Assert.AreEqual(expectedOutput, result);
            bytes = Base32.ExtendedHex.Decode(input.ToLowerInvariant());
            result = Encoding.ASCII.GetString(bytes.ToArray());
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        public void Encode_NullBytes_ReturnsEmptyString()
        {
            Assert.AreEqual(String.Empty, Base32.ExtendedHex.Encode(null, false));
        }

        [Test]
        [TestCase("!@#!#@!#@#!@")]
        [TestCase("||||")]
        public void Decode_InvalidInput_ThrowsArgumentException(string input)
        {
            Assert.Throws<ArgumentException>(() => Base32.ExtendedHex.Decode(input));
        }
    }
}