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

    [Test]
    [TestCase(0,                  ExpectedResult = "AA")]
    [TestCase(0x0000000000000011, ExpectedResult = "CE")]
    [TestCase(0x0000000000001122, ExpectedResult = "EIIQ")]
    [TestCase(0x0000000000112233, ExpectedResult = "GMRBC")]
    [TestCase(0x0000000011223344, ExpectedResult = "IQZSEEI")]
    [TestCase(0x0000001122334455, ExpectedResult = "KVCDGIQR")]
    [TestCase(0x0000112233445566, ExpectedResult = "MZKUIMZCCE")]
    [TestCase(0x0011223344556677, ExpectedResult = "O5TFKRBTEIIQ")]
    [TestCase(0x1122334455667788, ExpectedResult = "RB3WMVKEGMRBC")]
    [TestCase(0x1100000000000000, ExpectedResult = "AAAAAAAAAAABC")]
    [TestCase(0x1122000000000000, ExpectedResult = "AAAAAAAAAARBC")]
    [TestCase(0x1122330000000000, ExpectedResult = "AAAAAAAAGMRBC")]
    [TestCase(0x1122334400000000, ExpectedResult = "AAAAAACEGMRBC")]
    [TestCase(0x1122334455000000, ExpectedResult = "AAAAAVKEGMRBC")]
    [TestCase(0x1122334455660000, ExpectedResult = "AAAGMVKEGMRBC")]
    [TestCase(0x1122334455667700, ExpectedResult = "AB3WMVKEGMRBC")]
    public string Encode_long_ReturnsExpectedValues(long number)
    {
        return Base32.Rfc4648.Encode(number);
    }

    [Test]
    public void Encode_long_NegativeValues_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Base32.Rfc4648.Encode(-1));
    }
}
