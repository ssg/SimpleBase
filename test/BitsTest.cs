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
using NUnit.Framework;

namespace SimpleBaseTest;

[TestFixture]
class BitsTest
{
    static readonly object[][] testData =
    [
        [ new byte[] { 0x00, 0x00, 0x01 }, 2 ],
        [ new byte[] { 0x00, 0x01, 0x00 }, 1 ],
        [ new byte[] { 0x01, 0x00, 0x00 }, 0 ],
        [ new byte[] { }, 0 ],
        [ new byte[] { 0x00, 0x00, 0x00 }, 3 ],
        [ new byte[] { 0x00, 0x00, 0x00, 0x01 }, 3 ],
        [ new byte[] { 0xFF, 0xFF, 0xFF }, 0 ],
        [ new byte[] { 0x00, 0x00, 0x00, 0x00 }, 4 ],
    ];

    [Test]
    [TestCaseSource(nameof(testData))]
    public void CountPrefixingZeroes_CountsCorrectly(byte[] bytes, int expectedCount)
    {
        int count = SimpleBase.Bits.CountPrefixingZeroes(bytes);
        Assert.That(count, Is.EqualTo(expectedCount));
    }
}
