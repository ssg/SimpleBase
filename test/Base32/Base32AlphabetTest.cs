using NUnit.Framework;
using SimpleBase;
using System;

namespace SimpleBaseTest.Base32Test
{
    [TestFixture]
    class Base32AlphabetTest
    {
        [Test]
        public void ctor_NullAlphabet_Throws()
        {
            _ = Assert.Throws<ArgumentNullException>(() => new Base32Alphabet(null));
        }

        [Test]
        public void ctorWithPaddingChar_Works()
        {
            // alphabet characters are unimportant here
            var alpha = new Base32Alphabet("0123456789abcdef0123456789abcdef", '!');
            Assert.That(alpha.PaddingChar, Is.EqualTo('!'));
        }

        [Test]
        public void GetSafeByteCountForDecoding_Works()
        {
            Assert.That(Base32.Crockford.GetSafeByteCountForDecoding("12345"), Is.EqualTo(3));
            Assert.That(Base32.Crockford.GetSafeByteCountForDecoding(""), Is.EqualTo(0));
        }
    }
}
