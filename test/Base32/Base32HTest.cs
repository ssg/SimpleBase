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
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base32Test
{
    [TestFixture]
    class Base32HTest
    {
        private static readonly object[][] testData = {
            new object[] { "", "" },
            new object[] { "f", "0000036" },
            new object[] { "a", "0000031", true },
            //new object[] { "aq", "CSQG", false },
            //new object[] { "fo", "CSQG====", true },
            //new object[] { "foo", "CSQPY", false },
            //new object[] { "foo", "CSQPY===", true },
            //new object[] { "foob", "CSQPYRG", false },
            //new object[] { "foob", "CSQPYRG=", true },
            //new object[] { "fooba", "CSQPYRK1", false },
            //new object[] { "fooba", "CSQPYRK1", true },
            //new object[] { "foobar", "CSQPYRK1E8", false },
            //new object[] { "foobar", "CSQPYRK1E8======", true },
            //new object[] { "123456789012345678901234567890123456789", "QK4CRL6LV3EE1R60QK4CRL6LV3EE1R60QK4CRL6LV3EE1R60QK4CRL6LV3EE1R", false },
            //new object[] { "123456789012345678901234567890123456789", "64S36D1N6RVKGE9G64S36D1N6RVKGE9G64S36D1N6RVKGE9G64S36D1N6RVKGE8=", true }
        };

        [Test]
        public void Encode_SampleInterface_Compiles()
        {
            // this source code exists in samples and just needs to be compiled and run without errors.
            // do not edit/refactor the code below
            byte[] myBuffer = Array.Empty<byte>();
            string result = Base32.Base32H.Encode(myBuffer);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Decode_SampleInterface_Compiles()
        {
            // this source code exists in samples and just needs to be compiled and run without errors.
            // do not edit/refactor the code below
            string myText = "CSQPYRK1E8"; // any buffer will do
            Span<byte> result = Base32.Base32H.Decode(myText);
            Assert.That(result.Length > 0, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Encode_ReturnsExpectedValues(string input, string expectedOutput)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            string result = Base32.Base32H.Encode(bytes);
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Encode_Simple_ReturnsExpectedValues(string input, string expectedOutput)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            string result = Base32.Base32H.Encode(bytes);
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void TryEncode_Simple_ReturnsExpectedValues(string input, string expectedOutput)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            var output = new char[Base32.Base32H.GetSafeCharCountForEncoding(bytes)];
            bool success = Base32.Base32H.TryEncode(bytes, output, out int numCharsWritten);
            Assert.That(success, Is.True);
            Assert.That(output[..numCharsWritten], Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_ReturnsExpectedValues(string expectedOutput, string input, bool _)
        {
            // upper variant
            var bytes = Base32.Base32H.Decode(input);
            string result = Encoding.ASCII.GetString(bytes);
            Assert.That(result, Is.EqualTo(expectedOutput));

            // lower variant
            bytes = Base32.Base32H.Decode(input.ToLowerInvariant());
            result = Encoding.ASCII.GetString(bytes);
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void TryDecode_ReturnsExpectedValues(string expectedOutput, string input, bool _)
        {
            var output = new byte[Base32.Base32H.GetSafeByteCountForDecoding(input)];
            var success = Base32.Base32H.TryDecode(input, output, out int numBytesWritten);
            Assert.That(success, Is.True);
            string result = Encoding.ASCII.GetString(output[..numBytesWritten]);
            Assert.That(result, Is.EqualTo(expectedOutput));

            success = Base32.Base32H.TryDecode(input.ToLowerInvariant(), output, out numBytesWritten);
            Assert.That(success, Is.True);
            result = Encoding.ASCII.GetString(output[..numBytesWritten]);
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        public void TryDecode_ZeroBuffer_ReturnsFalse()
        {
            var success = Base32.Base32H.TryDecode("test", Array.Empty<byte>(), out int numBytesWritten);
            Assert.That(success, Is.False);
            Assert.That(numBytesWritten, Is.EqualTo(0));
        }

        [Test]
        public void Decode_InvalidInput_ThrowsArgumentException()
        {
            _ = Assert.Throws<ArgumentException>(() => Base32.Base32H.Decode("[];',m."));
        }

        [Test]
        [TestCase("O0o", "000")]
        [TestCase("Ii1", "111")]
        [TestCase("Ss5", "555")]
        [TestCase("UuV", "VVV")]
        public void Decode_Base32HChars_DecodedCorrectly(string equivalent, string actual)
        {
            var expectedResult = Base32.Base32H.Decode(actual);
            var result = Base32.Base32H.Decode(equivalent);
            Assert.That(result.ToArray(), Is.EqualTo(expectedResult.ToArray()));
        }

        [Test]
        public void Encode_NullBytes_ReturnsEmptyString()
        {
            Assert.That(Base32.Base32H.Encode(null), Is.EqualTo(String.Empty));
        }
    }
}