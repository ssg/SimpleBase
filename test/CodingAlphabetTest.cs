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
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest;

[TestFixture]
class CodingAlphabetTest
{
    class DummyAlphabet(string alphabet) : CodingAlphabet(10, alphabet, caseInsensitive: true)
    {
    }

    [Test]
    public void Ctor_WithBothCasesOfLettersAndCaseInsensitive_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new DummyAlphabet("01234567Aa"));
    }

    [Test]
    public void Ctor_LengthAndAlphabetLengthMismatch_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new DummyAlphabet("01234567a"));
    }

    [Test]
    public void Ctor_ProperArguments_ShouldNotThrow()
    {
        Assert.DoesNotThrow(() => new DummyAlphabet("01234567ab"));
    }
}
