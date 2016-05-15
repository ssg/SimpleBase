using System;
using System.Diagnostics;
using SimpleBase;

namespace benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("WARNING: DEBUG mode");
#else
            Console.WriteLine("Release mode");
#endif
            Encode();
            Decode();
            Console.ReadLine();
        }

        const int iterations = 1000;
        const int testBufSize = 1024 * 1024;

        static void Encode()
        {
            var buf = new byte[testBufSize];
            string result;
            Stopwatch w = Stopwatch.StartNew();
            for (int n = 0; n < iterations; n++)
            {
                result = Convert.ToBase64String(buf);
            }
            w.Stop();
            var first = w.Elapsed;
            w.Restart();
            for (int n = 0; n < iterations; n++)
            {
                result = Base32.Crockford.Encode(buf, padding: true);
            }
            w.Stop();
            var base32 = w.Elapsed;
            w.Restart();
            for (int n = 0; n < iterations; n++)
            {
                result = Base58.Bitcoin.Encode(buf);
            }
            w.Stop();
            var base58 = w.Elapsed;
            Dump("Encode benchmark", base58, base32, first);
        }

        static void Decode()
        {
            var input = new string('a', testBufSize);
            byte[] result;
            Stopwatch w = Stopwatch.StartNew();
            for (int n = 0; n < iterations; n++)
            {
                result = Convert.FromBase64String(input);
            }
            w.Stop();
            var first = w.Elapsed;
            w.Restart();
            for (int n = 0; n < iterations; n++)
            {
                result = Base32.Crockford.Decode(input);
            }
            w.Stop();
            var base32 = w.Elapsed;
            w.Restart();
            for (int n = 0; n < iterations; n++)
            {
                result = Base58.Bitcoin.Decode(input);
            }
            w.Stop();
            var base58 = w.Elapsed;
            Dump("Decode benchmark", base58, base32, first);
        }

        static void Dump(string name, TimeSpan base58, TimeSpan base32, TimeSpan base64)
        {
            Console.WriteLine("*** " + name);
            Console.WriteLine(" Baseline (.NET base64): {0}", base64);
            Console.WriteLine(" SimpleBase Base32     : {0}", base32);
            Console.WriteLine(" SimpleBase Base58     : {0}", base58);
            if (base32 > base64)
            {
                Console.WriteLine("{0}x slower than baseline", Math.Floor(base32.TotalMilliseconds / base64.TotalMilliseconds));
            }
            else
            {
                Console.WriteLine("YAY! {0}x faster than baseline", Math.Floor(base64.TotalMilliseconds / base32.TotalMilliseconds));
            }
        }
    }
}