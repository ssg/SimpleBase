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
 - **Base32**: RFC 4648, BECH32, Crockford, z-base-32, Geohash, FileCoin and Extended Hex 
   (BASE32-HEX) flavors with Crockford character substitution, or any other 
   custom flavors.
 - **Base58**: Both the standard encoding (Bitcoin (BTC), Ripple (XRP), Flickr, and custom alphabets) and Monero (XMR)
   Base58 algorithms are supported. Also provides Base58Check and Avalanche CB58 encoding helpers.
 - **Base85**: Ascii85, Z85 and custom flavors. IPv6 encoding/decoding support.
 - **Base16**: UpperCase, LowerCase and ModHex flavors. An experimental hexadecimal 
   encoder/decoder just to see how far I 
   can take the optimizations compared to .NET's  implementations. It's quite 
   fast now. It could also be used as a replacement for `SoapHexBinary.Parse` although
   .NET has [`Convert.FromHexString()`](https://learn.microsoft.com/en-us/dotnet/api/system.convert.fromhexstring?view=net-5.0) method since .NET 5.
 - [Multibase](https://github.com/multiformats/multibase) support. All formats
   covered by SimpleBase including a few Base64 variants are supported.
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

### Base32

Encode a byte array:

```csharp
using SimpleBase;

byte[] myBuffer;
string result = Base32.Crockford.Encode(myBuffer, padding: true);
// you can also use "ExtendedHex" or "Rfc4648" as encoder flavors
```

Decode a Base32-encoded string:

```csharp
using SimpleBase;

string myText = ...
byte[] result = Base32.Crockford.Decode(myText);
```

### Base58

Encode a byte array:

```csharp
byte[] myBuffer = ...
string result = Base58.Bitcoin.Encode(myBuffer);
// you can also use "Ripple" or "Flickr" as encoder flavors
```

Decode a Base58-encoded string:

```csharp
string myText = ...
byte[] result = Base58.Bitcoin.Decode(myText);
```

Encode a Base58Check address:

```csharp
byte[] address = ...
byte version = 1; // P2PKH address
string result = Base58.Bitcoin.EncodeCheck(address, version);
```

Decode a Base58Check address:

```csharp
string address = ...
Span<byte> buffer = new byte[maxAddressLength];
if (Base58.Bitcoin.TryDecodeCheck(address, buffer, out byte version, out int numBytesWritten));
buffer = buffer[..numBytesWritten]; // use only the written portion of the buffer
```

Avalanche CB58 usage is pretty much the same except it doesn't have a separate
version field. Just use `EncodeCb58` and `TryDecodeCb58` methods instead. For 
encoding:

```
byte[] address = ...
byte version = 1;
string result = Base58.Bitcoin.EncodeCb58(address);
```

For decoding:

```csharp
string address = ...
Span<byte> buffer = new byte[maxAddressLength];
if (Base58.Bitcoin.TryDecodeCb58(address, buffer, out int numBytesWritten));
buffer = buffer[..numBytesWritten]; // use only the written portion of the buffer
```

### Base85

Encode a byte array to Ascii85 string:

```csharp
byte[] myBuffer = ...
string result = Base85.Ascii85.Encode(myBuffer);
// you can also use Z85 as a flavor
```

Decode an encoded Ascii85 string:

```csharp
string encodedString = ...
byte[] result = Base85.Ascii85.Decode(encodedString);
```

Both "zero" and "space" shortcuts are supported for Ascii85. Z85 is still 
vanilla.

### Base16

Encode a byte array to hex string:

 ```csharp
byte[] myBuffer = ...
string result = Base16.EncodeUpper(myBuffer); // encode to uppercase
// or 
string result = Base16.EncodeLower(myBuffer); // encode to lowercase
```

To decode a valid hex string:

```csharp
string text = ...
byte[] result = Base16.Decode(text); // decodes both upper and lowercase
```

### Stream Mode

Most encoding classes also support a stream mode that can work on streams, be 
it a network connection, a file or whatever you want. They are ideal for 
handling arbitrarily large data as they don't consume memory other than a small 
buffer when encoding or decoding. Their syntaxes are mostly identical. Text 
encoding decoding is done through a `TextReader`/`TextWriter` and the rest is 
read through a `Stream` interface. Here is a simple code that encodes a file to 
another file using Base85 encoding:

```csharp
using (var input = File.Open("somefile.bin"))
using (var output = File.Create("somefile.ascii85"))
using (var writer = new TextWriter(output)) // you can specify encoding here
{
  Base85.Ascii85.Encode(input, writer);
}
```

Decode works similarly. Here is a Base32 file decoder:

```csharp
using (var input = File.Open("somefile.b32"))
using (var output = File.Create("somefile.bin"))
using (var reader = new TextReader(input)) // specify encoding here
{
	Base32.Crockford.Decode(reader, output);
}
```

### Asynchronous Stream Mode
You can also encode/decode streams in asynchronous fashion:

```csharp
using (var input = File.Open("somefile.bin"))
using (var output = File.Create("somefile.ascii85"))
using (var writer = new TextWriter(output)) // you can specify encoding here
{
  await Base85.Ascii85.EncodeAsync(input, writer);
}
```

And the decode:

```csharp
using (var input = File.Open("somefile.b32"))
using (var output = File.Create("somefile.bin"))
using (var reader = new TextReader(input)) // specify encoding here
{
	await Base32.Crockford.DecodeAsync(reader, output);
}
```

### TryEncode/TryDecode
If you want to use an existing pre-allocated buffer to encode or decode without 
causing a GC allocation every time, you can make use of TryEncode/TryDecode 
methods which receive input, output buffers as parameters. 

Encoding is like this:

```csharp
byte[] input = [1, 2, 3, 4, 5];
int outputBufferSize = Base58.Bitcoin.GetSafeCharCountForEncoding(input);
var output = new char[outputBufferSize];

if (Base58.Bitcoin.TryEncode(input, output, out int numCharsWritten))
{
   // there you go
}
```

and decoding:

```csharp
string input = "... some bitcoin address ...";
int outputBufferSize = Base58.Bitcoin.GetSafeByteCountForDecoding(output);
var output = new byte[outputBufferSize];

if (Base58.Bitcoin.TryDecode(input, output, out int numBytesWritten))
{
    // et voila!
}
```

### Multibase encoding/decoding
In order to encode a Multibase string just specify the encoding
you want to use:

```csharp
byte[] input = [1, 2, 3, 4, 5];
string result = Multibase.Encode(input, MultibaseEncoding.Base32);
```

When decoding a multibase string, the encoding is automatically detected:

```csharp
string input = "... some encoded multibase string ...";
byte[] result = Multibase.Decode(input);
```

If you don't want decoding to raise an exception, use TryDecode() method instead:

```csharp
string input = "... some encoded multibase string ...";
byte[] output = new byte[outputBufferSize]; // enough the fit the decoded buffer
if (Multibase.TryDecode(input, output, out int numBytesWritten))
{
    // et voila!
}
```

Benchmark Results
-----------------
Small buffer sizes are used (64 characters). They are closer to real life 
applications. Base58 performs really bad in decoding of larger buffer sizes, 
due to polynomial complexity of numeric base conversions.

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3915)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.408
  [Host]     : .NET 8.0.15 (8.0.1525.16413), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.15 (8.0.1525.16413), X64 RyuJIT AVX2

Encoding (64 byte buffer)

| Method                                 | Mean      | Error    | StdDev   | Gen0   | Allocated |
|--------------------------------------- |----------:|---------:|---------:|-------:|----------:|
| DotNet_Base64                          |  35.09 ns | 0.756 ns | 0.870 ns | 0.0119 |     200 B |
| SimpleBase_Base16_UpperCase            |  84.53 ns | 1.608 ns | 1.504 ns | 0.0167 |     280 B |
| SimpleBase_Base32_CrockfordWithPadding | 150.97 ns | 2.071 ns | 1.730 ns | 0.0138 |     232 B |
| SimpleBase_Base85_Z85                  | 149.03 ns | 0.835 ns | 0.741 ns | 0.0110 |     184 B |
| SimpleBase_Base58_Bitcoin              |  44.61 ns | 0.449 ns | 0.350 ns | 0.0091 |     152 B |
| SimpleBase_Base58_Monero               | 206.80 ns | 3.761 ns | 3.518 ns | 0.0119 |     200 B |
| SimpleBase_Multibase_Base16_UpperCase  | 104.93 ns | 2.153 ns | 4.096 ns | 0.0334 |     560 B |

Decoding (80 character string)

| Method                                 | Mean        | Error     | StdDev    | Median      | Gen0   | Gen1   | Allocated |
|--------------------------------------- |------------:|----------:|----------:|------------:|-------:|-------:|----------:|
| DotNet_Base64                          |   105.91 ns |  2.082 ns |  1.739 ns |   106.08 ns | 0.0052 |      - |      88 B |
| SimpleBase_Base16_UpperCase            |    50.76 ns |  0.892 ns |  0.834 ns |    50.89 ns | 0.0038 |      - |      64 B |
| SimpleBase_Base16_UpperCase_TextReader |   269.39 ns |  5.597 ns | 16.239 ns |   262.44 ns | 0.5007 | 0.0153 |    8376 B |
| SimpleBase_Base32_Crockford            |   143.13 ns |  1.471 ns |  1.376 ns |   143.05 ns | 0.0048 |      - |      80 B |
| SimpleBase_Base85_Z85                  |   250.52 ns |  2.395 ns |  2.123 ns |   249.93 ns | 0.0052 |      - |      88 B |
| SimpleBase_Base58_Bitcoin              | 3,730.15 ns | 33.852 ns | 31.665 ns | 3,735.73 ns | 0.0038 |      - |      88 B |
| SimpleBase_Base58_Monero               |   130.67 ns |  2.680 ns |  2.632 ns |   129.60 ns | 0.0052 |      - |      88 B |
| SimpleBase_Multibase_Base16_UpperCase  |    50.53 ns |  0.228 ns |  0.191 ns |    50.56 ns | 0.0038 |      - |      64 B |

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
Thanks to all contributors (most up to date is on the GitHub sidebar) who
provided patches, and reported bugs.

Chatting about this pet project with my friends 
[@detaybey](https://github.com/detaybey), 
[@vhallac](https://github.com/vhallac), 
[@alkimake](https://github.com/alkimake) and 
[@Utopians](https://github.com/Utopians) at one of our friend's birthday 
encouraged me to finish this. Thanks guys.
