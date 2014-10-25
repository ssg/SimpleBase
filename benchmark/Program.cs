using System;
using System.Diagnostics;
using SimpleBase32;

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

        const int iterations = 100;
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
            w.Start();
            for (int n = 0; n < iterations; n++)
            {
                result = Base32.Crockford.Encode(buf, padding: true);
            }
            w.Stop();
            Dump("Encode benchmark", w.Elapsed, first);
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
            Dump("Decode benchmark", w.Elapsed, first);
        }

        static void Dump(string name, TimeSpan current, TimeSpan baseline)
        {
            Console.WriteLine("*** " + name);
            Console.WriteLine(" Baseline (.NET base64): {0}", baseline);
            Console.WriteLine(" SimpleBase32          : {0}", current);
            if (current > baseline)
            {
                Console.WriteLine("{0}x slower than baseline", Math.Floor(current.TotalMilliseconds / baseline.TotalMilliseconds));
            }
            else
            {
                Console.WriteLine("YAY! {0}x faster than baseline", Math.Floor(baseline.TotalMilliseconds / current.TotalMilliseconds));
            }
        }
    }
}