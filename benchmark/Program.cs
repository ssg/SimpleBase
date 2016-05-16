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

        const int iterations = 1000000;
        const int testBufSize = 64; // typical buffer size -- needs to be legit Base64 length w/o padding

        static TimeSpan test(Action func)
        {
            Stopwatch w = Stopwatch.StartNew();
            for (int n = 0; n < iterations; n++)
            {
                func();
            }
            w.Stop();
            return w.Elapsed;
        }

        static void Encode()
        {
            byte[] buf = new byte[testBufSize];
            var base64 = test(() => Convert.ToBase64String(buf));
            var base32 = test(() => Base32.Crockford.Encode(buf, padding: true));
            var base58 = test(() => Base58.Bitcoin.Encode(buf));
            Dump("Encode benchmark", base64, base32, base58);
        }

        static void Decode()
        {
            var input = new string('a', testBufSize);
            var base64 = test(() => Convert.FromBase64String(input));
            var base32 = test(() => Base32.Crockford.Decode(input));
            var base58 = test(() => Base58.Bitcoin.Decode(input));
            Dump("Decode benchmark", base64, base32, base58);
        }

        static void Dump(string name, TimeSpan base64, TimeSpan base32, TimeSpan base58)
        {
            Console.WriteLine("*** " + name);
            Console.WriteLine(" Baseline (.NET base64): {0}", base64);
            Console.WriteLine(" SimpleBase Base32     : {0} ({1})", base32, compare(base32, base64));
            Console.WriteLine(" SimpleBase Base58     : {0} ({1})", base58, compare(base58, base64));
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
    }
}