using NUnit.Framework;
using SimpleBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBaseTest.Base16Test;

[TestFixture]
[Parallelizable]
internal class Base16Test
{
    private static readonly Base16[] encoders =
    [
        Base16.LowerCase,
        Base16.UpperCase,
        Base16.ModHex
    ];

    private static readonly object[][] testCases =
    [                                                                   // LowerCase        // UpperCase        // ModHex
        [Array.Empty<byte>(),                                           "",                 "",                 ""],
        [new byte[] { 0xAB },                                           "ab",               "AB",               "ln"],
        [new byte[] { 0x00, 0x01, 0x02, 0x03 },                         "00010203",         "00010203",         "cccbcdce"],
        [new byte[] { 0x10, 0x11, 0x12, 0x13 },                         "10111213",         "10111213",         "bcbbbdbe"],
        [new byte[] { 0xAB, 0xCD, 0xEF, 0xBA },                         "abcdefba",         "ABCDEFBA",         "lnrtuvnl"],
        [new byte[] { 0xAB, 0xCD, 0xEF, 0xBA, 0xAB, 0xCD, 0xEF, 0xBA }, "abcdefbaabcdefba", "ABCDEFBAABCDEFBA", "lnrtuvnllnrtuvnl"],
    ];

    private static IEnumerable<TestCaseData> testData
    {
        get
        {
            foreach (var pair in encoders.Select((encoder, index) => (encoder, index)))
            {
                foreach (var testRow in testCases)
                {
                    var testValue = testRow[pair.index + 1];
                    yield return new TestCaseData(pair.encoder, testRow[0], testValue)
                        .SetName($"{pair.encoder.Alphabet}_{testValue}");
                }
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
        Assert.That(memoryStream.ToArray(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_Stream(Base16 encoder, byte[] input, string expectedOutput)
    {
        using var inputStream = new MemoryStream(input);
        using var writer = new StringWriter();
        encoder.Encode(inputStream, writer);
        Assert.That(writer.ToString(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public async Task DecodeAsync_Stream(Base16 encoder, byte[] expectedOutput, string input)
    {
        using var memoryStream = new MemoryStream();
        using var reader = new StringReader(input);
        await encoder.DecodeAsync(reader, memoryStream);
        Assert.That(memoryStream.ToArray(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public async Task EncodeAsync_StreamAsync(Base16 encoder, byte[] input, string expectedOutput)
    {
        using var inputStream = new MemoryStream(input);
        using var writer = new StringWriter();
        await encoder.EncodeAsync(inputStream, writer);
        Assert.That(writer.ToString(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode(Base16 encoder, byte[] input, string expectedOutput)
    {
        var result = encoder.Encode(input);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryEncode_RegularInput_Succeeds(Base16 encoder, byte[] input, string expectedOutput)
    {
        var output = new char[input.Length * 2];
        Assert.Multiple(() =>
        {
            Assert.That(encoder.TryEncode(input, output, out int numCharsWritten), Is.True);
            Assert.That(numCharsWritten, Is.EqualTo(output.Length));
            Assert.That(new string(output), Is.EqualTo(expectedOutput));
        });
    }

    [Test]
    [TestCaseSource(nameof(encoders))]
    public void TryEncode_SmallerOutput_Fails(Base16 encoder)
    {
        var input = new byte[4];
        var output = Array.Empty<char>();
        Assert.Multiple(() =>
        {
            Assert.That(encoder.TryEncode(input, output, out int numCharsWritten), Is.False);
            Assert.That(numCharsWritten, Is.EqualTo(0));
        });
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode(Base16 encoder, byte[] expectedOutput, string input)
    {
        var result = encoder.Decode(input);
        Assert.That(result.ToArray(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryDecode_RegularInput_Succeeds(Base16 encoder, byte[] expectedOutput, string input)
    {
        var output = new byte[expectedOutput.Length];
        Assert.Multiple(() =>
        {
            Assert.That(encoder.TryDecode(input, output, out int numBytesWritten), Is.True);
            Assert.That(numBytesWritten, Is.EqualTo(output.Length));
            Assert.That(output, Is.EqualTo(expectedOutput));
        });
    }

    [Test]
    public void TryDecode_InvalidChar_ReturnsFalse()
    {
        var output = new byte[3];
        Assert.Multiple(() =>
        {
            Assert.That(Base16.UpperCase.TryDecode("1234ZB", output, out int numWritten), Is.False);
            Assert.That(numWritten, Is.EqualTo(2));
        });
    }

    [Test]
    [TestCaseSource(nameof(encoders))]
    public void TryDecode_SmallOutputBuffer_Fails(Base16 encoder)
    {
        var input = "1234";
        var output = new byte[1];
        Assert.Multiple(() =>
        {
            Assert.That(encoder.TryDecode(input, output, out int numBytesWritten), Is.False);
            Assert.That(numBytesWritten, Is.EqualTo(0));
        });
    }

    [Test]
    [TestCaseSource(nameof(encoders))]
    public void TryDecode_UnevenInputBuffer_Fails(Base16 encoder)
    {
        var input = "123";
        var output = new byte[1];
        Assert.Multiple(() =>
        {
            Assert.That(encoder.TryDecode(input, output, out int numBytesWritten), Is.False);
            Assert.That(numBytesWritten, Is.EqualTo(0));
        });
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_OtherCase_StillPasses(Base16 encoder, byte[] expectedOutput, string input)
    {
        var result = encoder.Decode(input.ToUpperInvariant());
        Assert.That(result.ToArray(), Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Decode_InvalidChar_Throws(
        [ValueSource(nameof(encoders))]Base16 encoder,
        [Values("AZ12", "ZAAA", "!AAA", "=AAA")]string input)
    {
        _ = Assert.Throws<ArgumentException>(() => encoder.Decode(input));
    }

    [Test]
    public void Decode_InvalidLength_Throws(
        [ValueSource(nameof(encoders))]Base16 encoder,
        [Values("123", "12345")]string input)
    {
        _ = Assert.Throws<ArgumentException>(() => encoder.Decode(input));
    }

    [Test]
    [TestCaseSource(nameof(encoders))]
    public void GetSafeCharCountForEncoding_ReturnsCorrectValue(Base16 encoder)
    {
        var input = new byte[5];
        Assert.That(encoder.GetSafeCharCountForEncoding(input), Is.EqualTo(10));
    }

    [Test]
    [TestCaseSource(nameof(encoders))]
    public void GetSafeByteCountForDecoding_ReturnsCorrectValues(Base16 encoder)
    {
        var input = new char[10];
        Assert.That(encoder.GetSafeByteCountForDecoding(input), Is.EqualTo(5));
    }

    [Test]
    [TestCaseSource(nameof(encoders))]
    public void GetSafeByteCountForDecoding_InvalidBufferSize_ReturnsZero(Base16 encoder)
    {
        var input = new char[11];
        Assert.That(encoder.GetSafeByteCountForDecoding(input), Is.EqualTo(0));
    }

    [Test]
    public void CustomCtor()
    {
        var encoder = new Base16(new Base16Alphabet("abcdefghijklmnop"));
        var result = encoder.Encode([0, 1, 16, 128, 255]);
        Assert.That(result, Is.EqualTo("aaabbaiapp"));
    }

    [Test]
    public void ToString_ReturnsNameWithAlphabet([ValueSource(nameof(encoders))]Base16 encoder)
    {
        Assert.That(encoder.ToString(), Is.EqualTo($"Base16_{encoder.Alphabet}"));
    }

    [Test]
    public void GetHashCode_ReturnsAlphabetHashCode([ValueSource(nameof(encoders))]Base16 encoder)
    {
        Assert.That(encoder.GetHashCode(), Is.EqualTo(encoder.Alphabet.GetHashCode()));
    }
}
