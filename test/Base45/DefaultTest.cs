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
using System.IO;
using System.Text;
using System.Threading.Tasks;
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

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_Stream_EncodesCorrectly(string decoded, string encoded)
    {
        var bytes = Encoding.UTF8.GetBytes(decoded);
        using var input = new MemoryStream(bytes);
        using var output = new StringWriter();
        Base45.Default.Encode(input, output);
        string result = output.ToString();
        Assert.That(result, Is.EqualTo(encoded));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_Stream_DecodesCorrectly(string decoded, string encoded)
    {
        using var input = new StringReader(encoded);
        using var output = new MemoryStream();
        Base45.Default.Decode(input, output);
        string result = Encoding.UTF8.GetString(output.ToArray());
        Assert.That(result, Is.EqualTo(decoded));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public async Task EncodeAsync_EncodesCorrectly(string decoded, string encoded)
    {
        var bytes = Encoding.UTF8.GetBytes(decoded);
        using var input = new MemoryStream(bytes);
        using var output = new StringWriter();
        await Base45.Default.EncodeAsync(input, output);
        string result = output.ToString();
        Assert.That(result, Is.EqualTo(encoded));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public async Task DecodeAsync_DecodesCorrectly(string decoded, string encoded)
    {
        using var input = new StringReader(encoded);
        using var output = new MemoryStream();
        await Base45.Default.DecodeAsync(input, output);
        string result = Encoding.UTF8.GetString(output.ToArray());
        Assert.That(result, Is.EqualTo(decoded));
    }
}
