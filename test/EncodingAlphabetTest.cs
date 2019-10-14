using NUnit.Framework;
using SimpleBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleBaseTest
{
    [TestFixture]
    class EncodingAlphabetTest
    {
        [Test]
        public void ctor_NullAlphabet_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => new Base16Alphabet(alphabet: null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
}
