using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest;

[TestFixture]
class EncodingAlphabetTest
{
    [Test]
    public void ToString_ReturnsValue()
    {
        var alpha = new Base16Alphabet("0123456789abcdef");
        Assert.That(alpha.ToString(), Is.EqualTo("0123456789abcdef"));
    }
}
