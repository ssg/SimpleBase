using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base85Test
{
    [TestFixture]
    class Base85AlphabetTest
    {
        [Test]
        public void GetSafeCharCountForEncoding_Works()
        {
            var input = new byte[] { 0, 1, 2, 3 };
            Assert.AreEqual(6, Base85.Ascii85.Alphabet.GetSafeCharCountForEncoding(input));
        }

        [Test]
        public void HasShortcut()
        {
            Assert.IsTrue(Base85.Ascii85.Alphabet.HasShortcut);
            Assert.IsFalse(Base85.Z85.Alphabet.HasShortcut);
        }
    }
}
