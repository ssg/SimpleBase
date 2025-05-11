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
using System.Text;
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base36Test;

[TestFixture]
class LowerCaseTest
{
    static readonly string[][] testData =
    [
        ["", ""],
        ["a", "2p"],
        ["test", "wanek4"],
        ["hello world", "fuvrsivvnfrbjwajo"]
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_EncodesCorrectly(string decoded, string encoded)
    {
        var bytes = Encoding.UTF8.GetBytes(decoded);
        string result = Base36.LowerCase.Encode(bytes);
        Assert.That(result, Is.EqualTo(encoded));
    }
}
