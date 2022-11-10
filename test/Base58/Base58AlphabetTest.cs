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
using NUnit.Framework;
using SimpleBase;
using System;

namespace SimpleBaseTest.Base58Test;

[TestFixture]
[Parallelizable]
class Base58AlphabetTest
{
    [Test]
    public void Ctor_InvalidLength_Throws()
    {
        _ = Assert.Throws<ArgumentException>(() => new Base58Alphabet("123"));
    }

    [Test]
    public void GetSafeCharCountForEncoding_Works()
    {
        var input = new byte[] { 0, 0, 0, 0, 1, 2, 3, 4 };
        Assert.That(Base58.Bitcoin.GetSafeCharCountForEncoding(input), Is.EqualTo(10));
    }
}
