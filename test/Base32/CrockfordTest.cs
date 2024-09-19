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
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base32Test;

[TestFixture]
class CrockfordTest
{
    private static readonly object[][] testData = [
        ["", "", false],
        ["f", "CR", false],
        ["f", "CR======", true],
        ["fo", "CSQG", false],
        ["fo", "CSQG====", true],
        ["foo", "CSQPY", false],
        ["foo", "CSQPY===", true],
        ["foob", "CSQPYRG", false],
        ["foob", "CSQPYRG=", true],
        ["fooba", "CSQPYRK1", false],
        ["fooba", "CSQPYRK1", true],
        ["foobar", "CSQPYRK1E8", false],
        ["foobar", "CSQPYRK1E8======", true],
        ["123456789012345678901234567890123456789", "64S36D1N6RVKGE9G64S36D1N6RVKGE9G64S36D1N6RVKGE9G64S36D1N6RVKGE8", false],
        ["123456789012345678901234567890123456789", "64S36D1N6RVKGE9G64S36D1N6RVKGE9G64S36D1N6RVKGE9G64S36D1N6RVKGE8=", true]
    ];        

    [Test]
    public void Encode_SampleInterface_Compiles()
    {
        // this source code exists in samples and just needs to be compiled and run without errors.
        // do not edit/refactor the code below
        byte[] myBuffer = [];
        string result = Base32.Crockford.Encode(myBuffer, padding: true);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Decode_SampleInterface_Compiles()
    {
        // this source code exists in samples and just needs to be compiled and run without errors.
        // do not edit/refactor the code below
        string myText = "CSQPYRK1E8"; // any buffer will do
        Span<byte> result = Base32.Crockford.Decode(myText);
        Assert.That(result.Length, Is.GreaterThan(0));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_Stream_ReturnsExpectedValues(string input, string expectedOutput, bool padded)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using var inputStream = new MemoryStream(bytes);
        using var writer = new StringWriter();
        Base32.Crockford.Encode(inputStream, writer, padded);
        Assert.That(writer.ToString(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_SimpleStream_ReturnsExpectedValues(string input, string expectedOutput, bool padded)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using var inputStream = new MemoryStream(bytes);
        using var writer = new StringWriter();
        Base32.Crockford.Encode(inputStream, writer, padded);
        Assert.That(writer.ToString(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public async Task EncodeAsync_SimpleStream_ReturnsExpectedValues(string input, string expectedOutput, bool padded)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using var inputStream = new MemoryStream(bytes);
        using var writer = new StringWriter();
        await Base32.Crockford.EncodeAsync(inputStream, writer, padded);
        Assert.That(writer.ToString(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_Stream_ReturnsExpectedValues(string expectedOutput, string input, bool _)
    {
        // upper case
        using (var inputStream = new StringReader(input))
        using (var outputStream = new MemoryStream())
        {
            Base32.Crockford.Decode(inputStream, outputStream);
            string result = Encoding.ASCII.GetString(outputStream.ToArray());
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        // lower case
        using (var inputStream = new StringReader(input.ToLowerInvariant()))
        using (var outputStream = new MemoryStream())
        {
            Base32.Crockford.Decode(inputStream, outputStream);
            string result = Encoding.ASCII.GetString(outputStream.ToArray());
            Assert.That(result, Is.EqualTo(expectedOutput));
        }
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public async Task DecodeAsync_Stream_ReturnsExpectedValues(string expectedOutput, string input, bool _)
    {
        // upper case
        using (var inputStream = new StringReader(input))
        using (var outputStream = new MemoryStream())
        {
            await Base32.Crockford.DecodeAsync(inputStream, outputStream);
            string result = Encoding.ASCII.GetString(outputStream.ToArray());
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        // lower case
        using (var inputStream = new StringReader(input.ToLowerInvariant()))
        using (var outputStream = new MemoryStream())
        {
            await Base32.Crockford.DecodeAsync(inputStream, outputStream);
            string result = Encoding.ASCII.GetString(outputStream.ToArray());
            Assert.That(result, Is.EqualTo(expectedOutput));
        }
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_ReturnsExpectedValues(string input, string expectedOutput, bool padded)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        string result = Base32.Crockford.Encode(bytes, padded);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_Simple_ReturnsExpectedValues(string input, string expectedOutput, bool padded)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        string result = Base32.Crockford.Encode(bytes, padded);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryEncode_Simple_ReturnsExpectedValues(string input, string expectedOutput, bool padded)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        var output = new char[Base32.Crockford.GetSafeCharCountForEncoding(bytes)];
        bool success = Base32.Crockford.TryEncode(bytes, output, padded, out int numCharsWritten);
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(output[..numCharsWritten], Is.EqualTo(expectedOutput));
        });
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_ReturnsExpectedValues(string expectedOutput, string input, bool _)
    {
        var bytes = Base32.Crockford.Decode(input);
        string result = Encoding.ASCII.GetString(bytes);
        Assert.That(result, Is.EqualTo(expectedOutput));
        bytes = Base32.Crockford.Decode(input.ToLowerInvariant());
        result = Encoding.ASCII.GetString(bytes);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryDecode_ReturnsExpectedValues(string expectedOutput, string input, bool _)
    {
        var output = new byte[Base32.Crockford.GetSafeByteCountForDecoding(input)];
        var success = Base32.Crockford.TryDecode(input, output, out int numBytesWritten);
        Assert.That(success, Is.True);
        string result = Encoding.ASCII.GetString(output[..numBytesWritten]);
        Assert.That(result, Is.EqualTo(expectedOutput));

        success = Base32.Crockford.TryDecode(input.ToLowerInvariant(), output, out numBytesWritten);
        Assert.That(success, Is.True);
        result = Encoding.ASCII.GetString(output[..numBytesWritten]);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void TryDecode_ZeroBuffer_ReturnsFalse()
    {
        var success = Base32.Crockford.TryDecode("test", [], out int numBytesWritten);
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.False);
            Assert.That(numBytesWritten, Is.EqualTo(0));
        });
    }

    [Test]
    public void Decode_InvalidInput_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() => Base32.Crockford.Decode("[];',m."));
    }

    [Test]
    [TestCase("O0o", "000")]
    [TestCase("Ll1", "111")]
    [TestCase("I1i", "111")]
    public void Decode_CrockfordChars_DecodedCorrectly(string equivalent, string actual)
    {
        var expectedResult = Base32.Crockford.Decode(actual);
        var result = Base32.Crockford.Decode(equivalent);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void Encode_NullBytes_ReturnsEmptyString()
    {
        Assert.That(Base32.Crockford.Encode(null, true), Is.EqualTo(String.Empty));
    }
}
