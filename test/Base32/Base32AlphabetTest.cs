using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base32Test;

[TestFixture]
class Base32AlphabetTest
{
    [Test]
    public void ctorWithPaddingChar_Works()
    {
        // alphabet characters are unimportant here
        var alpha = new Base32Alphabet("0123456789abcdef0123456789abcdef", '!', PaddingPosition.Start);
        Assert.Multiple(() =>
        {
            Assert.That(alpha.PaddingChar, Is.EqualTo('!'));
            Assert.That(alpha.PaddingPosition, Is.EqualTo(PaddingPosition.Start));
        });
    }

    [Test]
    public void GetSafeByteCountForDecoding_Works()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Base32.Crockford.GetSafeByteCountForDecoding("12345"), Is.EqualTo(3));
            Assert.That(Base32.Crockford.GetSafeByteCountForDecoding(""), Is.EqualTo(0));
        });
    }
}
