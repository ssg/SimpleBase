/*
     Copyright 2014-2025 Sedat Kapanoglu

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
using System.Linq;
using NUnit.Framework;
using SimpleBase;

namespace SimpleBaseTest.Base32Test;

[TestFixture]
class CrockfordEncodeCheckTest
{
    // test cases are from: https://github.dev/stacks-network/c32check/blob/master/tests/unitTests/src/index.ts
    string[] inputs = [
        "a46ff88886c2ef9762d970b4d2c63678835bd39d",
        "",
        "0000000000000000000000000000000000000000",
        "0000000000000000000000000000000000000001",
        "1000000000000000000000000000000000000001",
        "1000000000000000000000000000000000000000",
        "1",
        "22",
        "001",
        "0001",
        "00001",
        "000001",
        "0000001",
        "00000001",
        "10",
        "100",
        "1000",
        "10000",
        "100000",
        "1000000",
        "10000000",
        "100000000",
    ];

    byte[] versions = [22, 0, 31, 11, 17, 2];

    string[][] expectedOutputs = [
      [
      "P2J6ZY48GV1EZ5V2V5RB9MP66SW86PYKKNRV9EJ7",
      "P37JJX3D",
      "P000000000000000000002Q6VF78",
      "P00000000000000000005JA84HQ",
      "P80000000000000000000000000000004R0CMNV",
      "P800000000000000000000000000000033H8YKK",
      "P4VKEFGY",
      "P4ABAT49T",
      "P040SMAT7",
      "P040SMAT7",
      "P007S3BZWD",
      "P007S3BZWD",
      "P0005MDH0A2",
      "P0005MDH0A2",
      "P22J7S4CS",
      "P101ST5JKW",
      "PG02NDNFP7",
      "P80022RTP9J",
      "P40002HQ7B52",
      "P200003AWNGGR",
      "P1000003BCJ108",
      "PG000000DMPNB9",
    ],
    [
      "02J6ZY48GV1EZ5V2V5RB9MP66SW86PYKKPVKG2CE",
      "0A0DR2R",
      "0000000000000000000002AA028H",
      "000000000000000000006EKBDDS",
      "080000000000000000000000000000007R1QC00",
      "080000000000000000000000000000003ENTGCQ",
      "04C407K6",
      "049Q1W6AP",
      "006NZP224",
      "006NZP224",
      "0007YBH12H",
      "0007YBH12H",
      "000053HGS6K",
      "000053HGS6K",
      "021732WNV",
      "0103H9VB3W",
      "0G02BDQDTZ",
      "08002CT6SBA",
      "0400012P9QQ9",
      "02000008BW4AV",
      "010000013625RF",
      "0G000001QFSPXM",
    ],
    [
      "Z2J6ZY48GV1EZ5V2V5RB9MP66SW86PYKKQ9H6DPR",
      "Z44N8Q4",
      "Z000000000000000000002ZE1VMN",
      "Z00000000000000000005HZ3DVN",
      "Z80000000000000000000000000000004XBV6MS",
      "Z800000000000000000000000000000007VF5G0",
      "Z6RHFJAJ",
      "Z4BM8HYJA",
      "Z05NKF50D",
      "Z05NKF50D",
      "Z004720442",
      "Z004720442",
      "Z00073C2AR7",
      "Z00073C2AR7",
      "Z23M13WT9",
      "Z103F8N2SE",
      "ZG02G54C7T",
      "Z8000MKD341",
      "Z40003HGBBVV",
      "Z2000039BDD6F",
      "Z100000082GT4Q",
      "ZG0000021P09KP",
    ],
    [
      "B2J6ZY48GV1EZ5V2V5RB9MP66SW86PYKKNGTQ5XV",
      "B29AKKQ8",
      "B000000000000000000001A6KF5R",
      "B00000000000000000004TNHE36",
      "B80000000000000000000000000000007N1Y0J3",
      "B80000000000000000000000000000001P0H0EC",
      "B40R2K2V",
      "B4BCDY460",
      "B04PB501R",
      "B04PB501R",
      "B0057NK813",
      "B0057NK813",
      "B00048S8YNY",
      "B00048S8YNY",
      "B20QX4FW0",
      "B102PC6RCC",
      "BG02G1QXCQ",
      "B8000FWS04R",
      "B40001KAMP9Y",
      "B200002DNYYYC",
      "B1000003P9CPW6",
      "BG000003473Z3W",
    ],
    [
      "H2J6ZY48GV1EZ5V2V5RB9MP66SW86PYKKPZJKGHG",
      "HXQCX36",
      "H00000000000000000000ZKV5K0",
      "H000000000000000000049FQ4N0",
      "H800000000000000000000000000000043X9S3R",
      "H80000000000000000000000000000002R04Y9K",
      "H4NDX0WY",
      "H48VZCZQ1",
      "H05JF5G0A",
      "H05JF5G0A",
      "H007KAN0NP",
      "H007KAN0NP",
      "H000663B0ZQ",
      "H000663B0ZQ",
      "H23SE241P",
      "H102X2YQF6",
      "HG0322PNKV",
      "H8000JDRJP4",
      "H40003YJA8JD",
      "H200001ZTRYYH",
      "H1000002QFX7E6",
      "HG000000PPMVDM",
    ],
    [
      "22J6ZY48GV1EZ5V2V5RB9MP66SW86PYKKMQMB2T9",
      "2EC7BFA",
      "2000000000000000000003BMZJ0A",
      "200000000000000000004CF2C9N",
      "280000000000000000000000000000005Z78VV5",
      "280000000000000000000000000000000SJ03P9",
      "24N9YTH0",
      "24ATP2H2P",
      "206CXSP43",
      "206CXSP43",
      "2006CWFQ58",
      "2006CWFQ58",
      "20007TGK2A5",
      "20007TGK2A5",
      "222Q3MF1Q",
      "2100EZ96RY",
      "2G01YNNNTE",
      "28001HQ43QG",
      "240002P4722F",
      "2200001ASE5V7",
      "210000038X74ER",
      "2G000003FNKA3P",
    ],
  ];

    [Test]
    public void EncodeCheck_ReturnsExpectedCrockfordCheckEncodedStrings()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            var bytes = Convert.FromHexString(inputs[i]);
            for (int j = 0; j < versions.Length; j++)
            {
                byte version = versions[j];
                string expected = expectedOutputs[j][i];
                string result = Base32.Crockford.EncodeCheck(bytes, version);
                Assert.That(result, Is.EqualTo(expected), $"Failed for version {version} and hex string {inputs[i]}");
            }
        }
    }

}