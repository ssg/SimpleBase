using System;
using SimpleBase;

namespace benchmark
{
    class Program
    {
        static Benchmark[] benchmarks = new[]
        {
            new Benchmark(".NET Framework Base64", 1.33f,
                (buf) => Convert.ToBase64String(buf),
                (s) => Convert.FromBase64String(s)),

            new Benchmark("SimpleBase Base16", 2f,
                (buf) => Base16.EncodeUpper(buf),
                (s) => Base16.Decode(s)),

            new Benchmark("SimpleBase Base32 Crockford", 1.6f,
                (buf) => Base32.Crockford.Encode(buf, padding: true),
                (s) => Base32.Crockford.Decode(s)),

            new Benchmark("SimpleBase Base58", 1.38f,
                (buf) => Base58.Bitcoin.Encode(buf),
                (s) => Base58.Bitcoin.Decode(s)),
        };

        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("WARNING: DEBUG mode");
#else
            Console.WriteLine("Release mode");
#endif
            runTests();
            Console.WriteLine();
            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        static void runTests()
        {
            Console.WriteLine();
            Console.WriteLine("{0:#,#} iterations on {1} byte buffer (encode) / {1} character string (decode)",
                Benchmark.Iterations, Benchmark.BufSize);
            Console.WriteLine(@"
Implementation              | Growth | Encode                   | Decode
----------------------------|--------|--------------------------|------------------");
            var baseline = benchmarks[0];
            foreach (var benchmark in benchmarks)
            {
                Console.Write("{0,-28}| {1,-7}|", benchmark.Name, 
                    String.Format("{0:0.##}x", benchmark.Growth));
                beginTest();
                benchmark.TestEncode();
                endTest();
                Console.Write(" {0,-25}|", benchmark.GetEncodeText(baseline));
                beginTest();                
                benchmark.TestDecode();
                endTest();
                Console.WriteLine(" {0, -25}", benchmark.GetDecodeText(baseline));
            }
        }

        private static void endTest()
        {
            Console.Write(new String('\x08', 11));
        }

        private static void beginTest()
        {
            Console.Write(" testing...");
        }
    }
}