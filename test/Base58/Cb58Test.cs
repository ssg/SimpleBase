using NUnit.Framework;
using SimpleBase;
using System;
using System.Text;

namespace SimpleBaseTest.Base58Test;

[TestFixture]
public class Cb58Test
{
    // CB58 test cases have been taken from avalanchego package at https://github.com/ava-labs/avalanchego
    //    BSD 3-Clause License

    //Copyright(c) 2020, Ava Labs, Inc.
    //All rights reserved.

    //Redistribution and use in source and binary forms, with or without
    //modification, are permitted provided that the following conditions are met:

    //1. Redistributions of source code must retain the above copyright notice, this
    //   list of conditions and the following disclaimer.

    //2. Redistributions in binary form must reproduce the above copyright notice,
    //   this list of conditions and the following disclaimer in the documentation
    //   and/or other materials provided with the distribution.

    //3. Neither the name of the copyright holder nor the names of its
    //   contributors may be used to endorse or promote products derived from
    //   this software without specific prior written permission.

    //THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
    //AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    //IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    //DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
    //FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    //DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
    //SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
    //CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    //OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    //OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
    private static readonly TestCaseData[] testData =
    [
        new TestCaseData(Array.Empty<byte>(), "45PJLL"),
		new TestCaseData(new byte[]{ 0}, "1c7hwa"),
		new TestCaseData(new byte[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 255}, "1NVSVezva3bAtJesnUj"),
        new TestCaseData(new byte[32] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 },
			"SkB92YpWm4Q2ijQHH34cqbKkCZWszsiQgHVjtNeFF2HdvDQU"),
	];


    [Test]
    [TestCaseSource(nameof(testData))]
    public void EncodeCheck_ValidInput_ReturnsExpectedResult(byte[] bytes, string expectedOutput)
    {
        string result = Base58.Bitcoin.EncodeCb58(bytes);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void TryDecodeCheck_ValidInput_ReturnsExpectedResult(byte[] expectedOutput, string input)
    {
        byte[] outputBuffer = new byte[256];
        bool result = Base58.Bitcoin.TryDecodeCb58(input, outputBuffer.AsSpan(), out int numBytesWritten);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(outputBuffer[..numBytesWritten], Is.EqualTo(expectedOutput));
        });
    }

}
