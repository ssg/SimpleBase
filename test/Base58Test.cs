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
using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using NUnit.Framework;
using SimpleBase;

namespace Base32Test
{
    [TestFixture]
    [Parallelizable]
    class Base58Test
    {
        private static TestCaseData[] bitcoinTestData = new TestCaseData[]
        {
            new TestCaseData("0000010203", "11Ldp"),
            new TestCaseData("009C1CA2CBA6422D3988C735BB82B5C880B0441856B9B0910F", "1FESiat4YpNeoYhW3Lp7sW1T6WydcW7vcE"),
            new TestCaseData("000860C220EBBAF591D40F51994C4E2D9C9D88168C33E761F6", "1mJKRNca45GU2JQuHZqZjHFNktaqAs7gh"),
            new TestCaseData("00313E1F905554E7AE2580CD36F86D0C8088382C9E1951C44D010203", "17f1hgANcLE5bQhAGRgnBaLTTs23rK4VGVKuFQ"),
            new TestCaseData("0000000000", "11111"),
            new TestCaseData("1111111111", "2vgLdhi"),
            new TestCaseData("FFEEDDCCBBAA", "3CSwN61PP"),
            new TestCaseData("00", "1"),
            new TestCaseData("21", "a"),
        };

        [Test]
        public void Encode_NullBuffer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Base58.Bitcoin.Encode(null));
        }

        [Test]
        [TestCaseSource("bitcoinTestData")]
        public void Encode_Bitcoin_ReturnsExpectedResults(string input, string expectedOutput)
        {
            var buffer = SoapHexBinary.Parse(input).Value;
            string result = Base58.Bitcoin.Encode(buffer);
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        public void Ctor_NullAlphabet_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Base58(null));
        }

        [Test]
        public void Encode_EmptyBuffer_ReturnsEmptyString()
        {
            Assert.AreEqual(String.Empty, Base58.Bitcoin.Encode(new byte[] { }));
        }

        [Test]
        public void Decode_EmptyString_ReturnsEmptyBuffer()
        {
            var result = Base58.Bitcoin.Decode(String.Empty);
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Decode_NullBuffer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Base58.Bitcoin.Decode(null));
        }

        [Test]
        public void Decode_InvalidCharacter_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => Base58.Bitcoin.Decode("?"));
        }

        [Test]
        [TestCaseSource("bitcoinTestData")]
        public void Decode_Bitcoin_ReturnsExpectedResults(string expectedOutput, string input)
        {
            byte[] buffer = Base58.Bitcoin.Decode(input);
            string result = BitConverter.ToString(buffer).Replace("-", "");
            Assert.AreEqual(expectedOutput, result);
        }

    }
}