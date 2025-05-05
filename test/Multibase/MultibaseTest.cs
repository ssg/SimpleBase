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
using System.Buffers.Text;
using System.Text;
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.MultibaseTest;

[TestFixture]
class MultibaseTest
{
    static readonly MultibaseEncoding[] multibaseEncodings = Enum.GetValues<MultibaseEncoding>();

    static readonly byte[] testBuffer = [1, 2, 3, 4, 5];

    [Test]
    [TestCaseSource(nameof(multibaseEncodings))]
    public void Encode_PrependsBufferWithCorrectCharacter(MultibaseEncoding encoding)
    {
        string result = Multibase.Encode(testBuffer, encoding);
        Assert.That(result, Does.StartWith(((char)encoding).ToString()));
    }

    [Test]
    public void Encode_InvalidEncoding_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Multibase.Encode(testBuffer, 0));
    }

    static readonly byte[] encodingInput = Encoding.UTF8.GetBytes("SSG WAS HERE !!\xAB\xCD\xDE\xFF");

    static readonly object[][] encodedData =
    [
        [MultibaseEncoding.Base16Lower, "f535347205741532048455245202121c2abc38dc39ec3bf"],
        [MultibaseEncoding.Base16Upper, "F535347205741532048455245202121C2ABC38DC39EC3BF"],
        [MultibaseEncoding.Base32Lower, "bknjuoicxifjsascfkjcsaijbykv4hdodt3b36"],
        [MultibaseEncoding.Base32Upper, "BKNJUOICXIFJSASCFKJCSAIJBYKV4HDODT3B36"],
        [MultibaseEncoding.Base32HexLower, "vad9ke82n859i0i25a92i0891oals73e3jr1ru"],
        [MultibaseEncoding.Base32HexUpper, "VAD9KE82N859I0I25A92I0891OALS73E3JR1RU"],
        [MultibaseEncoding.Base32Z, "hkpjwqenzefj1y1nfkjn1yejbakih8dqdu5b56"],
        [MultibaseEncoding.Base58Flickr, "Z2HPxwKnQi8s2ugkrzPrrR6nyDEpMhoe6"],
        [MultibaseEncoding.Base58Bitcoin, "z2ipYXkNqJ8T2VGLSapSSr6NZefQnHPE6"],
        [MultibaseEncoding.Base64, "mU1NHIFdBUyBIRVJFICEhwqvDjcOew78="],
        [MultibaseEncoding.Base64Url, "uU1NHIFdBUyBIRVJFICEhwqvDjcOew78"],
        [MultibaseEncoding.Base64UrlPad, "UU1NHIFdBUyBIRVJFICEhwqvDjcOew78="],
    ];

    [Test]
    [TestCaseSource(nameof(encodedData))]
    public void Encode_EncodesDataCorrectly(MultibaseEncoding encoding, string expected)
    {
        string encoded = Multibase.Encode(encodingInput, encoding);
        Assert.That(encoded, Is.EqualTo(expected));
    }

    [Test]
    [TestCaseSource(nameof(multibaseEncodings))]
    public void Encode_EncodedDataDecodesBack(MultibaseEncoding encoding)
    {
        string encoded = Multibase.Encode(encodingInput, encoding);
        byte[] decoded = Multibase.Decode(encoded);
        Assert.That(decoded, Is.EqualTo(encodingInput));
    }

    [Test]
    [TestCaseSource(nameof(encodedData))]
    public void TryDecode_DecodesCorrectly(MultibaseEncoding _, string encoded)
    {
        byte[] bytes = new byte[1024]; // big enough for decoding
        Assert.That(Multibase.TryDecode(encoded, bytes, out int _), Is.True);
    }

    [Test]
    public void Decode_EmptyString_Throws()
    {
        Assert.Throws<ArgumentException>(() => Multibase.Decode(string.Empty));
    }

    [Test]
    public void TryDecode_EmptyString_ReturnsFalse()
    {
        Assert.That(Multibase.TryDecode(string.Empty, new byte[1], out _), Is.False);
    }
}
