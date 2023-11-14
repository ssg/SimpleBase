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

namespace SimpleBaseTest.Base32Test;

[TestFixture]
class ExtendedHexTest
{
    private static readonly string[][] testData =
    [
        ["", ""],
        ["f", "CO======"],
        ["fo", "CPNG===="],
        ["foo", "CPNMU==="],
        ["foob", "CPNMUOG="],
        ["fooba", "CPNMUOJ1"],
        ["foobar", "CPNMUOJ1E8======"],
        ["1234567890123456789012345678901234567890", "64P36D1L6ORJGE9G64P36D1L6ORJGE9G64P36D1L6ORJGE9G64P36D1L6ORJGE9G"],
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_Stream_ReturnsExpectedValues(string input, string expectedOutput)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using var inputStream = new MemoryStream(bytes);
        using var writer = new StringWriter();
        Base32.ExtendedHex.Encode(inputStream, writer, padding: true);
        Assert.That(writer.ToString(), Is.EqualTo(expectedOutput));
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
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        // lower case
        using (var inputStream = new StringReader(input.ToLowerInvariant()))
        using (var outputStream = new MemoryStream())
        {
            Base32.ExtendedHex.Decode(inputStream, outputStream);
            string result = Encoding.ASCII.GetString(outputStream.ToArray());
            Assert.That(result, Is.EqualTo(expectedOutput));
        }
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_ReturnsExpectedValues(string input, string expectedOutput)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        string result = Base32.ExtendedHex.Encode(bytes, padding: true);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_ReturnsExpectedValues(string expectedOutput, string input)
    {
        var bytes = Base32.ExtendedHex.Decode(input);
        string result = Encoding.ASCII.GetString(bytes);
        Assert.That(result, Is.EqualTo(expectedOutput));
        bytes = Base32.ExtendedHex.Decode(input.ToLowerInvariant());
        result = Encoding.ASCII.GetString(bytes);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Encode_NullBytes_ReturnsEmptyString()
    {
        Assert.That(Base32.ExtendedHex.Encode(null, false), Is.EqualTo(String.Empty));
    }

    [Test]
    [TestCase("!@#!#@!#@#!@")]
    [TestCase("||||")]
    public void Decode_InvalidInput_ThrowsArgumentException(string input)
    {
        _ = Assert.Throws<ArgumentException>(() => Base32.ExtendedHex.Decode(input));
    }
}
