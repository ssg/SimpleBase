using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using SimpleBase;

namespace Benchmark;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
public class DecoderBenchmarks
{
    readonly string s = new('a', 80);
    readonly string ms = 'F' + new string('a', 80);
    readonly MemoryStream memoryStream = new();

    [Benchmark]
    public byte[] DotNet_Base64() => Convert.FromBase64String(s);

    [Benchmark]
    public byte[] SimpleBase_Base16_UpperCase() => Base16.UpperCase.Decode(s);

    /// <summary>
    /// Created to be able to bench <c>StreamHelper</c>
    /// The encoding does not matter for benching <c>StreamHelper</c>
    /// Base16 is fastest, means less overhead for measuring <c>StreamHelper</c>
    /// </summary>
    /// <remarks>
    /// [IterationSetup] or [IterationCleanup] attributes are not recommended for microbenchmarks, so setup & cleanup are part of the benchmark
    /// https://benchmarkdotnet.org/articles/features/setup-and-cleanup.html#sample-introsetupcleanupiteration
    /// </remarks>
    [Benchmark]
    public void SimpleBase_Base16_UpperCase_TextReader()
    {
        StringReader reader = new(s); // No need to dispose, less overhead, StringReader does not leak anything
        Base16.UpperCase.Decode(reader, memoryStream);
        memoryStream.Position = 0; // Reset output stream, so it does not grow forever, we do not need to read it
    }

    [Benchmark]
    public byte[] SimpleBase_Base32_Crockford() => Base32.Crockford.Decode(s);

    [Benchmark]
    public byte[] SimpleBase_Base85_Z85() => Base85.Z85.Decode(s);

    [Benchmark]
    public byte[] SimpleBase_Base58_Bitcoin() => Base58.Bitcoin.Decode(s);

    [Benchmark]
    public byte[] SimpleBase_Base58_Monero() => Base58.Monero.Decode(s);

    [Benchmark]
    public byte[] SimpleBase_Multibase_Base16_UpperCase() => Multibase.Decode(ms);
}