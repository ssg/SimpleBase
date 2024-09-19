using NUnit.Framework;
using SimpleBase;
using System;

namespace SimpleBaseTest.Base85Test;

class Z85Test
{
    private static readonly TestCaseData[] testVectors =
    [
        new TestCaseData(Array.Empty<byte>(), ""),
        new TestCaseData(new byte[] { 0x86, 0x4F, 0xD2, 0x6F, 0xB5, 0x59, 0xF7, 0x5B }, "HelloWorld"),
        new TestCaseData(new byte[] { 0x11 }, "5D"),
        new TestCaseData(new byte[] { 0x11, 0x22 }, "5H4"),
        new TestCaseData(new byte[] { 0x11, 0x22, 0x33 }, "5H61"),
        new TestCaseData(new byte[] { 0x11, 0x22, 0x33, 0x44 }, "5H620"),
        new TestCaseData(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55 }, "5H620rr"),
        new TestCaseData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, "00000"),
        new TestCaseData(new byte[] { 0x20, 0x20, 0x20, 0x20 }, "arR^H"),
    ];

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public void Encode_TestVectors_ShouldEncodeCorrectly(byte[] input, string expectedOutput)
    {
        var result = Base85.Z85.Encode(input);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Encode_NullBuffer_ReturnsEmptyString()
    {
        Assert.That(Base85.Z85.Encode(null), Is.EqualTo(String.Empty));
    }

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public void Decode_TestVectors_ShouldDecodeCorrectly(byte[] expectedOutput, string input)
    {
        var result = Base85.Z85.Decode(input);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }
}
