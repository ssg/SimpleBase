using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base32Test;

[TestFixture]
class GeohashTest
{
    [Test]
    public void Decode_SmokeTest()
    {
        const string input = "ezs42";
        var result = Base32.Geohash.Decode(input);
        var expected = new byte[] { 0b01101111, 0b11110000, 0b01000001 };
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Encode_SmokeTest()
    {
        const string expected = "ezs42";
        var input = new byte[] { 0b01101111, 0b11110000, 0b01000001 };
        var result = Base32.Geohash.Encode(input);
        Assert.That(result, Is.EqualTo(expected));
    }
}
