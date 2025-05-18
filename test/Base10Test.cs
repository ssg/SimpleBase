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
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest;

[TestFixture]
class Base10Test
{
    static readonly object[][] zeroPrefixedTestData =
    [
        [new byte[] { 0x00, 0x01 }, "01"],
        [new byte[] { 0x00, 0x00, 0xFF }, "00255"],
        [new byte[] { 0x00, 0x01, 0x00 }, "0256"],
    ];

    static readonly object[][] testData =
    [
        [new byte[] { }, ""],
        [new byte[] { 0x01, 0x00 }, "256"],
        [new byte[] { 0x01, 0x00, 0x00 }, "65536"],
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_EncodesCorrectly(byte[] decoded, string encoded)
    {
        string result = Base10.Default.Encode(decoded);
        Assert.That(result, Is.EqualTo(encoded));
    }

    [Test]
    [TestCaseSource(nameof(zeroPrefixedTestData))]
    public void Encode_ZeroPrefixed_EncodesCorrectly(byte[] decoded, string encoded)
    {
        string result = Base10.Default.Encode(decoded);
        Assert.That(result, Is.EqualTo(encoded));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_DecodesCorrectly(byte[] decoded, string encoded)
    {
        var bytes = Base10.Default.Decode(encoded);
        Assert.That(bytes, Is.EqualTo(decoded));
    }
}
