using NUnit.Framework;
using SimpleBase;
using System;

namespace SimpleBaseTest
{
    [TestFixture]
    class EncodingAlphabetTest
    {
        [Test]
        public void ctor_NullAlphabet_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _ = Assert.Throws<ArgumentNullException>(() => new Base16Alphabet(alphabet: null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public void ToString_ReturnsValue()
        {
            var alpha = new Base16Alphabet("0123456789abcdef");
            Assert.That(alpha.ToString(), Is.EqualTo("0123456789abcdef"));
        }
    }
}
