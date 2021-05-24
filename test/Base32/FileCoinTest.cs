using System;
using System.Text;
using SimpleBase;
using NUnit.Framework;

namespace SimpleBaseTest.Base32Test
{
    [TestFixture]
    class FileCoinTest
    {
        private static readonly string[][] testData = new[]
        {
            new[] { "", "" },
            new[] {"f", "my======" },
            new[] {"fo", "mzxq====" },
            new[] {"foo", "mzxw6===" },
            new[] {"foob", "mzxw6yq=" },
            new[] {"fooba", "mzxw6ytb" },
            new[] {"foobar", "mzxw6ytboi======" },
            new[] {"foobar1", "mzxw6ytboiyq====" },
            new[] {"foobar12", "mzxw6ytboiyte===" },
            new[] {"foobar123", "mzxw6ytboiytemy=" },
            new[] {"1234567890123456789012345678901234567890", "gezdgnbvgy3tqojqgezdgnbvgy3tqojqgezdgnbvgy3tqojqgezdgnbvgy3tqojq" }
        };

        private static readonly object[] byteTestData = new object[]
        {
            new object[] { new byte[] { 245, 202, 80, 149, 94, 201, 222, 50, 17, 198, 138, 104, 32, 183, 131, 33, 139, 208, 203, 211, 197, 191, 92, 194 }, "6xffbfk6zhpdeeogrjucbn4degf5bs6tyw7vzqq", false },
        };

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Encode_ReturnsExpectedValues(string input, string expectedOutput)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            string result = Base32.FileCoin.Encode(bytes, padding: true);
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCaseSource(nameof(byteTestData))]
        public void Encode_Bytes_ReturnsExpectedValues(byte[] bytes, string expectedOutput, bool padding)
        {
            string result = Base32.FileCoin.Encode(bytes, padding: padding);
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void Decode_ReturnsExpectedValues(string expectedOutput, string input)
        {
            var bytes = Base32.FileCoin.Decode(input);
            string result = Encoding.ASCII.GetString(bytes.ToArray());
            Assert.That(result, Is.EqualTo(expectedOutput));
            bytes = Base32.FileCoin.Decode(input.ToLowerInvariant());
            result = Encoding.ASCII.GetString(bytes.ToArray());
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Encode_NullBytes_ReturnsEmptyString()
        {
            Assert.That(Base32.FileCoin.Encode(null, true), Is.EqualTo(String.Empty));
        }

        [Test]
        public void Decode_InvalidInput_ThrowsArgumentException()
        {
            _ = Assert.Throws<ArgumentException>(() => Base32.FileCoin.Decode("[];',m."));
        }
    }
}