using System;
using System.Diagnostics;

namespace benchmark
{
    class Benchmark
    {
        public const int Iterations = 1000000;
        
        public const int BufSize = 64; // typical buffer size -- needs to be legit Base64 length w/o padding

        public string Name { get; private set; }
        public float Growth { get; private set; }
        public TimeSpan EncodeTime;
        public TimeSpan DecodeTime;
        public Action<byte[]> EncodeFunc { get; private set; }
        public Action<string> DecodeFunc { get; private set; }

        public Benchmark(string name, float growth, Action<byte[]> encodeFunc, Action<string> decodeFunc)
        {
            this.Name = name;
            this.Growth = growth;
            this.EncodeFunc = encodeFunc;
            this.DecodeFunc = decodeFunc;
        }

        private static string compare(TimeSpan bench, TimeSpan baseline)
        {
            if (bench > baseline)
            {
                return String.Format("{0:0.#}x slower", bench.TotalMilliseconds / baseline.TotalMilliseconds);
            }
            else
            {
                return String.Format("YAY! {0:0.#}x faster", baseline.TotalMilliseconds / bench.TotalMilliseconds);
            }
        }

        public string GetEncodeText(Benchmark baseline)
        {
            return getPrintable(this.EncodeTime, baseline.EncodeTime);
        }

        public string GetDecodeText(Benchmark baseline)
        {
            return getPrintable(this.DecodeTime, baseline.DecodeTime);
        }

        private string getPrintable(TimeSpan time, TimeSpan baseline)
        {
            string result = String.Format("{0:F2}", time.TotalMilliseconds / 1000.0);
            if (time == baseline)
            {
                return result;
            }
            return result + " (" + compare(time, baseline) + ")";
        }

        public void TestEncode()
        {
            byte[] buf = new byte[BufSize];
            buf[0] = 1;
            buf[BufSize - 1] = 1; // avoid all-zero optimizations of Base58
            Stopwatch w = Stopwatch.StartNew();
            for (int n = 0; n < Iterations; n++)
            {
                EncodeFunc(buf);
            }
            w.Stop();
            this.EncodeTime = w.Elapsed;
        }

        public void TestDecode()
        {
            string str = new String('a', BufSize);
            Stopwatch w = Stopwatch.StartNew();
            for (int n = 0; n < Iterations; n++)
            {
                DecodeFunc(str);
            }
            w.Stop();
            this.DecodeTime = w.Elapsed;
        }
    }
}