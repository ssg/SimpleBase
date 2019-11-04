using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base85Test
{
    [TestFixture]
    class Base85AlphabetTest
    {
        [Test]
        public void GetSafeCharCountForEncoding_Buffer_Works()
        {
            var input = new byte[] { 0, 1, 2, 3 };
            Assert.AreEqual(8, Base85.Ascii85.Alphabet.GetSafeCharCountForEncoding(input));
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 5)]
        [TestCase(2, 6)]
        [TestCase(3, 7)]
        [TestCase(4, 8)]
        [TestCase(5, 10)]
        [TestCase(8, 13)]
        public void GetSafeCharCountForEncoding_Length_Works(int inputLen, int expectedSize)
        {
            Assert.AreEqual(expectedSize, Base85.Ascii85.Alphabet.GetSafeCharCountForEncoding(inputLen));
        }

        [Test]
        public void HasShortcut()
        {
            Assert.IsTrue(Base85.Ascii85.Alphabet.HasShortcut);
            Assert.IsFalse(Base85.Z85.Alphabet.HasShortcut);
        }
    }
}
