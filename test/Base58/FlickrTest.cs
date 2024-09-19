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
class FlickrTest
{
    private static readonly TestCaseData[] flickrTestData =
    [
        new TestCaseData("0000010203", "11kCP"),
        new TestCaseData("009C1CA2CBA6422D3988C735BB82B5C880B0441856B9B0910F", "1ferHzT4xPnDNxGv3kP7Sv1s6vYCBv7VBe"),
        new TestCaseData("000860C220EBBAF591D40F51994C4E2D9C9D88168C33E761F6", "1LijqnBz45gt2ipUhyQyJhfnKTzQaS7FG"),
        new TestCaseData("00313E1F905554E7AE2580CD36F86D0C8088382C9E1951C44D010203", "17E1GFanBke5ApGagqFMbzkssS23Rj4ugujUfp"),
        new TestCaseData("0000000000", "11111"),
        new TestCaseData("1111111111", "2VFkCGH"),
        new TestCaseData("FFEEDDCCBBAA", "3crWn61oo"),
        new TestCaseData("00", "1"),
        new TestCaseData("21", "z"),
    ];

    [Test]
    public void Encode_NullBuffer_ReturnsEmptyString()
    {
        Assert.That(Base58.Flickr.Encode(null), Is.EqualTo(String.Empty));
    }

    [Test]
    [TestCaseSource(nameof(flickrTestData))]
    public void Encode_Flickr_ReturnsExpectedResults(string input, string expectedOutput)
    {
        var buffer = Base16.UpperCase.Decode(input);
        string result = Base58.Flickr.Encode(buffer);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(flickrTestData))]
    public void TryEncode_Flickr_ReturnsExpectedResults(string input, string expectedOutput)
    {
        var inputBuffer = Base16.UpperCase.Decode(input);
        var outputBuffer = new char[Base58.Flickr.GetSafeCharCountForEncoding(inputBuffer)];
        Assert.Multiple(() =>
        {
            Assert.That(Base58.Flickr.TryEncode(inputBuffer, outputBuffer, out int numWritten), Is.True);
            Assert.That(outputBuffer[..numWritten], Is.EqualTo(expectedOutput));
        });
    }

    [Test]
    public void Encode_EmptyBuffer_ReturnsEmptyString()
    {
        Assert.That(Base58.Flickr.Encode([]), Is.EqualTo(String.Empty));
    }

    [Test]
    public void Decode_EmptyString_ReturnsEmptyBuffer()
    {
        var result = Base58.Flickr.Decode(String.Empty);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Decode_InvalidCharacter_Throws()
    {
        _ = Assert.Throws<ArgumentException>(() => Base58.Flickr.Decode("?"));
    }

    [Test]
    [TestCaseSource(nameof(flickrTestData))]
    public void Decode_Flickr_ReturnsExpectedResults(string expectedOutput, string input)
    {
        var buffer = Base58.Flickr.Decode(input);
        string result = BitConverter.ToString(buffer).Replace("-", "",
            StringComparison.Ordinal);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }
}
