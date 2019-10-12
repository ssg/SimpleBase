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
using System.IO;
using System.Text;
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base32Test
{
    [TestFixture]
    class CrockfordTest
    {
        private static readonly string[][] testData = new[]
        {
            new[] { "", "" },
            new[] { "f", "CR" },
            new[] { "fo", "CSQG" },
            new[] { "foo", "CSQPY" },
            new[] { "foob", "CSQPYRG" },
            new[] { "fooba", "CSQPYRK1" },
            new[] { "foobar", "CSQPYRK1E8" },
            new[] { "1234567890123456789012345678901234567890", "64S36D1N6RVKGE9G64S36D1N6RVKGE9G64S36D1N6RVKGE9G64S36D1N6RVKGE9G" },
        };

        [Test]
        public void Encode_SampleInterface_Compiles()
        {
            // this source code exists in samples and just needs to be compiled and run without errors.
            // do not edit/refactor the code below
            byte[] myBuffer = new byte[0];
            string result = Base32.Crockford.Encode(myBuffer, padding: true);
            Assert.IsEmpty(result);
        }

        [Test]
        public void Decode_SampleInterface_Compiles()
        {
            // this source code exists in samples and just needs to be compiled and run without errors.
            // do not edit/refactor the code below
            string myText = "CSQPYRK1E8"; // any buffer will do
            Span<byte> result = Base32.Crockford.Decode(myText);
            Assert.IsTrue(result.Length > 0);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Encode_Stream_ReturnsExpectedValues(string input, string expectedOutput)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            using var inputStream = new MemoryStream(bytes);
            using var writer = new StringWriter();
            Base32.Crockford.Encode(inputStream, writer, padding: false);
            Assert.AreEqual(expectedOutput, writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_Stream_ReturnsExpectedValues(string expectedOutput, string input)
        {
            // upper case
            using (var inputStream = new StringReader(input))
            using (var outputStream = new MemoryStream())
            {
                Base32.Crockford.Decode(inputStream, outputStream);
                string result = Encoding.ASCII.GetString(outputStream.ToArray());
                Assert.AreEqual(expectedOutput, result);
            }

            // lower case
            using (var inputStream = new StringReader(input.ToLowerInvariant()))
            using (var outputStream = new MemoryStream())
            {
                Base32.Crockford.Decode(inputStream, outputStream);
                string result = Encoding.ASCII.GetString(outputStream.ToArray());
                Assert.AreEqual(expectedOutput, result);
            }
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Encode_ReturnsExpectedValues(string input, string expectedOutput)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            string result = Base32.Crockford.Encode(bytes, padding: false);
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_ReturnsExpectedValues(string expectedOutput, string input)
        {
            var bytes = Base32.Crockford.Decode(input);
            string result = Encoding.ASCII.GetString(bytes.ToArray());
            Assert.AreEqual(expectedOutput, result);
            bytes = Base32.Crockford.Decode(input.ToLowerInvariant());
            result = Encoding.ASCII.GetString(bytes.ToArray());
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        public void Decode_InvalidInput_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Base32.Crockford.Decode("[];',m."));
        }

        [Test]
        [TestCase("O0o", "000")]
        [TestCase("Ll1", "111")]
        [TestCase("I1i", "111")]
        public void Decode_CrockfordChars_DecodedCorrectly(string equivalent, string actual)
        {
            var expectedResult = Base32.Crockford.Decode(actual);
            var result = Base32.Crockford.Decode(equivalent);
            Assert.AreEqual(expectedResult.ToArray(), result.ToArray());
        }

        [Test]
        public void Encode_NullBytes_ReturnsEmptyString()
        {
            Assert.AreEqual(String.Empty, Base32.Crockford.Encode(null, true));
        }
    }
}