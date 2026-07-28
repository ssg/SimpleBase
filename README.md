SimpleBase
==========
[![NuGet Version](https://img.shields.io/nuget/v/SimpleBase.svg)](https://www.nuget.org/packages/SimpleBase/)
[![Build Status](https://github.com/ssg/SimpleBase/actions/workflows/build.yml/badge.svg)](https://github.com/ssg/SimpleBase/actions?query=workflow%3Abuild)

This is my own take for exotic base encodings like Base32, Base58 and Base85. 
I started to write it in 2013 as coding practice and kept it as a small pet 
project. I suggest anyone who wants to brush up their coding skills to give 
those encoding problems a shot. They turned out to be more challenging than I 
expected. To grasp the algorithms I had to get a pen and paper to see how the 
math worked.

Features
--------
 - **[Multibase](https://github.com/multiformats/multibase)**: All formats
   covered by SimpleBase including a few Base64 variants are supported. **Base2**, **Base8**, and **Base10** are also supported.
 - **Base32**: RFC 4648, BECH32, Crockford, z-base-32, Geohash, FileCoin and Extended Hex 
   (BASE32-HEX) flavors with Crockford character substitution, or any other 
   custom flavors.
 - **Base36**: Both lowercase and uppercase alphabets are supported.
 - **Base45**: RFC 9285 is supported.
 - **Base58**: All the standard (Bitcoin (BTC), Ripple (XRP), Monero (XMR)) and custom Base58 encoding methods are supported. Also provides Base58Check and Avalanche (AVAX) CB58 encoding/decoding helpers.
 - **Base62**: The standard Base62 encoding/decoding supported along with a custom alphabet.
 - **Base85**: Ascii85, Z85 and custom flavors. IPv6 encoding/decoding support.
 - **Base16**: UpperCase, LowerCase and ModHex flavors. An experimental hexadecimal 
   encoder/decoder just to see how far I can take the optimizations compared to .NET's
   implementations. It's quite fast now, but .NET has [`Convert.FromHexString()`](https://learn.microsoft.com/en-us/dotnet/api/system.convert.fromhexstring) method since .NET 5.
   This is mostly a baseline implementation now (except for when you need ModHex).
 - **Base256 Emoji🚀**: Supported by Multibase, and can also be used individually. 
 - One-shot memory buffer based APIs for simple use cases.
 - Stream-based async APIs for more advanced scenarios.
 - Lightweight: No dependencies.
 - Support for big-endian CPUs like IBM s390x (zArchitecture).
 - Thread-safe.
 - Simple to use.

NuGet
------
To install it from [NuGet](https://www.nuget.org/packages/SimpleBase/):

  `Install-Package SimpleBase`

If you need .NET Standard 2.0 compatible version for targeting older .NET Framework or .NET Core, use the 2.x release line instead. It's missing newer features, but still supported:

  `Install-Package SimpleBase -MaximumVersion 2.999.0`

Usage
------
The basic usage for encoding a buffer into, say, Base32, is as simple as:

```csharp
using SimpleBase;

byte[] myBuffer;
string result = Base32.Crockford.Encode(myBuffer, padding: true);
// you can also use "ExtendedHex" or "Rfc4648" as encoder flavors
```

Decoding is also similar:

```csharp
using SimpleBase;

string myText = ...
byte[] result = Base32.Crockford.Decode(myText);
```

See the [wiki](wiki) for more types of examples and full documentation. 

Benchmark Results
-----------------
Small buffer sizes are used (64 characters). They are closer to real life 
applications. Base58 performs really bad in decoding of larger buffer sizes, 
due to polynomial complexity of numeric base conversions.

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8894/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 5950X 3.40GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3


| Method                      | Mean         | Error     | StdDev    | Gen0   | Allocated |
|---------------------------- |-------------:|----------:|----------:|-------:|----------:|
| DotNet_Base64               |     17.69 ns |  0.117 ns |  0.104 ns | 0.0120 |     200 B |
| Base2_Default               |    229.76 ns |  4.648 ns |  8.843 ns | 0.0625 |    1048 B |
| Base8_Default               |    110.19 ns |  2.138 ns |  2.927 ns | 0.0219 |     368 B |
| Base10_Default              | 10,380.62 ns | 43.676 ns | 36.471 ns | 0.0153 |     328 B |
| Base16_UpperCase            |     68.47 ns |  1.209 ns |  1.131 ns | 0.0167 |     280 B |
| Multibase_Base16_UpperCase  |     81.15 ns |  1.625 ns |  3.734 ns | 0.0334 |     560 B |
| Base32_CrockfordWithPadding |    128.36 ns |  1.676 ns |  1.308 ns | 0.0138 |     232 B |
| Base36_LowerCase            |  6,638.64 ns | 25.361 ns | 23.722 ns | 0.0076 |     224 B |
| Base45_Default              |     97.34 ns |  1.872 ns |  1.659 ns | 0.0129 |     216 B |
| Base58_Bitcoin              |  5,849.35 ns | 27.707 ns | 23.137 ns | 0.0076 |     200 B |
| Base58_Monero               |    174.22 ns |  3.455 ns |  4.613 ns | 0.0119 |     200 B |
| Base62_Default              |  5,872.56 ns | 98.857 ns | 92.471 ns | 0.0076 |     192 B |
| Base85_Z85                  |    119.07 ns |  2.199 ns |  2.258 ns | 0.0110 |     184 B |
| Base256Emoji_Default        |    160.29 ns |  3.073 ns |  3.156 ns | 0.0162 |     272 B |

Decoding (80 character string, except Base45 which must use an 81 character string)

| Method                               | Mean        | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|------------------------------------- |------------:|----------:|----------:|-------:|-------:|----------:|
| DotNet_Base64                        |    73.50 ns |  0.586 ns |  0.548 ns | 0.0052 |      - |      88 B |
| Base2_Default                        |    75.73 ns |  0.604 ns |  0.535 ns | 0.0024 |      - |      40 B |
| Base8_Default                        |    48.33 ns |  0.564 ns |  0.441 ns | 0.0033 |      - |      56 B |
| Base10_Default                       | 1,508.23 ns | 14.977 ns | 14.009 ns | 0.0038 |      - |      64 B |
| Base16_UpperCase                     |    39.44 ns |  0.346 ns |  0.324 ns | 0.0038 |      - |      64 B |
| Base16_UpperCase_TextReader          |   208.16 ns |  4.168 ns |  8.130 ns | 0.4966 | 0.0155 |    8312 B |
| Multibase_Base16_UpperCase           |    41.37 ns |  0.832 ns |  0.779 ns | 0.0038 |      - |      64 B |
| Multibase_TryDecode_Base16_UpperCase |    36.23 ns |  0.506 ns |  0.395 ns |      - |      - |         - |
| Base32_Crockford                     |    77.25 ns |  0.380 ns |  0.355 ns | 0.0048 |      - |      80 B |
| Base36_LowerCase                     | 2,235.53 ns | 24.883 ns | 23.276 ns | 0.0038 |      - |      80 B |
| Base45_Default                       |    51.74 ns |  0.330 ns |  0.293 ns | 0.0048 |      - |      80 B |
| Base58_Bitcoin                       | 2,729.74 ns | 15.788 ns | 13.996 ns | 0.0038 |      - |      88 B |
| Base58_Monero                        |    57.42 ns |  0.658 ns |  0.513 ns | 0.0052 |      - |      88 B |
| Base62_Default                       | 3,002.94 ns | 11.360 ns |  9.486 ns | 0.0038 |      - |      88 B |
| Base85_Z85                           |   189.11 ns |  1.445 ns |  1.352 ns | 0.0052 |      - |      88 B |
| Base256Emoji_Default                 |   223.56 ns |  2.182 ns |  1.934 ns | 0.0062 |      - |     104 B |

Notes
-----
I'm sure there are areas for improvement. I didn't want to go further in 
optimizations which would hurt readability and extensibility. I might 
experiment on them in the future.

Test suite for Base32 isn't complete, I took most of it from RFC4648. Base58 
really lacks a good spec or test vectors needed. I had to resort to using 
online converters to generate preliminary test vectors.

Base85 tests are also makseshift tests based on what output 
[Cryptii](https://cryptii.com/) produces. Contribution to missing test cases 
are greatly appreciated.

It's interesting that I wasn't able to reach .NET Base64's performance with 
Base16 with a straightforward managed code despite that it's much simpler. I 
was only able to match it after I converted Base16 to unsafe code with good 
independent interleaving so CPU pipeline optimizations could take place. 
Still not satisfied though. Is .NET's Base64 implementation native? Perhaps.

Thanks
------
Thanks to all [contributors](https://github.com/ssg/SimpleBase/graphs/contributors) who
provided patches, and reported bugs.

Chatting about this pet project with my friends 
[@detaybey](https://github.com/detaybey), 
[@vhallac](https://github.com/vhallac), 
[@alkimake](https://github.com/alkimake) and 
[@Utopians](https://github.com/Utopians) at one of our friend's birthday 
encouraged me to finish this. Thanks guys.
