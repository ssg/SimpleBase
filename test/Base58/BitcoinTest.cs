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
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base58Test;

[TestFixture]
[Parallelizable]
class BitcoinTest
{
    private static readonly TestCaseData[] bitcoinTestData =
    [
        new TestCaseData("0001", "12"),
        new TestCaseData("0000010203", "11Ldp"),
        new TestCaseData("009C1CA2CBA6422D3988C735BB82B5C880B0441856B9B0910F", "1FESiat4YpNeoYhW3Lp7sW1T6WydcW7vcE"),
        new TestCaseData("000860C220EBBAF591D40F51994C4E2D9C9D88168C33E761F6", "1mJKRNca45GU2JQuHZqZjHFNktaqAs7gh"),
        new TestCaseData("00313E1F905554E7AE2580CD36F86D0C8088382C9E1951C44D010203", "17f1hgANcLE5bQhAGRgnBaLTTs23rK4VGVKuFQ"),
        new TestCaseData("0000000000", "11111"),
        new TestCaseData("1111111111", "2vgLdhi"),
        new TestCaseData("FFEEDDCCBBAA", "3CSwN61PP"),
        new TestCaseData("00", "1"),
        new TestCaseData("21", "a"),
        new TestCaseData("000102030405060708090A0B0C0D0E0F000102030405060708090A0B0C0D0E0F", "1thX6LZfHDZZKUs92febWaf4WJZnsKRiVwJusXxB7L"),
        new TestCaseData("0000000000000000000000000000000000000000000000000000", "11111111111111111111111111"),
    ];

    [Test]
    public void Encode_NullBuffer_ReturnsEmptyString()
    {
        Assert.That(Base58.Bitcoin.Encode(null), Is.EqualTo(String.Empty));
    }

    [Test]
    [TestCaseSource(nameof(bitcoinTestData))]
    public void Encode_Bitcoin_ReturnsExpectedResults(string input, string expectedOutput)
    {
        var buffer = Base16.UpperCase.Decode(input);
        string result = Base58.Bitcoin.Encode(buffer);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(bitcoinTestData))]
    public void TryEncode_Bitcoin_ReturnsExpectedResults(string input, string expectedOutput)
    {
        var inputBuffer = Base16.UpperCase.Decode(input);
        var outputBuffer = new char[Base58.Bitcoin.GetSafeCharCountForEncoding(inputBuffer)];
        Assert.Multiple(() =>
        {
            Assert.That(Base58.Bitcoin.TryEncode(inputBuffer, outputBuffer, out int numWritten), Is.True);
            Assert.That(outputBuffer[..numWritten], Is.EqualTo(expectedOutput));
        });
    }

    [Test]
    public void Encode_EmptyBuffer_ReturnsEmptyString()
    {
        Assert.That(Base58.Bitcoin.Encode([]), Is.EqualTo(String.Empty));
    }

    [Test]
    public void Decode_EmptyString_ReturnsEmptyBuffer()
    {
        var result = Base58.Bitcoin.Decode(String.Empty);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void TryDecode_EmptyString_ReturnsEmptyBuffer()
    {
        var result = Base58.Bitcoin.TryDecode(String.Empty, new byte[1], out int numBytesWritten);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(numBytesWritten, Is.EqualTo(0));
        });
    }

    [Test]
    public void Decode_InvalidCharacter_Throws()
    {
        _ = Assert.Throws<ArgumentException>(() => Base58.Bitcoin.Decode("?"));
    }

    [Test]
    public void TryDecode_InvalidCharacter_Throws()
    {
        _ = Assert.Throws<ArgumentException>(() => Base58.Bitcoin.TryDecode("?", new byte[10], out _));
    }

    [Test]
    [TestCaseSource(nameof(bitcoinTestData))]
    public void Decode_Bitcoin_ReturnsExpectedResults(string expectedOutput, string input)
    {
        var buffer = Base58.Bitcoin.Decode(input);
        string result = BitConverter.ToString(buffer).Replace("-", "",
            StringComparison.Ordinal);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(bitcoinTestData))]
    public void TryDecode_Bitcoin_ReturnsExpectedResults(string expectedOutput, string input)
    {
        var output = new byte[Base58.Bitcoin.GetSafeByteCountForDecoding(input)];
        var success = Base58.Bitcoin.TryDecode(input, output, out int numBytesWritten);
        Assert.That(success, Is.True);
        string result = BitConverter.ToString(output[..numBytesWritten]).Replace("-", "",
            StringComparison.Ordinal);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }
}
