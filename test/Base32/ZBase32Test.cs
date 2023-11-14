/*
     Copyright 2014-2016 Sedat Kapanoglu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using System;
using System.Text;
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base32Test;

[TestFixture]
class ZBase32Test
{
    private static readonly string[][] testData =
    [
        ["", ""],
        ["dCode z-base-32", "ctbs63dfrb7n4aubqp114c31"],
        [ "Never did sun more beautifully steep",
            "j31zc3m1rb1g13byqp4shedpp73gkednciozk7djc34sa5d3rb3ze3mfqy" ],
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_ReturnsExpectedValues(string input, string expectedOutput)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        string result = Base32.ZBase32.Encode(bytes, padding: false);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_ReturnsExpectedValues(string expectedOutput, string input)
    {
        var bytes = Base32.ZBase32.Decode(input);
        string result = Encoding.ASCII.GetString(bytes);
        Assert.That(result, Is.EqualTo(expectedOutput));
        bytes = Base32.ZBase32.Decode(input.ToLowerInvariant());
        result = Encoding.ASCII.GetString(bytes);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Encode_NullBytes_ReturnsEmptyString()
    {
        Assert.That(Base32.ZBase32.Encode(null, padding: false), Is.EqualTo(String.Empty));
    }

    [Test]
    public void Decode_InvalidInput_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() => Base32.ZBase32.Decode("[];',m."));
    }
}
