using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base16Test;

// these are just primitive tests just to make sure they at least do what they are supposed to.
// we want to move this functionality out in the next release anyway.
[TestFixture]
class LegacyTest
{
    [Test]
    public void Decode_DecodesBothLowerAndUpperCase()
    {
        var expectedResult = new byte[] { 0xAB, 0xCD, 0xEF, 0xF0 };
        Assert.Multiple(() =>
        {
            Assert.That(Base16.Decode("ABCDEFF0").ToArray(), Is.EqualTo(expectedResult));
            Assert.That(Base16.Decode("abcdeff0").ToArray(), Is.EqualTo(expectedResult));
        });
    }
}
