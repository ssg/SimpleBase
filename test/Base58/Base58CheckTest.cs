using NUnit.Framework;
using SimpleBase;
using System;
using System.Text;

namespace SimpleBaseTest.Base58Test;

[TestFixture]
public class Base58CheckTest
{
    // the Base58Check test cases taken from btcutil package at https://github.com/btcsuite/btcd/tree/master/btcutil
    // see LICENSE.btcd.txt for details
    static readonly object[] testData =
    [
        new object[] { 20, "", "3MNQE1X"},
        new object[] { 20, " ", "B2Kr6dBE"},
        new object[] { 20, "-", "B3jv1Aft"},
        new object[] { 20, "0", "B482yuaX"},
        new object[] { 20, "1", "B4CmeGAC"},
        new object[] { 20, "-1", "mM7eUf6kB"},
        new object[] { 20, "11", "mP7BMTDVH"},
        new object[] { 20, "abc", "4QiVtDjUdeq"},
        new object[] { 20, "1234598760", "ZmNb8uQn5zvnUohNCEPP"},
        new object[] { 20, "abcdefghijklmnopqrstuvwxyz", "K2RYDcKfupxwXdWhSAxQPCeiULntKm63UXyx5MvEH2"},
        new object[] { 20, "00000000000000000000000000000000000000000000000000000000000000", "bi1EWXwJay2udZVxLJozuTb8Meg4W9c6xnmJaRDjg6pri5MBAxb9XwrpQXbtnqEoRV5U2pixnFfwyXC8tRAVC8XxnjK"},
    ];

    // test vector taken from gotezos at https://github.com/goat-systems/go-tezos/blob/800cc714fad7313e92a5068407c23e0e397f5323/keys/key_test.go#L127
    // see LICENSE.gotezos.txt for details
    static object[][] zeroPrefixedLongVersionTestData = [
        [ "0000861299624c9a3b52be10762c64bac282b1c02316", new byte[] { 6, 161, 159 }, "tz1XrwX7i9Nzh8e6UmG3VnFkAeoyWdTqDf3U" ],
    ];

    static object[][] longVersionTestData = [
        [ "861299624c9a3b52be10762c64bac282b1c02316", new byte[] { 6, 161, 159 }, "tz1XrwX7i9Nzh8e6UmG3VnFkAeoyWdTqDf3U" ],
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void EncodeCheck_ValidInput_ReturnsExpectedResult(int version, string payload, string expectedOutput)
    {
        var bytes = Encoding.ASCII.GetBytes(payload);
        string result = Base58.Bitcoin.EncodeCheck(bytes, (byte)version);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(zeroPrefixedLongVersionTestData))]
    public void EncodeCheckSkipZeroes_ZeroPrefixes_ReturnsExpectedResult(string hexPayload, byte[] version, string expectedResult)
    {
        var payload = Convert.FromHexString(hexPayload);
        string result = Base58.Bitcoin.EncodeCheckSkipZeroes(payload, version);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCaseSource(nameof(zeroPrefixedLongVersionTestData))]
    public void TryDecodeCheck_ZeroPrefixedLongVersion_ValidInput_ReturnsExpectedResult(string expectedHexPayload, byte[] expectedVersion, string encoded)
    {
        var versionBuffer = new byte[expectedVersion.Length];
        var outputBuffer = new byte[256];
        bool result = Base58.Bitcoin.TryDecodeCheck(encoded, outputBuffer, versionBuffer, out int bytesWritten);
        Assert.That(result, Is.True);
        Assert.That(versionBuffer, Is.EquivalentTo(expectedVersion));
        var expectedBuffer = Convert.FromHexString(expectedHexPayload.TrimStart('0'));
        Assert.That(outputBuffer.AsSpan(0, bytesWritten).ToArray(), Is.EquivalentTo(expectedBuffer));
    }

    [Test]
    [TestCaseSource(nameof(longVersionTestData))]
    public void EncodeCheck_LongVersionOverload_DoesNotStripPrefixZeroes(string hexPayload, byte[] version, string expectedResult)
    {
        var payload = Convert.FromHexString(hexPayload);
        string result = Base58.Bitcoin.EncodeCheck(payload, version);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryDecodeCheck_ValidInput_ReturnsExpectedResult(int expectedVersion, string expectedOutput, string input)
    {
        Span<byte> outputBuffer = new byte[256];
        bool result = Base58.Bitcoin.TryDecodeCheck(input, outputBuffer, out byte actualVersion, out int bytesWritten);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(actualVersion, Is.EqualTo(expectedVersion));
        });
        string output = Encoding.ASCII.GetString(outputBuffer[..bytesWritten]);
        Assert.That(output, Is.EqualTo(expectedOutput));
    }

}
