using System;
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest
{
    [TestFixture]
    internal class RequireTest
    {
        [Test]
        public void NotNull_NotNull_DoesNothing()
        {
            Assert.DoesNotThrow(() => Require.NotNull("test", "paramName"));
        }

        [Test]
        public void NotNull_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Require.NotNull<string>(null, "paramName"));
        }
    }
}