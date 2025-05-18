using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using SimpleBase;

namespace Benchmark;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
public class DecoderBenchmarks
{
    readonly string lowercaseA = new('a', 80);
    readonly string multibasePrefixed = 'F' + new string('a', 80);
    readonly string base45str = new('A', 81);
    readonly string emojiStr = string.Concat(Enumerable.Repeat("🚀", 80));
    readonly string allZeroes = new('0', 80);
    readonly MemoryStream memoryStream = new();
    static readonly byte[] buffer = new byte[80];

    [Benchmark]
    public byte[] DotNet_Base64() => Convert.FromBase64String(lowercaseA);

    [Benchmark]
    public byte[] Base2_Default() => Base2.Default.Decode(allZeroes);

    [Benchmark]
    public byte[] Base8_Default() => Base2.Default.Decode(allZeroes);

    [Benchmark]
    public byte[] Base16_UpperCase() => Base16.UpperCase.Decode(lowercaseA);

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
    public void Base16_UpperCase_TextReader()
    {
        StringReader reader = new(lowercaseA); // No need to dispose, less overhead, StringReader does not leak anything
        Base16.UpperCase.Decode(reader, memoryStream);
        memoryStream.Position = 0; // Reset output stream, so it does not grow forever, we do not need to read it
    }

    [Benchmark]
    public byte[] Multibase_Base16_UpperCase() => Multibase.Decode(multibasePrefixed);

    [Benchmark]
    public byte[] Multibase_TryDecode_Base16_UpperCase()
    {
        bool result = Multibase.TryDecode(multibasePrefixed, buffer, out _);
        if (!result)
        {
            throw new InvalidOperationException("Failed to decode");
        }
        return buffer;
    }

    [Benchmark]
    public byte[] Base32_Crockford() => Base32.Crockford.Decode(lowercaseA);

    [Benchmark]
    public byte[] Base36_LowerCase() => Base36.LowerCase.Decode(lowercaseA);

    [Benchmark]
    public byte[] Base45_Default() => Base45.Default.Decode(base45str);

    [Benchmark]
    public byte[] Base58_Bitcoin() => Base58.Bitcoin.Decode(lowercaseA);

    [Benchmark]
    public byte[] Base58_Monero() => Base58.Monero.Decode(lowercaseA);

    [Benchmark]
    public byte[] Base62_Default() => Base62.Default.Decode(lowercaseA);

    [Benchmark]
    public byte[] Base85_Z85() => Base85.Z85.Decode(lowercaseA);

    [Benchmark]
    public byte[] Base256Emoji_Default() => Base256Emoji.Default.Decode(emojiStr);
}