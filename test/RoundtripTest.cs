using System;
using System.Diagnostics;
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest;

[TestFixture]
public class RoundtripTest
{
    [OneTimeSetUp]
    public void Setup()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        Trace.Flush();
    }

    const int maxBufferLength = 256;
    static readonly byte[] buffer = new byte[maxBufferLength];

    [OneTimeSetUp]
    public void InitializeBuffer()
    {
        buffer[0] = 1;
    }

    static readonly IBaseCoder[] encoders =
    [
        Base2.Default,
        Base8.Default,
        Base10.Default,

        Base16.UpperCase,
        Base16.LowerCase,

        Base32.Rfc4648,
        Base32.Geohash,
        Base32.FileCoin,
        Base32.Crockford,
        Base32.Bech32,

        Base36.UpperCase,
        Base36.LowerCase,

        Base45.Default,

        Base58.Bitcoin,
        Base58.Flickr,
        Base58.Monero,
        Base58.Ripple,

        Base62.Default,
        Base62.LowerFirst,

        Base85.Ascii85,
        Base85.Rfc1924,
        Base85.Z85,

        Base256Emoji.Default,
    ];

    [Test]
    public void Decode_DecodesToTheOriginalEncodedValue([Range(1, maxBufferLength)]int length, 
        [ValueSource(nameof(encoders))]IBaseCoder coder)
    {
        var input = buffer[..length];
        string encodedText = coder.Encode(input);
        var result = coder.Decode(encodedText);
        Assert.That(result, Is.EquivalentTo(input));
    }
}
