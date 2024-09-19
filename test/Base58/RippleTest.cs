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
class RippleTest
{
    private static readonly TestCaseData[] rippleTestData =
    [
        new TestCaseData("0000010203", "rrLdF"),
        new TestCaseData("009C1CA2CBA6422D3988C735BB82B5C880B0441856B9B0910F", "rENS52thYF4eoY6WsLFf1WrTaWydcWfvcN"),
        new TestCaseData("000860C220EBBAF591D40F51994C4E2D9C9D88168C33E761F6", "rmJKR4c2hnG7pJQuHZqZjHE4kt2qw1fg6"),
        new TestCaseData("00313E1F905554E7AE2580CD36F86D0C8088382C9E1951C44D010203", "rfCr6gw4cLNnbQ6wGRg8B2LTT1psiKhVGVKuEQ"),
        new TestCaseData("0000000000", "rrrrr"),
        new TestCaseData("1111111111", "pvgLd65"),
        new TestCaseData("FFEEDDCCBBAA", "sUSA4arPP"),
        new TestCaseData("00", "r"),
        new TestCaseData("21", "2"),
    ];

    [Test]
    public void Encode_NullBuffer_ReturnsEmptyString()
    {
        Assert.That(Base58.Ripple.Encode(null), Is.EqualTo(String.Empty));
    }

    [Test]
    [TestCaseSource(nameof(rippleTestData))]
    public void Encode_Ripple_ReturnsExpectedResults(string input, string expectedOutput)
    {
        var buffer = Base16.UpperCase.Decode(input);
        string result = Base58.Ripple.Encode(buffer);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(rippleTestData))]
    public void TryEncode_Ripple_ReturnsExpectedResults(string input, string expectedOutput)
    {
        var inputBuffer = Base16.UpperCase.Decode(input);
        var outputBuffer = new char[Base58.Ripple.GetSafeCharCountForEncoding(inputBuffer)];
        Assert.Multiple(() =>
        {
            Assert.That(Base58.Ripple.TryEncode(inputBuffer, outputBuffer, out int numWritten), Is.True);
            Assert.That(outputBuffer[..numWritten], Is.EqualTo(expectedOutput));
        });
    }

    [Test]
    public void Encode_EmptyBuffer_ReturnsEmptyString()
    {
        Assert.That(Base58.Ripple.Encode([]), Is.EqualTo(String.Empty));
    }

    [Test]
    public void Decode_EmptyString_ReturnsEmptyBuffer()
    {
        var result = Base58.Ripple.Decode(String.Empty);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Decode_InvalidCharacter_Throws()
    {
        _ = Assert.Throws<ArgumentException>(() => Base58.Ripple.Decode("?"));
    }

    [Test]
    [TestCaseSource(nameof(rippleTestData))]
    public void Decode_Ripple_ReturnsExpectedResults(string expectedOutput, string input)
    {
        var buffer = Base58.Ripple.Decode(input);
        string result = BitConverter.ToString(buffer).Replace("-", "",
            StringComparison.Ordinal);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }
}
