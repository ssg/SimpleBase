using NUnit.Framework;
using SimpleBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBaseTest.Base85Test
{
    [TestFixture]
    public class Rfc1924Test
    {
        [Test]
        [TestCase("1080:0:0:0:8:800:200C:417A", "4)+k&C#VzJ4br>0wv%Yp")]
        public void EncodeIpv6_WorksCorrectly(string ip, string expectedOutput)
        {
            var addr = IPAddress.Parse(ip);
            Assert.That(Base85.Rfc1924.EncodeIpv6(addr), Is.EqualTo(expectedOutput));
        }
    }
}
