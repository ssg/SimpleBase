using NUnit.Framework;
using SimpleBase;
using System;
using System.Text;

namespace SimpleBaseTest.Base32Test;

[TestFixture]
class Bech32Test
{
    // test data was genereated with cryptii.com with a custom alphabet
    private static readonly string[][] testData =
    [
        ["", ""],
        ["f", "vc======"],
        ["fo", "vehs===="],
        ["foo", "vehk7==="],
        ["foob", "vehk7cs="],
        ["fooba", "vehk7cnp"],
        ["foobar", "vehk7cnpwg======"],
        ["foobar1", "vehk7cnpwgcs===="],
        ["foobar12", "vehk7cnpwgcny==="],
        ["foobar123", "vehk7cnpwgcnyvc="],
        ["foobar1234", "vehk7cnpwgcnyve5"],
        ["1234567890123456789012345678901234567890", "xyerxdp4xcmnswfsxyerxdp4xcmnswfsxyerxdp4xcmnswfsxyerxdp4xcmnswfs"],
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_ReturnsExpectedValues(string input, string expectedOutput)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        string result = Base32.Bech32.Encode(bytes, padding: true);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_ReturnsExpectedValues(string expectedOutput, string input)
    {
        var bytes = Base32.Bech32.Decode(input);
        string result = Encoding.ASCII.GetString(bytes);
        Assert.That(result, Is.EqualTo(expectedOutput));
        bytes = Base32.Bech32.Decode(input.ToLowerInvariant());
        result = Encoding.ASCII.GetString(bytes);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Encode_NullBytes_ReturnsEmptyString()
    {
        Assert.That(Base32.Bech32.Encode(null, true), Is.EqualTo(String.Empty));
    }

    [Test]
    public void Decode_InvalidInput_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() => Base32.Bech32.Decode("[];',m."));
    }
}

