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
   covered by SimpleBase including a few Base64 variants are supported. Base2, Base8, and Base10 are also supported.
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

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.4061)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.300
  [Host]     : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2

Encoding (64 byte buffer)

| Method                      | Mean      | Error    | StdDev   | Gen0   | Allocated |
|---------------------------- |----------:|---------:|---------:|-------:|----------:|
| DotNet_Base64               |  29.82 ns | 0.644 ns | 0.924 ns | 0.0119 |     200 B |
| Base2_Default               | 235.29 ns | 4.472 ns | 6.413 ns | 0.0625 |    1048 B |
| Base8_Default               | 126.04 ns | 2.556 ns | 3.903 ns | 0.0224 |     376 B |
| Base16_UpperCase            |  82.37 ns | 1.667 ns | 1.920 ns | 0.0167 |     280 B |
| Multibase_Base16_UpperCase  | 107.22 ns | 2.102 ns | 4.197 ns | 0.0334 |     560 B |
| Base32_CrockfordWithPadding | 156.90 ns | 1.599 ns | 1.335 ns | 0.0138 |     232 B |
| Base36_LowerCase            |  47.26 ns | 0.874 ns | 0.898 ns | 0.0091 |     152 B |
| Base45_Default              | 123.42 ns | 1.853 ns | 1.642 ns | 0.0129 |     216 B |
| Base58_Bitcoin              |  47.16 ns | 0.989 ns | 1.177 ns | 0.0091 |     152 B |
| Base58_Monero               | 207.06 ns | 1.920 ns | 1.796 ns | 0.0119 |     200 B |
| Base62_Default              |  46.39 ns | 0.509 ns | 0.476 ns | 0.0091 |     152 B |
| Base85_Z85                  | 163.01 ns | 1.226 ns | 1.024 ns | 0.0110 |     184 B |
| Base256Emoji_Default        | 230.50 ns | 4.505 ns | 4.214 ns | 0.0167 |     280 B |

Decoding (80 character string, except Base45 which must use an 81 character string)

| Method                               | Mean        | Error     | StdDev     | Gen0   | Gen1   | Allocated |
|------------------------------------- |------------:|----------:|-----------:|-------:|-------:|----------:|
| DotNet_Base64                        |   102.24 ns |  0.234 ns |   0.219 ns | 0.0052 |      - |      88 B |
| Base2_Default                        |   102.95 ns |  0.074 ns |   0.070 ns | 0.0024 |      - |      40 B |
| Base8_Default                        |   103.41 ns |  0.199 ns |   0.176 ns | 0.0024 |      - |      40 B |
| Base16_UpperCase                     |    50.60 ns |  0.931 ns |   0.777 ns | 0.0038 |      - |      64 B |
| Base16_UpperCase_TextReader          |   275.90 ns |  2.852 ns |   2.381 ns | 0.5007 | 0.0153 |    8376 B |
| Multibase_Base16_UpperCase           |    51.73 ns |  0.254 ns |   0.238 ns | 0.0038 |      - |      64 B |
| Multibase_TryDecode_Base16_UpperCase |    46.95 ns |  0.265 ns |   0.235 ns |      - |      - |         - |
| Base32_Crockford                     |   126.78 ns |  1.751 ns |   1.552 ns | 0.0048 |      - |      80 B |
| Base36_LowerCase                     | 4,160.01 ns | 65.532 ns |  54.722 ns |      - |      - |      80 B |
| Base45_Default                       |    73.56 ns |  1.248 ns |   1.167 ns | 0.0048 |      - |      80 B |
| Base58_Bitcoin                       | 4,544.46 ns | 89.449 ns | 133.882 ns |      - |      - |      88 B |
| Base58_Monero                        |   107.88 ns |  0.408 ns |   0.341 ns | 0.0052 |      - |      88 B |
| Base62_Default                       | 4,500.24 ns |  7.457 ns |   6.610 ns |      - |      - |      88 B |
| Base85_Z85                           |   256.92 ns |  0.580 ns |   0.514 ns | 0.0052 |      - |      88 B |
| Base256Emoji_Default                 |   287.68 ns |  1.634 ns |   1.528 ns | 0.0062 |      - |     104 B |

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
