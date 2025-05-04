using BenchmarkDotNet.Running;

namespace Benchmark;

class Program
{
    static void Main()
    {
        // _ = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        _ = BenchmarkRunner.Run<EncoderBenchmarks>();
        _ = BenchmarkRunner.Run<DecoderBenchmarks>();
    }
}
