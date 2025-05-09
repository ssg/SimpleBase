using NUnit.Framework;
using SimpleBase;
using System;
using System.Text;

namespace SimpleBaseTest.Base58Test;

[TestFixture]
public class Cb58Test
{
    // CB58 test cases have been taken from avalanchego package at https://github.com/ava-labs/avalanchego
    // BSD 3-Clause License
    // see LICENSE.avalanchego.txt file for details
    static readonly TestCaseData[] testData =
    [
        new TestCaseData(Array.Empty<byte>(), "45PJLL"),
		new TestCaseData(new byte[]{ 0}, "1c7hwa"),
		new TestCaseData(new byte[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 255}, "1NVSVezva3bAtJesnUj"),
        new TestCaseData(new byte[32] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 },
			"SkB92YpWm4Q2ijQHH34cqbKkCZWszsiQgHVjtNeFF2HdvDQU"),
	];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void EncodeCheck_ValidInput_ReturnsExpectedResult(byte[] bytes, string expectedOutput)
    {
        string result = Base58.Bitcoin.EncodeCb58(bytes);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryDecodeCheck_ValidInput_ReturnsExpectedResult(byte[] expectedOutput, string input)
    {
        byte[] outputBuffer = new byte[256];
        bool result = Base58.Bitcoin.TryDecodeCb58(input, outputBuffer.AsSpan(), out int bytesWritten);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(outputBuffer[..bytesWritten], Is.EqualTo(expectedOutput));
        });
    }

}
