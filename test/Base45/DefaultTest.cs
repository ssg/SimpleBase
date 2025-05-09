/*
     Copyright 2014-2025 Sedat Kapanoglu

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
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base45Test;

[TestFixture]
class DefaultTest
{
    // test cases taken from https://datatracker.ietf.org/doc/rfc9285/
    static readonly string[][] testData =
    [
        ["", ""],
        ["AB", "BB8"],
        ["base-45", "UJCLQE7W581"],
        ["ietf!", "QED8WEX0"],
        ["SSG", "1OAQ1"],
        ["SSG1", "1OA009"],
        ["SSG12", "1OA00951"],
        ["SSG123", "1OA009QF6"],
        ["A", "K1"],
    ];

    static readonly string[] invalidEncodingInputs =
    [
        "1",
        "1231",
        "1231231",
        "???",
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_ReturnsCorrectValues(string decoded, string encoded)
    {
        var bytes = Encoding.UTF8.GetBytes(decoded);
        string result = Base45.Default.Encode(bytes);
        Assert.That(result, Is.EqualTo(encoded));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_ReturnsCorrectValues(string decoded, string encoded)
    {
        var bytes = Base45.Default.Decode(encoded);
        var result = Encoding.UTF8.GetString(bytes);
        Assert.That(result, Is.EqualTo(decoded));
    }

    [Test]
    [TestCaseSource(nameof(invalidEncodingInputs))]
    public void Decode_ThrowsOnInvalidEncoding(string invalidInput)
    {
        Assert.Throws<ArgumentException>(() => Base45.Default.Decode(invalidInput));
    }
}
