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
        Assert.That(result[0], Is.EqualTo((char)encoding));
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
        [MultibaseEncoding.Base36Lower, "k2p81m7y66k71a7teel1hfa4ldp6bneq0hj9b"],
        [MultibaseEncoding.Base36Upper, "K2P81M7Y66K71A7TEEL1HFA4LDP6BNEQ0HJ9B"],
        [MultibaseEncoding.Base32Z, "hkpjwqenzefj1y1nfkjn1yejbakih8dqdu5b56"],
        [MultibaseEncoding.Base45, "R1OAS:8H1B+MA6691IAZ242C46WLL-H83KB4"],
        [MultibaseEncoding.Base58Flickr, "Z2HPxwKnQi8s2ugkrzPrrR6nyDEpMhoe6"],
        [MultibaseEncoding.Base58Bitcoin, "z2ipYXkNqJ8T2VGLSapSSr6NZefQnHPE6"],
        [MultibaseEncoding.Base64, "mU1NHIFdBUyBIRVJFICEhwqvDjcOew78"],
        [MultibaseEncoding.Base64Pad, "MU1NHIFdBUyBIRVJFICEhwqvDjcOew78="],
        [MultibaseEncoding.Base64Url, "uU1NHIFdBUyBIRVJFICEhwqvDjcOew78"],
        [MultibaseEncoding.Base64UrlPad, "UU1NHIFdBUyBIRVJFICEhwqvDjcOew78="],
    ];

    // These vectors are taken from https://github.com/multiformats/multibase/tree/master/tests 
    static readonly byte[] officialEncodingInput = Encoding.UTF8.GetBytes("yes mani !");

    static readonly object[][] officialEncodingData =
    [
        [MultibaseEncoding.Base2, "001111001011001010111001100100000011011010110000101101110011010010010000000100001"],
        [MultibaseEncoding.Base8, "7362625631006654133464440102"],
        [MultibaseEncoding.Base10, "9573277761329450583662625"],
        [MultibaseEncoding.Base16Lower, "f796573206d616e692021"],
        [MultibaseEncoding.Base16Upper, "F796573206D616E692021"],
        [MultibaseEncoding.Base32Lower, "bpfsxgidnmfxgsibb"],
        [MultibaseEncoding.Base32Upper, "BPFSXGIDNMFXGSIBB"],
        [MultibaseEncoding.Base32HexLower, "vf5in683dc5n6i811"],
        [MultibaseEncoding.Base32HexUpper, "VF5IN683DC5N6I811"],
        [MultibaseEncoding.Base32Z, "hxf1zgedpcfzg1ebb"],
        [MultibaseEncoding.Base36Lower, "k2lcpzo5yikidynfl"],
        [MultibaseEncoding.Base36Upper, "K2LCPZO5YIKIDYNFL"],
        [MultibaseEncoding.Base58Flickr, "Z7Pznk19XTTzBtx"],
        [MultibaseEncoding.Base58Bitcoin, "z7paNL19xttacUY"],
        [MultibaseEncoding.Base64, "meWVzIG1hbmkgIQ"],
        [MultibaseEncoding.Base64Pad, "MeWVzIG1hbmkgIQ=="],
        [MultibaseEncoding.Base64Url, "ueWVzIG1hbmkgIQ"],
        [MultibaseEncoding.Base64UrlPad, "UeWVzIG1hbmkgIQ=="],
        [MultibaseEncoding.Base256Emoji, "🚀🏃✋🌈😅🌷🤤😻🌟😅👏"],
    ];

    static readonly byte[] officialZeroPrefixedEncodingInput = Encoding.UTF8.GetBytes("\x00yes mani !");

    static readonly object[][] officialZeroPrefixedEncodingData =
    [
        [MultibaseEncoding.Base2, "00000000001111001011001010111001100100000011011010110000101101110011010010010000000100001"],
        [MultibaseEncoding.Base8, "7000745453462015530267151100204"],
        [MultibaseEncoding.Base10, "90573277761329450583662625"],
        [MultibaseEncoding.Base16Lower, "f00796573206d616e692021"],
        [MultibaseEncoding.Base16Upper, "F00796573206D616E692021"],
        [MultibaseEncoding.Base32Lower, "bab4wk4zanvqw42jaee"],
        [MultibaseEncoding.Base32Upper, "BAB4WK4ZANVQW42JAEE"],
        [MultibaseEncoding.Base32HexLower, "v01smasp0dlgmsq9044"],
        [MultibaseEncoding.Base32HexUpper, "V01SMASP0DLGMSQ9044"],
        //[MultibaseEncoding.Base32pad, "cab4wk4zanvqw42jaee======"],
        //[MultibaseEncoding.Base32padupper, "CAB4WK4ZANVQW42JAEE======"],
        //[MultibaseEncoding.Base32hexpad, "t01smasp0dlgmsq9044======"],
        //[MultibaseEncoding.Base32hexpadupper, "T01SMASP0DLGMSQ9044======"],
        [MultibaseEncoding.Base32Z, "hybhskh3ypiosh4jyrr"],
        [MultibaseEncoding.Base36Lower, "k02lcpzo5yikidynfl"],
        [MultibaseEncoding.Base36Upper, "K02LCPZO5YIKIDYNFL"],
        [MultibaseEncoding.Base58Flickr, "Z17Pznk19XTTzBtx"],
        [MultibaseEncoding.Base58Bitcoin, "z17paNL19xttacUY"],
        [MultibaseEncoding.Base64, "mAHllcyBtYW5pICE"],
        [MultibaseEncoding.Base64Pad, "MAHllcyBtYW5pICE="],
        [MultibaseEncoding.Base64Url, "uAHllcyBtYW5pICE"],
        [MultibaseEncoding.Base64UrlPad, "UAHllcyBtYW5pICE="],
        [MultibaseEncoding.Base256Emoji, "🚀🚀🏃✋🌈😅🌷🤤😻🌟😅👏"],
    ];

    const string caseInsensitiveInput = "hello world";

    static readonly object[][] caseInsensitiveDecodingData =
    [
        [MultibaseEncoding.Base16Lower, "f68656c6c6f20776F726C64"],
        [MultibaseEncoding.Base16Upper, "F68656c6c6f20776F726C64"],
        [MultibaseEncoding.Base32Lower, "bnbswy3dpeB3W64TMMQ"],
        [MultibaseEncoding.Base32Upper, "Bnbswy3dpeB3W64TMMQ"],
        //MultibaseEncoding.Base32Hex, "vd1imor3f41RMUSJCCG"],
        [MultibaseEncoding.Base32HexUpper, "Vd1imor3f41RMUSJCCG"],
        //MultibaseEncoding.Base32Pad, "cnbswy3dpeB3W64TMMQ======"],
        //MultibaseEncoding.Base32PadUpper, "Cnbswy3dpeB3W64TMMQ======"],
        //MultibaseEncoding.Base32HexPad, "td1imor3f41RMUSJCCG======"],
        //MultibaseEncoding.Base32HexPadUpper, "Td1imor3f41RMUSJCCG======"],
        [MultibaseEncoding.Base36Lower, "kfUvrsIvVnfRbjWaJo"],
        [MultibaseEncoding.Base36Upper, "KfUVrSIVVnFRbJWAJo"]
    ];

    [Test]
    [TestCaseSource(nameof(encodedData))]
    public void Encode_EncodesDataCorrectly(MultibaseEncoding encoding, string expected)
    {
        string encoded = Multibase.Encode(encodingInput, encoding);
        Assert.That(encoded, Is.EqualTo(expected));
    }

    [Test]
    [TestCaseSource(nameof(officialEncodingData))]
    public void Encode_OfficialEncodingData_EncodesDataCorrectly(MultibaseEncoding encoding, string expected)
    {
        string encoded = Multibase.Encode(officialEncodingInput, encoding);
        Assert.That(encoded, Is.EqualTo(expected));
    }

    [Test]
    [TestCaseSource(nameof(officialZeroPrefixedEncodingData))]
    public void Encode_OfficialZeroPrefixedEncodingData_EncodesDataCorrectly(MultibaseEncoding encoding, string expected)
    {
        string encoded = Multibase.Encode(officialZeroPrefixedEncodingInput, encoding);
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

    [Test]
    [TestCaseSource(nameof(caseInsensitiveDecodingData))]
    public void Decode_MixedCaseInput_DecodesCorrectly(MultibaseEncoding _, string encoded)
    {
        byte[] decoded = Multibase.Decode(encoded);
        Assert.That(decoded, Is.EqualTo(Encoding.UTF8.GetBytes(caseInsensitiveInput)));
    }
}
