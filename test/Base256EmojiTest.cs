using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest;

[TestFixture]
public class Base256EmojiTest
{
    static readonly string[][] testData =
    [
        ["\x00yes mani !", "🚀🏃✋🌈😅🌷🤤😻🌟😅👏"],
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

}
