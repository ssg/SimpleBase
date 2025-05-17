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
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest;

[TestFixture]
class Base2Test
{
    static readonly object[][] testData =
    [
        [new byte[] { }, ""],
        [new byte[] { 0x00, 0x01, 0x02, 0x03 }, "00000000" + "00000001" + "00000010" + "00000011"],
        [new byte[] { 0xFF, 0xFE, 0xFD, 0xFC }, "11111111" + "11111110" + "11111101" + "11111100"],
        [new byte[] { 0x00, 0x01, 0x02, 0x03, 0xFF, 0xFE, 0xFD }, "00000000" + "00000001" + "00000010" + "00000011" + "11111111" + "11111110" + "11111101"]
    ];

    static readonly string[] nonCanonicalInput =
    [
        "1",
        "10",
        "101",
        "1010",
        "101010",
        "1010101",
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_EncodesCorrectly(byte[] decoded, string encoded)
    {
        string result = Base2.Default.Encode(decoded);
        Assert.That(result, Is.EqualTo(encoded));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_DecodesCorrectly(byte[] decoded, string encoded)
    {
        var bytes = Base2.Default.Decode(encoded);
        Assert.That(bytes, Is.EqualTo(decoded));
    }

    [Test]
    [TestCaseSource(nameof(nonCanonicalInput))]
    public void Decode_NonCanonicalInput_Throws(string nonCanonicalText)
    {
        Assert.Throws<ArgumentException>(() => Base2.Default.Decode(nonCanonicalText));
    }
}


