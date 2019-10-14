using NUnit.Framework;
using SimpleBase;
using System;
using System.Collections.Generic;
using System.Text;

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
        public void GetSafeByteCountForDecoding_InvalidBufferSize_Throws()
        {
            var input = new char[11];
            Assert.Throws<ArgumentException>(() => Base16.UpperCase.Alphabet.GetSafeByteCountForDecoding(input));
        }
    }
}
