/*
     Copyright 2014-2025 Sedat Kapanoglu

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

namespace SimpleBaseTest;

[TestFixture]
class Base2Test
{
    static readonly object[][] testData =
    [
        [new byte[] { }, ""],
        [new byte[] { 0x00, 0x01, 0x02, 0x03 }, "00000000" + "00000001" + "00000010" + "00000011"],
        [new byte[] { 0xFF, 0xFE, 0xFD, 0xFC }, "11111111" + "11111110" + "11111101" + "11111100"],
        [new byte[] { 0x00, 0x01, 0x02, 0x03, 0xFF, 0xFE, 0xFD }, "00000000" + "00000001" + "00000010" + "00000011" + "11111111" + "11111110" + "11111101"]
    ];

    static readonly string[] nonCanonicalInput =
    [
        "1",
        "10",
        "101",
        "1010",
        "101010",
        "1010101",
    ];

    static readonly object[][] edgeCaseData =
    [
        [new byte[] { 0x00 }, "00000000"],
        [new byte[] { 0xFF }, "11111111"],
        [new byte[] { 0x01, 0x02 }, "0000000100000010"],
        [new byte[] { 0xAA, 0x55 }, "1010101001010101"],
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_EncodesCorrectly(byte[] decoded, string encoded)
    {
        string result = Base2.Default.Encode(decoded);
        Assert.That(result, Is.EqualTo(encoded));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_DecodesCorrectly(byte[] decoded, string encoded)
    {
        var bytes = Base2.Default.Decode(encoded);
        Assert.That(bytes, Is.EqualTo(decoded));
    }

    [Test]
    [TestCaseSource(nameof(nonCanonicalInput))]
    public void Decode_NonCanonicalInput_Throws(string nonCanonicalText)
    {
        Assert.Throws<ArgumentException>(() => Base2.Default.Decode(nonCanonicalText));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryEncode_ValidInput_ReturnsExpectedValues(byte[] input, string expectedOutput)
    {
        var output = new char[Base2.Default.GetSafeCharCountForEncoding(input)];
        bool success = Base2.Default.TryEncode(input, output, out int numCharsWritten);
        
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(output[..numCharsWritten], Is.EqualTo(expectedOutput.ToCharArray()));
            Assert.That(numCharsWritten, Is.EqualTo(expectedOutput.Length));
        });
    }

    [Test]
    [TestCaseSource(nameof(edgeCaseData))]
    public void TryEncode_EdgeCases_ReturnsExpectedValues(byte[] input, string expectedOutput)
    {
        var output = new char[Base2.Default.GetSafeCharCountForEncoding(input)];
        bool success = Base2.Default.TryEncode(input, output, out int numCharsWritten);
        
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(new string(output[..numCharsWritten]), Is.EqualTo(expectedOutput));
            Assert.That(numCharsWritten, Is.EqualTo(expectedOutput.Length));
        });
    }

    [Test]
    public void TryEncode_EmptyInput_ReturnsTrue()
    {
        var output = new char[1];
        bool success = Base2.Default.TryEncode([], output, out int numCharsWritten);
        
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(numCharsWritten, Is.EqualTo(0));
        });
    }

    [Test]
    public void TryEncode_InsufficientOutputBuffer_ReturnsFalse()
    {
        var input = new byte[] { 0xFF };
        var output = new char[7]; // needs 8 characters
        bool success = Base2.Default.TryEncode(input, output, out int numCharsWritten);
        
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.False);
            Assert.That(numCharsWritten, Is.EqualTo(0));
        });
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryDecode_ValidInput_ReturnsExpectedValues(byte[] expectedOutput, string input)
    {
        var output = new byte[Base2.Default.GetSafeByteCountForDecoding(input)];
        bool success = Base2.Default.TryDecode(input, output, out int bytesWritten);
        
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(output[..bytesWritten], Is.EqualTo(expectedOutput));
            Assert.That(bytesWritten, Is.EqualTo(expectedOutput.Length));
        });
    }

    [Test]
    [TestCaseSource(nameof(edgeCaseData))]
    public void TryDecode_EdgeCases_ReturnsExpectedValues(byte[] expectedOutput, string input)
    {
        var output = new byte[Base2.Default.GetSafeByteCountForDecoding(input)];
        bool success = Base2.Default.TryDecode(input, output, out int bytesWritten);
        
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(output[..bytesWritten], Is.EqualTo(expectedOutput));
            Assert.That(bytesWritten, Is.EqualTo(expectedOutput.Length));
        });
    }

    [Test]
    public void TryDecode_EmptyInput_ReturnsTrue()
    {
        var output = new byte[1];
        bool success = Base2.Default.TryDecode("", output, out int bytesWritten);
        
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(bytesWritten, Is.EqualTo(0));
        });
    }

    [Test]
    [TestCaseSource(nameof(nonCanonicalInput))]
    public void TryDecode_NonCanonicalInput_ReturnsFalse(string nonCanonicalText)
    {
        var output = new byte[10];
        bool success = Base2.Default.TryDecode(nonCanonicalText, output, out int bytesWritten);
        
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.False);
            Assert.That(bytesWritten, Is.EqualTo(0));
        });
    }

    [Test]
    public void TryDecode_InvalidCharacters_ReturnsFalse()
    {
        var output = new byte[10];
        bool success = Base2.Default.TryDecode("0102abcd", output, out int bytesWritten);
        
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.False);
            Assert.That(bytesWritten, Is.EqualTo(0));
        });
    }

    [Test]
    public void TryDecode_InsufficientOutputBuffer_ReturnsFalse()
    {
        var output = new byte[0]; // insufficient buffer
        bool success = Base2.Default.TryDecode("00000001", output, out int bytesWritten);
        
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.False);
            Assert.That(bytesWritten, Is.EqualTo(0));
        });
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void GetSafeCharCountForEncoding_ValidInput_ReturnsCorrectCount(byte[] input, string expectedOutput)
    {
        int result = Base2.Default.GetSafeCharCountForEncoding(input);
        Assert.That(result, Is.EqualTo(expectedOutput.Length));
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(1, 8)]
    [TestCase(2, 16)]
    [TestCase(3, 24)]
    [TestCase(10, 80)]
    public void GetSafeCharCountForEncoding_VariousLengths_ReturnsCorrectCount(int inputLength, int expectedCharCount)
    {
        var input = new byte[inputLength];
        int result = Base2.Default.GetSafeCharCountForEncoding(input);
        Assert.That(result, Is.EqualTo(expectedCharCount));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void GetSafeByteCountForDecoding_ValidInput_ReturnsCorrectCount(byte[] expectedOutput, string input)
    {
        int result = Base2.Default.GetSafeByteCountForDecoding(input);
        Assert.That(result, Is.EqualTo(expectedOutput.Length));
    }

    [Test]
    [TestCase("", 0)]
    [TestCase("00000000", 1)]
    [TestCase("0000000000000000", 2)]
    [TestCase("000000000000000000000000", 3)]
    public void GetSafeByteCountForDecoding_VariousLengths_ReturnsCorrectCount(string input, int expectedByteCount)
    {
        int result = Base2.Default.GetSafeByteCountForDecoding(input);
        Assert.That(result, Is.EqualTo(expectedByteCount));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_Stream_ReturnsExpectedValues(byte[] input, string expectedOutput)
    {
        using var inputStream = new MemoryStream(input);
        using var writer = new StringWriter();
        
        Base2.Default.Encode(inputStream, writer);
        
        Assert.That(writer.ToString(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public async Task EncodeAsync_Stream_ReturnsExpectedValues(byte[] input, string expectedOutput)
    {
        using var inputStream = new MemoryStream(input);
        using var writer = new StringWriter();
        
        await Base2.Default.EncodeAsync(inputStream, writer);
        
        Assert.That(writer.ToString(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_Stream_ReturnsExpectedValues(byte[] expectedOutput, string input)
    {
        using var inputReader = new StringReader(input);
        using var outputStream = new MemoryStream();
        
        Base2.Default.Decode(inputReader, outputStream);
        
        Assert.That(outputStream.ToArray(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public async Task DecodeAsync_Stream_ReturnsExpectedValues(byte[] expectedOutput, string input)
    {
        using var inputReader = new StringReader(input);
        using var outputStream = new MemoryStream();
        
        await Base2.Default.DecodeAsync(inputReader, outputStream);
        
        Assert.That(outputStream.ToArray(), Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Encode_LargeStream_WorksCorrectly()
    {
        // Test with larger data to ensure stream handling works properly
        var largeData = new byte[10000];
        for (int i = 0; i < largeData.Length; i++)
        {
            largeData[i] = (byte)(i % 256);
        }

        using var inputStream = new MemoryStream(largeData);
        using var writer = new StringWriter();
        
        Base2.Default.Encode(inputStream, writer);
        string result = writer.ToString();
        
        // Verify round-trip
        var decoded = Base2.Default.Decode(result);
        Assert.That(decoded, Is.EqualTo(largeData));
    }

    [Test]
    public async Task EncodeAsync_LargeStream_WorksCorrectly()
    {
        // Test with larger data to ensure async stream handling works properly
        var largeData = new byte[10000];
        for (int i = 0; i < largeData.Length; i++)
        {
            largeData[i] = (byte)(i % 256);
        }

        using var inputStream = new MemoryStream(largeData);
        using var writer = new StringWriter();
        
        await Base2.Default.EncodeAsync(inputStream, writer);
        string result = writer.ToString();
        
        // Verify round-trip
        var decoded = Base2.Default.Decode(result);
        Assert.That(decoded, Is.EqualTo(largeData));
    }

    [Test]
    public void Decode_InvalidCharacter_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Base2.Default.Decode("0001002"));
    }

    [Test]
    public void Decode_InvalidCharacterMixed_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Base2.Default.Decode("01010abc"));
    }

    [Test]
    public void Decode_SpecialCharacters_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Base2.Default.Decode("01010!@#"));
    }

    [Test]
    public void Encode_NullInput_ReturnsEmptyString()
    {
        byte[]? nullBytes = null;
        string result = Base2.Default.Encode(nullBytes);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Encode_ReadOnlySpan_WorksCorrectly()
    {
        ReadOnlySpan<byte> input = new byte[] { 0xAB, 0xCD };
        string result = Base2.Default.Encode(input);
        Assert.That(result, Is.EqualTo("1010101111001101"));
    }

    [Test]
    public void Decode_ReadOnlySpan_WorksCorrectly()
    {
        ReadOnlySpan<char> input = "1010101111001101".AsSpan();
        byte[] result = Base2.Default.Decode(input);
        Assert.That(result, Is.EqualTo(new byte[] { 0xAB, 0xCD }));
    }

    [Test]
    public void Default_ImplementsAllInterfaces()
    {
        var instance = Base2.Default;
        
        Assert.Multiple(() =>
        {
            Assert.That(instance, Is.InstanceOf<IBaseCoder>());
            Assert.That(instance, Is.InstanceOf<INonAllocatingBaseCoder>());
            Assert.That(instance, Is.InstanceOf<IBaseStreamCoder>());
        });
    }

    [Test]
    public void Constructor_CreatesValidInstance()
    {
        var instance = new Base2();
        Assert.That(instance, Is.Not.Null);
        
        // Test basic functionality
        string result = instance.Encode(new byte[] { 0x42 });
        Assert.That(result, Is.EqualTo("01000010"));
    }
}


