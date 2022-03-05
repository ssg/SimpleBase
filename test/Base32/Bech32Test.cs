using NUnit.Framework;
using SimpleBase;
using System;
using System.Text;

namespace SimpleBaseTest.Base32Test;

[TestFixture]
class Bech32Test
{
    // test data was genereated with cryptii.com with a custom alphabet
    private static readonly string[][] testData = new[]
    {
        new[] { "", "" },
        new[] {"f", "vc======" },
        new[] {"fo", "vehs====" },
        new[] {"foo", "vehk7===" },
        new[] {"foob", "vehk7cs=" },
        new[] {"fooba", "vehk7cnp" },
        new[] {"foobar", "vehk7cnpwg======" },
        new[] {"foobar1", "vehk7cnpwgcs====" },
        new[] {"foobar12", "vehk7cnpwgcny===" },
        new[] {"foobar123", "vehk7cnpwgcnyvc=" },
        new[] {"foobar1234", "vehk7cnpwgcnyve5" },
        new[] {"1234567890123456789012345678901234567890", "xyerxdp4xcmnswfsxyerxdp4xcmnswfsxyerxdp4xcmnswfsxyerxdp4xcmnswfs" },
    };

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
        string result = Encoding.ASCII.GetString(bytes.ToArray());
        Assert.That(result, Is.EqualTo(expectedOutput));
        bytes = Base32.Bech32.Decode(input.ToLowerInvariant());
        result = Encoding.ASCII.GetString(bytes.ToArray());
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

