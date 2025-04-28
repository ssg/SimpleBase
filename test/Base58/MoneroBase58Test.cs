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
class MoneroBase58Test
{
    // test data is taken from https://github.com/monero-rs/base58-monero/blob/main/src/base58.rs
    // MIT License
    // Copyright(c) 2019-2023 Monero Rust Contributors
    // see LICENSE.monero-rs.txt file for details
    static readonly TestCaseData[] bitcoinTestData =
    [
        new TestCaseData("00", "11"),
        new TestCaseData("39", "1z"),
        new TestCaseData("FF", "5Q"),
        new TestCaseData("0000", "111"),
        new TestCaseData("0039", "11z"),
        new TestCaseData("0100", "15R"),
        new TestCaseData("FFFF", "LUv"),
        new TestCaseData("000000", "11111"),
        new TestCaseData("000039", "1111z"),
        new TestCaseData("010000", "11LUw"),
        new TestCaseData("FFFFFF", "2UzHL"),
        new TestCaseData("00000039", "11111z"),
        new TestCaseData("FFFFFFFF", "7YXq9G"),
        new TestCaseData("0000000039", "111111z"),
        new TestCaseData("FFFFFFFFFF", "VtB5VXc"),
        new TestCaseData("000000000039", "11111111z"),
        new TestCaseData("FFFFFFFFFFFF", "3CUsUpv9t"),
        new TestCaseData("00000000000039", "111111111z"),
        new TestCaseData("FFFFFFFFFFFFFF", "Ahg1opVcGW"),
        new TestCaseData("0000000000000039", "1111111111z"),
        new TestCaseData("FFFFFFFFFFFFFFFF", "jpXCZedGfVQ"),
        new TestCaseData("0000000000000000", "11111111111"),
        new TestCaseData("0000000000000001", "11111111112"),
        new TestCaseData("0000000000000008", "11111111119"),
        new TestCaseData("0000000000000009", "1111111111A"),
        new TestCaseData("000000000000003A", "11111111121"),
        new TestCaseData("00FFFFFFFFFFFFFF", "1Ahg1opVcGW"),
        new TestCaseData("06156013762879F7", "22222222222"),
        new TestCaseData("05E022BA374B2A00", "1z111111111"),

        new TestCaseData("00", "11"),
        new TestCaseData("0000", "111"),
        new TestCaseData("000000", "11111"),
        new TestCaseData("00000000", "111111"),
        new TestCaseData("0000000000", "1111111"),
        new TestCaseData("000000000000", "111111111"),
        new TestCaseData("00000000000000", "1111111111"),
        new TestCaseData("0000000000000000", "11111111111"),
        new TestCaseData("000000000000000000", "1111111111111"),
        new TestCaseData("00000000000000000000", "11111111111111"),
        new TestCaseData("0000000000000000000000", "1111111111111111"),
        new TestCaseData("000000000000000000000000", "11111111111111111"),
        new TestCaseData("00000000000000000000000000", "111111111111111111"),
        new TestCaseData("0000000000000000000000000000", "11111111111111111111"),
        new TestCaseData("000000000000000000000000000000", "111111111111111111111"),
        new TestCaseData("00000000000000000000000000000000", "1111111111111111111111"),
        new TestCaseData("06156013762879F7FFFFFFFFFF", "22222222222VtB5VXc"),
    ];

    [Test]
    public void Encode_NullBuffer_ReturnsEmptyString()
    {
        Assert.That(Base58.Monero.Encode(null), Is.EqualTo(string.Empty));
    }

    [Test]
    [TestCaseSource(nameof(bitcoinTestData))]
    public void Encode_Bitcoin_ReturnsExpectedResults(string input, string expectedOutput)
    {
        var buffer = Base16.UpperCase.Decode(input);
        string result = Base58.Monero.Encode(buffer);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(bitcoinTestData))]
    public void TryEncode_Bitcoin_ReturnsExpectedResults(string input, string expectedOutput)
    {
        var inputBuffer = Base16.UpperCase.Decode(input);
        var outputBuffer = new char[Base58.Monero.GetSafeCharCountForEncoding(inputBuffer)];
        Assert.Multiple(() =>
        {
            Assert.That(Base58.Monero.TryEncode(inputBuffer, outputBuffer, out int numWritten), Is.True);
            Assert.That(outputBuffer[..numWritten], Is.EqualTo(expectedOutput));
        });
    }

    [Test]
    public void Encode_EmptyBuffer_ReturnsEmptyString()
    {
        Assert.That(Base58.Monero.Encode([]), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Decode_EmptyString_ReturnsEmptyBuffer()
    {
        var result = Base58.Monero.Decode(string.Empty);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void TryDecode_EmptyString_ReturnsEmptyBuffer()
    {
        var result = Base58.Monero.TryDecode(string.Empty, new byte[1], out int numBytesWritten);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(numBytesWritten, Is.EqualTo(0));
        });
    }

    [Test]
    public void Decode_InvalidCharacter_Throws()
    {
        _ = Assert.Throws<ArgumentException>(() => Base58.Monero.Decode("?"));
    }

    [Test]
    public void TryDecode_InvalidCharacter_Throws()
    {
        _ = Assert.Throws<ArgumentException>(() => Base58.Monero.TryDecode("?", new byte[10], out _));
    }

    [Test]
    [TestCaseSource(nameof(bitcoinTestData))]
    public void Decode_Bitcoin_ReturnsExpectedResults(string expectedOutput, string input)
    {
        var buffer = Base58.Monero.Decode(input);
        string result = BitConverter.ToString(buffer).Replace("-", "",
            StringComparison.Ordinal);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(bitcoinTestData))]
    public void TryDecode_Bitcoin_ReturnsExpectedResults(string expectedOutput, string input)
    {
        var output = new byte[Base58.Monero.GetSafeByteCountForDecoding(input)];
        var success = Base58.Monero.TryDecode(input, output, out int numBytesWritten);
        Assert.That(success, Is.True);
        string result = BitConverter.ToString(output[..numBytesWritten]).Replace("-", "",
            StringComparison.Ordinal);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }
}
