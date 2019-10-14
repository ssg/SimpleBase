using NUnit.Framework;
using SimpleBase;
using System.IO;
using System.Threading.Tasks;

namespace SimpleBaseTest.Base16Test
{
    // these are just primitive tests just to make sure they at least do what they are supposed to.
    // we want to move this functionality out in the next release anyway.
    [TestFixture]
    class LegacyTest
    {
        private static readonly TestCaseData[] upperCaseTestData = new[]
        {
            new TestCaseData(new byte[] { }, ""),
            new TestCaseData(new byte[] { 0xAB }, "AB"),
            new TestCaseData(new byte[] { 0x00, 0x01, 0x02, 0x03 }, "00010203"),
            new TestCaseData(new byte[] { 0x10, 0x11, 0x12, 0x13 }, "10111213"),
            new TestCaseData(new byte[] { 0xAB, 0xCD, 0xEF, 0xBA }, "ABCDEFBA"),
            new TestCaseData(new byte[] { 0xAB, 0xCD, 0xEF, 0xBA, 0xAB, 0xCD, 0xEF, 0xBA }, "ABCDEFBAABCDEFBA"),
        };

        private static readonly TestCaseData[] testData = new[]
        {
            new TestCaseData(new byte[] { }, ""),
            new TestCaseData(new byte[] { 0xAB }, "ab"),
            new TestCaseData(new byte[] { 0x00, 0x01, 0x02, 0x03 }, "00010203"),
            new TestCaseData(new byte[] { 0x10, 0x11, 0x12, 0x13 }, "10111213"),
            new TestCaseData(new byte[] { 0xAB, 0xCD, 0xEF, 0xBA }, "abcdefba"),
            new TestCaseData(new byte[] { 0xAB, 0xCD, 0xEF, 0xBA, 0xAB, 0xCD, 0xEF, 0xBA }, "abcdefbaabcdefba"),
        };

        [Test]
        public void EncodeUpper_Works()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.AreEqual("ABCD", Base16.EncodeUpper(new byte[] { 0xAB, 0xCD }));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Test]
        public void EncodeLower_Works()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.AreEqual("abcd", Base16.EncodeLower(new byte[] { 0xAB, 0xCD }));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Test]
        [TestCaseSource(nameof(upperCaseTestData))]
        public void EncodeUpper_Stream(byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
#pragma warning disable CS0618 // Type or member is obsolete
            Base16.EncodeUpper(inputStream, writer);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.AreEqual(expectedOutput, writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(upperCaseTestData))]
        public async Task EncodeUpperAsync_Stream(byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
#pragma warning disable CS0618 // Type or member is obsolete
            await Base16.EncodeUpperAsync(inputStream, writer);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.AreEqual(expectedOutput, writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public void EncodeLower_Stream(byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
#pragma warning disable CS0618 // Type or member is obsolete
            Base16.EncodeLower(inputStream, writer);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.AreEqual(expectedOutput, writer.ToString());
        }

        [Test]
        [TestCaseSource(nameof(testData))]
        public async Task EncodeLowersync_Stream(byte[] input, string expectedOutput)
        {
            using var inputStream = new MemoryStream(input);
            using var writer = new StringWriter();
#pragma warning disable CS0618 // Type or member is obsolete
            await Base16.EncodeLowerAsync(inputStream, writer);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.AreEqual(expectedOutput, writer.ToString());
        }
    }
}
