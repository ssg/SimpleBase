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
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _ = Assert.Throws<ArgumentNullException>(() => new Base32Alphabet(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public void ctorWithPaddingChar_Works()
        {
            // alphabet characters are unimportant here
            var alpha = new Base32Alphabet("0123456789abcdef0123456789abcdef", '!');
            Assert.AreEqual('!', alpha.PaddingChar);
        }

        [Test]
        public void GetSafeByteCountForDecoding_Works()
        {
            Assert.AreEqual(3, Base32.Crockford.GetSafeByteCountForDecoding("12345"));
            Assert.AreEqual(0, Base32.Crockford.GetSafeByteCountForDecoding(""));
        }
    }
}
