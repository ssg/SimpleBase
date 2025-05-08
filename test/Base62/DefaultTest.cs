/*
     Copyright 2014-2025 Sedat Kapanoglu

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

namespace SimpleBaseTest.Base62Test;

[TestFixture]
class DefaultTest
{
    static readonly string[][] testData = [
        ["", ""],
        ["SSG WAS HERE!", "2ETo47rrJdrFdqI4CP"],
        ["A quick brown fox jumps over the lazy dog", "MbW36N4wUwiF8w630WywYtgnrGqMKAxpYKQRT90ZlD5pv9LLGP4wHgd"],
        ["A", "13"],
        ["AA", "4LR"],
        ["AAA", "HwWX"],
        ["abc", "QmIN"],
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_ReturnsCorrectValues(string input, string expected)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var encoded = Base62.Default.Encode(bytes);
        Assert.That(encoded, Is.EqualTo(expected));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_ReturnsCorrectValues(string decoded, string encoded)
    {
        var result = Base62.Default.Decode(encoded);
        Assert.That(result, Is.EqualTo(decoded));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryDecode_ReturnsCorrectValues(string decoded, string encoded)
    {
        Span<byte> output = stackalloc byte[100];
        var result = Base62.Default.TryDecode(encoded, output, out int bytesWritten);
        Assert.That(result, Is.True);
        Assert.That(Encoding.UTF8.GetString(output[..bytesWritten]), Is.EqualTo(decoded));
    }

}
