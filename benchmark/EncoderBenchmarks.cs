using System;
using BenchmarkDotNet.Attributes;
using SimpleBase;

namespace benchmark;

[MarkdownExporterAttribute.GitHub]
public class EncoderBenchmarks
{
    private readonly byte[] buffer = new byte[64];

    [Benchmark(Baseline = true)]
    public string DotNet_Base64() => Convert.ToBase64String(buffer);

    [Benchmark]
    public string SimpleBase_Base16_UpperCase() => Base16.UpperCase.Encode(buffer);

    [Benchmark]
    public string SimpleBase_Base32_CrockfordWithPadding() => Base32.Crockford.Encode(buffer, padding: true);

    [Benchmark]
    public string SimpleBase_Base85_Z85() => Base85.Z85.Encode(buffer);

    [Benchmark]
    public string SimpleBase_Base58_Bitcoin() => Base58.Bitcoin.Encode(buffer);
}
