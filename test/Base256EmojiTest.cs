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

namespace SimpleBaseTest;

[TestFixture]
[Parallelizable]
public class Base256EmojiTest
{
    static readonly string[][] testData =
    [
        ["", ""],
        ["\x00", "🚀"],
        ["\x01", "🪐"],
        ["ÿ", "❓👶"],
        ["\x00yes mani !", "🚀🏃✋🌈😅🌷🤤😻🌟😅👏"],
        ["Hello, World!", "😄✋🍀🍀😓💪😅😑😓🥺🍀😳👏"],
        ["The quick brown fox", "💋😴✋😅✅🤘🌟💃🙃😅👉🥺😓✔😻😅😚😓😣"],
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_EncodesCorrectly(string decoded, string encoded)
    {
        var bytes = Encoding.UTF8.GetBytes(decoded);
        string result = Base256Emoji.Default.Encode(bytes);
        Assert.That(result, Is.EqualTo(encoded));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_DecodesCorrectly(string decoded, string encoded)
    {
        var bytes = Base256Emoji.Default.Decode(encoded);
        string result = Encoding.UTF8.GetString(bytes);
        Assert.That(result, Is.EqualTo(decoded));
    }

    [Test]
    public void Encode_NullBuffer_ReturnsEmptyString()
    {
        Assert.That(Base256Emoji.Default.Encode(null), Is.EqualTo(String.Empty));
    }

    [Test]
    public void Encode_EmptyBuffer_ReturnsEmptyString()
    {
        Assert.That(Base256Emoji.Default.Encode([]), Is.EqualTo(String.Empty));
    }

    [Test]
    public void Decode_EmptyString_ReturnsEmptyBuffer()
    {
        var result = Base256Emoji.Default.Decode(String.Empty);
        Assert.That(result, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryEncode_RegularInput_Succeeds(string decoded, string encoded)
    {
        var input = Encoding.UTF8.GetBytes(decoded);
        int safeLength = Base256Emoji.Default.GetSafeCharCountForEncoding(input);
        var output = new char[safeLength];
        
        Assert.Multiple(() =>
        {
            Assert.That(Base256Emoji.Default.TryEncode(input, output, out int charsWritten), Is.True);
            Assert.That(new string(output[..charsWritten]), Is.EqualTo(encoded));
        });
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryDecode_RegularInput_Succeeds(string decoded, string encoded)
    {
        var expected = Encoding.UTF8.GetBytes(decoded);
        int safeLength = Base256Emoji.Default.GetSafeByteCountForDecoding(encoded);
        var output = new byte[safeLength];
        
        Assert.Multiple(() =>
        {
            Assert.That(Base256Emoji.Default.TryDecode(encoded, output, out int bytesWritten), Is.True);
            Assert.That(output[..bytesWritten], Is.EqualTo(expected));
        });
    }

    [Test]
    public void TryEncode_InsufficientBuffer_ReturnsFalse()
    {
        var input = new byte[] { 1, 2, 3 };
        var output = new char[1]; // Too small for 3 emojis
        
        Assert.Multiple(() =>
        {
            Assert.That(Base256Emoji.Default.TryEncode(input, output, out int charsWritten), Is.False);
            Assert.That(charsWritten, Is.EqualTo(0));
        });
    }

    [Test]
    public void TryDecode_InsufficientBuffer_ReturnsFalse()
    {
        var input = "🚀🪐"; // 2 emojis = 2 bytes
        var output = new byte[1]; // Too small
        
        Assert.Multiple(() =>
        {
            Assert.That(Base256Emoji.Default.TryDecode(input, output, out int bytesWritten), Is.False);
            Assert.That(bytesWritten, Is.EqualTo(1)); // Will decode first emoji before failing
        });
    }

    [Test]
    public void Decode_InvalidEmoji_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() => Base256Emoji.Default.Decode("🚀A")); // A is not an emoji
    }

    [Test]
    public void TryDecode_InvalidEmoji_ReturnsFalse()
    {
        var output = new byte[10];
        Assert.That(Base256Emoji.Default.TryDecode("🚀A", output, out int bytesWritten), Is.False);
        Assert.That(bytesWritten, Is.EqualTo(1)); // Should have decoded the first emoji before failing
    }

    [Test]
    public void Decode_MissingSurrogatePair_ThrowsArgumentException()
    {
        // Create a string with a high surrogate but no low surrogate at the end
        var invalidInput = "🚀" + (char)0xD83D; // High surrogate without low surrogate
        
        _ = Assert.Throws<ArgumentException>(() => Base256Emoji.Default.Decode(invalidInput));
    }

    [Test]
    public void Decode_UnexpectedLowSurrogate_ThrowsArgumentException()
    {
        // Create a string with a low surrogate at the beginning
        var invalidInput = (char)0xDE00 + "🚀"; // Low surrogate without high surrogate
        
        _ = Assert.Throws<ArgumentException>(() => Base256Emoji.Default.Decode(invalidInput));
    }

    [Test]
    public void GetSafeCharCountForEncoding_ReturnsCorrectEstimate()
    {
        var input = new byte[10];
        int result = Base256Emoji.Default.GetSafeCharCountForEncoding(input);
        Assert.That(result, Is.EqualTo(20)); // 2 chars per byte (safe overestimate)
    }

    [Test]
    public void GetSafeByteCountForDecoding_ReturnsCorrectEstimate()
    {
        var input = "🚀🪐☄🛰🌌"; // 5 emojis
        int result = Base256Emoji.Default.GetSafeByteCountForDecoding(input);
        Assert.That(result, Is.GreaterThanOrEqualTo(5)); // At least as many bytes as emojis
    }

    [Test]
    public void Constructor_InvalidAlphabetLength_ThrowsArgumentException()
    {
        var shortAlphabet = new string[255]; // Should be 256
        _ = Assert.Throws<ArgumentException>(() => new Base256Emoji(shortAlphabet));
    }

    [Test]
    public void Constructor_CustomAlphabet_WorksCorrectly()
    {
        // Create a custom alphabet with simple characters for easier testing
        var customAlphabet = new string[256];
        for (int i = 0; i < 256; i++)
        {
            customAlphabet[i] = ((char)(i + 0x1F600)).ToString(); // Use emoji range
        }
        
        var encoder = new Base256Emoji(customAlphabet);
        var input = new byte[] { 0, 1, 255 };
        var encoded = encoder.Encode(input);
        var decoded = encoder.Decode(encoded);
        
        Assert.That(decoded, Is.EqualTo(input));
    }

    [Test]
    public void RoundTrip_AllBytes_WorksCorrectly()
    {
        // Test all possible byte values
        var input = new byte[256];
        for (int i = 0; i < 256; i++)
        {
            input[i] = (byte)i;
        }
        
        var encoded = Base256Emoji.Default.Encode(input);
        var decoded = Base256Emoji.Default.Decode(encoded);
        
        Assert.That(decoded, Is.EqualTo(input));
    }

    [Test]
    public void RoundTrip_RandomData_WorksCorrectly()
    {
        var random = new Random(42); // Fixed seed for reproducible tests
        var input = new byte[100];
        random.NextBytes(input);
        
        var encoded = Base256Emoji.Default.Encode(input);
        var decoded = Base256Emoji.Default.Decode(encoded);
        
        Assert.That(decoded, Is.EqualTo(input));
    }

    [Test]
    public void Encode_LargeData_WorksCorrectly()
    {
        var input = new byte[1000];
        for (int i = 0; i < input.Length; i++)
        {
            input[i] = (byte)(i % 256);
        }
        
        var encoded = Base256Emoji.Default.Encode(input);
        var decoded = Base256Emoji.Default.Decode(encoded);
        
        Assert.That(decoded, Is.EqualTo(input));
    }

    [Test]
    public void TryEncode_EmptyInput_Succeeds()
    {
        var input = Array.Empty<byte>();
        var output = new char[10];
        
        Assert.Multiple(() =>
        {
            Assert.That(Base256Emoji.Default.TryEncode(input, output, out int charsWritten), Is.True);
            Assert.That(charsWritten, Is.EqualTo(0));
        });
    }

    [Test]
    public void TryDecode_EmptyInput_Succeeds()
    {
        var input = "";
        var output = new byte[10];
        
        Assert.Multiple(() =>
        {
            Assert.That(Base256Emoji.Default.TryDecode(input, output, out int bytesWritten), Is.True);
            Assert.That(bytesWritten, Is.EqualTo(0));
        });
    }

    static readonly object[][] unicodeTestData =
    [
        ["🚀", new byte[] { 0 }],           // First emoji
        ["🥂", new byte[] { 255 }],         // Last emoji  
        ["🚀🪐", new byte[] { 0, 1 }],       // First two emojis
        ["📣🥂", new byte[] { 254, 255 }],   // Last two emojis
    ];

    [Test]
    [TestCaseSource(nameof(unicodeTestData))]
    public void Encode_SpecificUnicodeValues_ReturnsExpectedResults(string expectedOutput, byte[] input)
    {
        string result = Base256Emoji.Default.Encode(input);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(unicodeTestData))]
    public void Decode_SpecificUnicodeValues_ReturnsExpectedResults(string input, byte[] expectedOutput)
    {
        var result = Base256Emoji.Default.Decode(input);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }
}
