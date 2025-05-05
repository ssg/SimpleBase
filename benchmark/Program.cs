using BenchmarkDotNet.Running;

namespace Benchmark;

class Program
{
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0058 // Expression value is never used
    static void Main(string[] args)
    {
        _ = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        //BenchmarkRunner.Run<EncoderBenchmarks>();
        //BenchmarkRunner.Run<DecoderBenchmarks>();
    }
#pragma warning restore IDE0058 // Expression value is never used
#pragma warning restore IDE0060 // Remove unused parameter

}
