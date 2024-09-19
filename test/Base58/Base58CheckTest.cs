using NUnit.Framework;
using SimpleBase;
using System;
using System.Text;

namespace SimpleBaseTest.Base58Test;

[TestFixture]
public class Base58CheckTest
{
    // the Base58Check test cases taken from btcutil package at https://github.com/btcsuite/btcutil
    // ISC License
    
    //Copyright(c) 2013-2017 The btcsuite developers
    //Copyright(c) 2016-2017 The Lightning Network Developers
    
    //Permission to use, copy, modify, and distribute this software for any
    //purpose with or without fee is hereby granted, provided that the above
    //copyright notice and this permission notice appear in all copies.
    
    //THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
    //WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
    //MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
    //ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
    //WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
    //ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
    //OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
    private static readonly object[] testData =
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

    [Test]
    [TestCaseSource(nameof(testData))]
    public void EncodeCheck_ValidInput_ReturnsExpectedResult(int version, string payload, string expectedOutput)
    {
        var bytes = Encoding.ASCII.GetBytes(payload);
        string result = Base58.Bitcoin.EncodeCheck(bytes, (byte)version);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryDecodeCheck_ValidInput_ReturnsExpectedResult(int expectedVersion, string expectedOutput, string input)
    {
        Span<byte> outputBuffer = new byte[256];
        bool result = Base58.Bitcoin.TryDecodeCheck(input, outputBuffer, out byte actualVersion, out int numBytesWritten);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(actualVersion, Is.EqualTo(expectedVersion));
        });
        string output = Encoding.ASCII.GetString(outputBuffer[..numBytesWritten]);
        Assert.That(output, Is.EqualTo(expectedOutput));
    }

}
