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

namespace SimpleBaseTest.Base32Test;

[TestFixture]
class Rfc4648Test
{
    private static readonly string[][] testData =
    [
        ["", ""],
        ["f", "MY======"],
        ["fo", "MZXQ===="],
        ["foo", "MZXW6==="],
        ["foob", "MZXW6YQ="],
        ["fooba", "MZXW6YTB"],
        ["foobar", "MZXW6YTBOI======"],
        ["foobar1", "MZXW6YTBOIYQ===="],
        ["foobar12", "MZXW6YTBOIYTE==="],
        ["foobar123", "MZXW6YTBOIYTEMY="],
        ["1234567890123456789012345678901234567890", "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ"],
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_ReturnsExpectedValues(string input, string expectedOutput)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        string result = Base32.Rfc4648.Encode(bytes, padding: true);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_ReturnsExpectedValues(string expectedOutput, string input)
    {
        var bytes = Base32.Rfc4648.Decode(input);
        string result = Encoding.ASCII.GetString(bytes);
        Assert.That(result, Is.EqualTo(expectedOutput));
        bytes = Base32.Rfc4648.Decode(input.ToLowerInvariant());
        result = Encoding.ASCII.GetString(bytes);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Encode_NullBytes_ReturnsEmptyString()
    {
        Assert.That(Base32.Rfc4648.Encode(null, true), Is.EqualTo(String.Empty));
    }

    [Test]
    public void Decode_InvalidInput_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() => Base32.Rfc4648.Decode("[];',m."));
    }

    private static readonly TestCaseData[] ulongTestCases =
    [
        new TestCaseData(0UL,                  "AA"),
        new TestCaseData(0x0000000000000011UL, "CE"),
        new TestCaseData(0x0000000000001122UL, "EIIQ"),
        new TestCaseData(0x0000000000112233UL, "GMRBC"),
        new TestCaseData(0x0000000011223344UL, "IQZSEEI"),
        new TestCaseData(0x0000001122334455UL, "KVCDGIQR"),
        new TestCaseData(0x0000112233445566UL, "MZKUIMZCCE"),
        new TestCaseData(0x0011223344556677UL, "O5TFKRBTEIIQ"),
        new TestCaseData(0x1122334455667788UL, "RB3WMVKEGMRBC"),
        new TestCaseData(0x1100000000000000UL, "AAAAAAAAAAABC"),
        new TestCaseData(0x1122000000000000UL, "AAAAAAAAAARBC"),
        new TestCaseData(0x1122330000000000UL, "AAAAAAAAGMRBC"),
        new TestCaseData(0x1122334400000000UL, "AAAAAACEGMRBC"),
        new TestCaseData(0x1122334455000000UL, "AAAAAVKEGMRBC"),
        new TestCaseData(0x1122334455660000UL, "AAAGMVKEGMRBC"),
        new TestCaseData(0x1122334455667700UL, "AB3WMVKEGMRBC"),
    ];

    [Test]
    [TestCaseSource(nameof(ulongTestCases))]
    public void Encode_ulong_ReturnsExpectedValues(ulong number, string expectedOutput)
    {
        Assert.That(Base32.Rfc4648.Encode(number), Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(ulongTestCases))]
    public void Encode_BigEndian_ulong_ReturnsExpectedValues(ulong number, string expectedOutput)
    {
        if (!BitConverter.IsLittleEndian)
        {
            throw new InvalidOperationException("big endian tests are only supported on little endian archs");
        }
        number = reverseBytes(number);

        var bigEndian = new Base32(Base32Alphabet.Rfc4648, isBigEndian: true);
        Assert.That(bigEndian.Encode(number), Is.EqualTo(expectedOutput));
    }

    private static ulong reverseBytes(ulong number)
    {
        var span = BitConverter.GetBytes(number).AsSpan();
        span.Reverse();
        return BitConverter.ToUInt64(span);
    }

    [Test]
    [TestCaseSource(nameof(ulongTestCases))]
    public void DecodeUInt64_ReturnsExpectedValues(ulong expectedNumber, string input)
    {
        Assert.That(Base32.Rfc4648.DecodeUInt64(input), Is.EqualTo(expectedNumber));
    }

    [Test]
    [TestCaseSource(nameof(ulongTestCases))]
    public void DecodeUInt64_BigEndian_ReturnsExpectedValues(ulong expectedNumber, string input)
    {
        if (!BitConverter.IsLittleEndian)
        {
            throw new InvalidOperationException("big endian tests are only supported on little endian archs");
        }
        expectedNumber = reverseBytes(expectedNumber);
        var bigEndian = new Base32(Base32Alphabet.Rfc4648, isBigEndian: true);
        Assert.That(bigEndian.DecodeUInt64(input), Is.EqualTo(expectedNumber));
    }

    [Test]
    public void Encode_long_NegativeValues_Throws()
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => Base32.Rfc4648.Encode(-1));
    }
}
