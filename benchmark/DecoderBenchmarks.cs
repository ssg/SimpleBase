using System;
using BenchmarkDotNet.Attributes;
using SimpleBase;

namespace benchmark;

[MarkdownExporterAttribute.GitHub]
[MarkdownExporter()]
public class DecoderBenchmarks
{
    private readonly string s = new('a', 80);

    [Benchmark(Baseline = true)]
    public byte[] DotNet_Base64() => Convert.FromBase64String(s);

    [Benchmark]
    public byte[] SimpleBase_Base16_UpperCase() => Base16.UpperCase.Decode(s);

    [Benchmark]
    public byte[] SimpleBase_Base32_Crockford() => Base32.Crockford.Decode(s);

    [Benchmark]
    public byte[] SimpleBase_Base85_Z85() => Base85.Z85.Decode(s);

    [Benchmark]
    public byte[] SimpleBase_Base58_Bitcoin() => Base58.Bitcoin.Decode(s);
}
