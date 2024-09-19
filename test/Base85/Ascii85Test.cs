using NUnit.Framework;
using SimpleBase;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleBaseTest.Base85Test;

internal class Ascii85Test
{
    private static readonly object[][] testVectors =
    [
        [Array.Empty<byte>(), ""],
        [new byte[] { 0, 0, 0, 0 }, "z"],
        [new byte[] { 0x20, 0x20, 0x20, 0x20 }, "y"],
        [new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 }, "5sdq,70"],
        [new byte[] { 0x86, 0x4F, 0xD2, 0x6F, 0xB5, 0x59, 0xF7, 0x5B }, "L/669[9<6."],
        [new byte[] { 0x11, 0x22, 0x33 }, "&L'\""],
        [new byte[] { 77, 97, 110, 32 }, "9jqo^"],
    ];

    [Test]
    public void Decode_InvalidShortcut_ThrowsArgumentException()
    {
        const string input = "9zjqo";
        _ = Assert.Throws<ArgumentException>(() => Base85.Ascii85.Decode(input));
    }

    [Test]
    public void Decode_InvalidCharacter_ThrowsArgumentException()
    {
        const string input = "~!@#()(";
        _ = Assert.Throws<ArgumentException>(() => Base85.Ascii85.Decode(input));
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
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public void Encode_TestVectorsOnStream_ShouldEncodeCorrectly(byte[] input, string expectedOutput)
    {
        using var inputStream = new MemoryStream(input);
        using var writer = new StringWriter();
        Base85.Ascii85.Encode(inputStream, writer);
        Assert.That(writer.ToString(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public async Task EncodeAsync_TestVectorsOnStream_ShouldEncodeCorrectly(byte[] input, string expectedOutput)
    {
        using var inputStream = new MemoryStream(input);
        using var writer = new StringWriter();
        await Base85.Ascii85.EncodeAsync(inputStream, writer);
        Assert.That(writer.ToString(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public void Encode_TestVectors_ShouldEncodeCorrectly(byte[] input, string expectedOutput)
    {
        var result = Base85.Ascii85.Encode(input);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public void TryEncode_TestVectors_ShouldEncodeCorrectly(byte[] input, string expectedOutput)
    {
        var output = new char[Base85.Ascii85.GetSafeCharCountForEncoding(input)];
        Assert.Multiple(() =>
        {
            Assert.That(Base85.Ascii85.TryEncode(input, output, out int numCharsWritten), Is.True);
            Assert.That(new string(output[..numCharsWritten]), Is.EqualTo(expectedOutput));
        });
    }

    [Test]
    public void Encode_UnevenBuffer_DoesNotThrowArgumentException()
    {
        Assert.DoesNotThrow(() => Base85.Ascii85.Encode(new byte[3]));
    }

    [Test]
    public void Encode_NullBuffer_ReturnsEmptyString()
    {
        Assert.That(Base85.Ascii85.Encode(null), Is.EqualTo(String.Empty));
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
        Assert.That(writer.ToArray(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public async Task DecodeAsync_TestVectorsWithStream_ShouldDecodeCorrectly(byte[] expectedOutput, string input)
    {
        using var inputStream = new StringReader(input);
        using var writer = new MemoryStream();
        await Base85.Ascii85.DecodeAsync(inputStream, writer);
        Assert.That(writer.ToArray(), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public void Decode_TestVectors_ShouldDecodeCorrectly(byte[] expectedOutput, string input)
    {
        var result = Base85.Ascii85.Decode(input);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public void TryDecode_TestVectors_ShouldDecodeCorrectly(byte[] expectedOutput, string input)
    {
        var buffer = new byte[Base85.Ascii85.GetSafeByteCountForDecoding(input)];
        Assert.Multiple(() =>
        {
            Assert.That(Base85.Ascii85.TryDecode(input, buffer, out int numBytesWritten), Is.True);
            Assert.That(buffer[..numBytesWritten], Is.EqualTo(expectedOutput));
        });
    }
}
