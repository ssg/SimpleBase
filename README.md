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

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7705/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 5950X 4.00GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v3


| Method                      | Mean        | Error     | StdDev    | Gen0   | Allocated |
|---------------------------- |------------:|----------:|----------:|-------:|----------:|
| DotNet_Base64               |    22.93 ns |  0.413 ns |  0.345 ns | 0.0120 |     200 B |
| Base2_Default               |   269.93 ns |  5.336 ns |  5.240 ns | 0.0625 |    1048 B |
| Base8_Default               |   133.93 ns |  2.183 ns |  1.823 ns | 0.0219 |     368 B |
| Base16_UpperCase            |    85.39 ns |  0.642 ns |  0.600 ns | 0.0167 |     280 B |
| Multibase_Base16_UpperCase  |    96.92 ns |  1.783 ns |  3.260 ns | 0.0334 |     560 B |
| Base32_CrockfordWithPadding |   136.41 ns |  1.744 ns |  1.632 ns | 0.0138 |     232 B |
| Base36_LowerCase            | 8,297.76 ns | 12.181 ns | 10.172 ns |      - |     224 B |
| Base45_Default              |   119.01 ns |  2.155 ns |  2.725 ns | 0.0129 |     216 B |
| Base58_Bitcoin              | 7,333.59 ns | 46.523 ns | 43.518 ns | 0.0076 |     200 B |
| Base58_Monero               |   180.92 ns |  1.260 ns |  1.117 ns | 0.0119 |     200 B |
| Base62_Default              | 7,210.10 ns | 11.733 ns | 10.975 ns | 0.0076 |     192 B |
| Base85_Z85                  |   138.75 ns |  0.786 ns |  0.735 ns | 0.0110 |     184 B |
| Base256Emoji_Default        |   215.19 ns |  3.288 ns |  2.915 ns | 0.0162 |     272 B |

Decoding (80 character string, except Base45 which must use an 81 character string)

| Method                               | Mean        | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|------------------------------------- |------------:|----------:|----------:|-------:|-------:|----------:|
| DotNet_Base64                        |   104.14 ns |  1.107 ns |  1.035 ns | 0.0052 |      - |      88 B |
| Base2_Default                        |    94.64 ns |  0.598 ns |  0.559 ns | 0.0024 |      - |      40 B |
| Base8_Default                        |    94.38 ns |  1.376 ns |  1.149 ns | 0.0024 |      - |      40 B |
| Base16_UpperCase                     |    48.49 ns |  0.128 ns |  0.107 ns | 0.0038 |      - |      64 B |
| Base16_UpperCase_TextReader          |   251.52 ns |  4.994 ns | 10.642 ns | 0.4966 | 0.0155 |    8312 B |
| Multibase_Base16_UpperCase           |    51.67 ns |  0.143 ns |  0.126 ns | 0.0038 |      - |      64 B |
| Multibase_TryDecode_Base16_UpperCase |    45.49 ns |  0.090 ns |  0.075 ns |      - |      - |         - |
| Base32_Crockford                     |    96.56 ns |  0.793 ns |  0.619 ns | 0.0048 |      - |      80 B |
| Base36_LowerCase                     | 3,167.34 ns | 22.424 ns | 18.725 ns | 0.0038 |      - |      80 B |
| Base45_Default                       |    65.67 ns |  0.699 ns |  0.654 ns | 0.0048 |      - |      80 B |
| Base58_Bitcoin                       | 3,545.49 ns | 11.770 ns |  9.829 ns | 0.0038 |      - |      88 B |
| Base58_Monero                        |    75.63 ns |  1.222 ns |  1.083 ns | 0.0052 |      - |      88 B |
| Base62_Default                       | 3,759.70 ns | 63.712 ns | 59.596 ns | 0.0038 |      - |      88 B |
| Base85_Z85                           |   237.43 ns |  0.770 ns |  0.601 ns | 0.0052 |      - |      88 B |
| Base256Emoji_Default                 |   273.74 ns |  2.261 ns |  1.888 ns | 0.0062 |      - |     104 B |

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
