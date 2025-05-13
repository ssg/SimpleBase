SimpleBase
==========
[![NuGet Version](https://img.shields.io/nuget/v/SimpleBase.svg)](https://www.nuget.org/packages/SimpleBase/)
[![Build Status](https://github.com/ssg/SimpleBase/actions/workflows/test.yml/badge.svg)](https://github.com/ssg/SimpleBase/actions?query=workflow%3Atest)

This is my own take for exotic base encodings like Base32, Base58 and Base85. 
I started to write it in 2013 as coding practice and kept it as a small pet 
project. I suggest anyone who wants to brush up their coding skills to give 
those encoding problems a shot. They turned out to be more challenging than I 
expected. To grasp the algorithms I had to get a pen and paper to see how the 
math worked.

Features
--------
 - [Multibase](https://github.com/multiformats/multibase) support. All formats
   covered by SimpleBase including a few Base64 variants are supported. 
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

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3915)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 8.0.15 (8.0.1525.16413), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.15 (8.0.1525.16413), X64 RyuJIT AVX2

Encoding (64 byte buffer)

| Method                      | Mean      | Error    | StdDev   | Gen0   | Allocated |
|---------------------------- |----------:|---------:|---------:|-------:|----------:|
| DotNet_Base64               |  28.52 ns | 0.501 ns | 0.469 ns | 0.0119 |     200 B |
| Base16_UpperCase            |  83.05 ns | 1.432 ns | 1.340 ns | 0.0167 |     280 B |
| Multibase_Base16_UpperCase  | 100.51 ns | 1.671 ns | 1.563 ns | 0.0334 |     560 B |
| Base32_CrockfordWithPadding | 148.46 ns | 0.715 ns | 0.634 ns | 0.0138 |     232 B |
| Base36_LowerCase            |  44.05 ns | 0.039 ns | 0.037 ns |      - |         - |
| Base45_Default              | 121.33 ns | 0.702 ns | 0.657 ns | 0.0129 |     216 B |
| Base58_Bitcoin              |  44.62 ns | 0.297 ns | 0.232 ns | 0.0091 |     152 B |
| Base58_Monero               | 208.37 ns | 3.417 ns | 3.656 ns | 0.0119 |     200 B |
| Base62_Default              |  43.61 ns | 0.141 ns | 0.132 ns |      - |         - |
| Base85_Z85                  | 148.95 ns | 1.231 ns | 1.151 ns | 0.0110 |     184 B |
| Base256Emoji_Default        | 224.16 ns | 1.189 ns | 1.112 ns | 0.0167 |     280 B |

Decoding (80 character string, except Base45 which must use an 81 character string)

| Method                               | Mean        | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|------------------------------------- |------------:|----------:|----------:|-------:|-------:|----------:|
| DotNet_Base64                        |   105.76 ns |  1.258 ns |  1.177 ns | 0.0052 |      - |      88 B |
| Base16_UpperCase                     |    49.91 ns |  0.494 ns |  0.462 ns | 0.0038 |      - |      64 B |
| Base16_UpperCase_TextReader          |   308.32 ns |  6.134 ns |  5.438 ns | 0.5007 | 0.0153 |    8376 B |
| Multibase_Base16_UpperCase           |    51.04 ns |  0.417 ns |  0.390 ns | 0.0038 |      - |      64 B |
| Multibase_TryDecode_Base16_UpperCase |    46.19 ns |  0.162 ns |  0.151 ns |      - |      - |         - |
| Base32_Crockford                     |   141.81 ns |  1.785 ns |  1.669 ns | 0.0048 |      - |      80 B |
| Base36_LowerCase                     | 4,039.85 ns |  7.350 ns |  6.875 ns |      - |      - |      80 B |
| Base45_Default                       |    89.65 ns |  0.513 ns |  0.479 ns | 0.0048 |      - |      80 B |
| Base58_Bitcoin                       | 3,688.39 ns | 28.256 ns | 23.595 ns | 0.0038 |      - |      88 B |
| Base58_Monero                        |   109.46 ns |  1.426 ns |  1.334 ns | 0.0052 |      - |      88 B |
| Base62_Default                       | 4,563.37 ns | 39.847 ns | 37.273 ns |      - |      - |      88 B |
| Base85_Z85                           |   253.36 ns |  1.720 ns |  1.525 ns | 0.0052 |      - |      88 B |
| Base256Emoji_Default                 |   291.45 ns |  2.437 ns |  2.280 ns | 0.0062 |      - |     104 B |

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
