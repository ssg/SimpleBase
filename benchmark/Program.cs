using System;
using SimpleBase;

namespace benchmark
{
    class Program
    {
        static readonly Benchmark[] benchmarks = new[]
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

            new Benchmark("SimpleBase Base85 Z85", 1.25f,
                (buf) => Base85.Z85.Encode(buf),
                (s) => Base85.Z85.Decode(s)),

            new Benchmark("SimpleBase Base58", 1.38f,
                (buf) => Base58.Bitcoin.Encode(buf),
                (s) => Base58.Bitcoin.Decode(s)),

        };

        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("***************************************");
            Console.WriteLine("********* WARNING: DEBUG mode *********");
            Console.WriteLine("***************************************");
#endif
            runTests();
            Console.WriteLine();
            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        static void runTests()
        {
            Console.WriteLine($"{Benchmark.Iterations:#,#} iterations");
            Console.WriteLine($"{Benchmark.EncodeSize} byte buffer for encoding");
            Console.WriteLine($"{Benchmark.DecodeSize} character string for decoding");
            Console.WriteLine("\n\r" +
                "Implementation              | Growth | Encode                   | Decode\n\r" +
                "----------------------------|--------|--------------------------|------------------");
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