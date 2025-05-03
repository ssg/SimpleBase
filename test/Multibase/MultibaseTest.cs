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
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.MultibaseTest;

[TestFixture]
class MultibaseTest
{
    static readonly MultibaseEncoding[] multibaseEncodings = Enum.GetValues<MultibaseEncoding>();

    static readonly byte[] testBuffer = [1, 2, 3, 4, 5];

    [Test]
    [TestCaseSource(nameof(multibaseEncodings))]
    public void Encode_PrependsBufferWithCorrectCharacter(MultibaseEncoding encoding)
    {
        string result = Multibase.Encode(testBuffer, encoding);
        Assert.That(result, Does.StartWith(((char)encoding).ToString()));
    }

    [Test]
    public void Encode_InvalidEncoding_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Multibase.Encode(testBuffer, (MultibaseEncoding)0));
    }
}
