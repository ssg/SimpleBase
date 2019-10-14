using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base16Test
{
    [TestFixture]
    class Base16AlphabetTest
    {
        [Test]
        public void GetSafeCharCountForEncoding_ReturnsCorrectValue()
        {
            var input = new byte[5];
            Assert.AreEqual(10, Base16.UpperCase.Alphabet.GetSafeCharCountForEncoding(input));
        }

        [Test]
        public void GetSafeByteCountForDecoding_ReturnsCorrectValues()
        {
            var input = new char[10];
            Assert.AreEqual(5, Base16.UpperCase.Alphabet.GetSafeByteCountForDecoding(input));
        }

        [Test]
        public void GetSafeByteCountForDecoding_InvalidBufferSize_ReturnsZero()
        {
            var input = new char[11];
            Assert.AreEqual(0, Base16.UpperCase.Alphabet.GetSafeByteCountForDecoding(input));
        }
    }
}
