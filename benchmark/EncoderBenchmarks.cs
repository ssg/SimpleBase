using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using SimpleBase;

namespace Benchmark;

[MemoryDiagnoser]
public class EncoderBenchmarks
{
    readonly byte[] buffer = Enumerable.Range(1, 64).Select(i => (byte)i).ToArray();

    [Benchmark]
    public string DotNet_Base64() => Convert.ToBase64String(buffer);

    [Benchmark]
    public string Base2_Default() => Base2.Default.Encode(buffer);

    [Benchmark]
    public string Base8_Default() => Base8.Default.Encode(buffer);

    [Benchmark]
    public string Base10_Default() => Base10.Default.Encode(buffer);

    [Benchmark]
    public string Base16_UpperCase() => Base16.UpperCase.Encode(buffer);

    [Benchmark]
    public string Multibase_Base16_UpperCase() => Multibase.Encode(buffer, MultibaseEncoding.Base16Upper);

    [Benchmark]
    public string Base32_CrockfordWithPadding() => Base32.Crockford.Encode(buffer, padding: true);

    [Benchmark]
    public string Base36_LowerCase() => Base36.LowerCase.Encode(buffer);

    [Benchmark]
    public string Base45_Default() => Base45.Default.Encode(buffer);

    [Benchmark]
    public string Base58_Bitcoin() => Base58.Bitcoin.Encode(buffer);

    [Benchmark]
    public string Base58_Monero() => Base58.Monero.Encode(buffer);

    [Benchmark]
    public string Base62_Default() => Base62.Default.Encode(buffer);

    [Benchmark]
    public string Base85_Z85() => Base85.Z85.Encode(buffer);

    [Benchmark]
    public string Base256Emoji_Default() => Base256Emoji.Default.Encode(buffer);
}
