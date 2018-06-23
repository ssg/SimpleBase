SimpleBase
==========
[![NuGet Version](https://img.shields.io/nuget/v/SimpleBase.svg)](https://www.nuget.org/packages/SimpleBase/)
[![Build Status](https://travis-ci.org/ssg/SimpleBase.svg?branch=master)](https://travis-ci.org/ssg/SimpleBase)

This is my own take for exotic base encodings like Base32, Base58 and Base85. I started to write it in 2013 
as coding practice and kept it as a small pet project. I suggest anyone who wants to brush up 
their coding skills to give those encoding problems a shot. They turned out to be more challenging 
than I expected. To grasp the algorithms I had to get a pen and paper to see how the math worked.

Features
--------
 - Base32: RFC 4648, Crockford and Extended Hex (BASE32-HEX) flavors with Crockford 
character substitution, or any other custom alphabet you might want to use.
 - Base58: Bitcoin, Ripple, Flickr and custom flavors.
 - Base85: Ascii85, Z85 and custom flavors.
 - Base16: An experimental hexadecimal encoder/decoder just to see how far I can take 
 the optimizations compared to .NET's  implementations. It's quite fast now. It can also be used as a replacement for `SoapHexBinary.Parse` method since it's missing from .NET Core.
 - Thread-safe
 - Simple to use

NuGet
------
To install it from NuGet:

  `Install-Package SimpleBase`

Usage
------------

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
// you can also use "ExtendedHex" or "Rfc4648" as decoder flavors
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
// you can also use "Ripple" or "Flickr" as decoder flavors
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
// you can also use Z85 as a flavor
```

Both "zero" and "space" shortcuts are supported for Ascii85. Z85 is still vanilla.

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

Benchmark Results
-----------------
Small buffer sizes are used (64 characters). They are closer to real life applications. Base58 
performs really bad in decoding of larger buffer sizes, due to polynomial complexity of 
numeric base conversions.

1,000,000 iterations
64 byte buffer for encoding
80 character string for decoding

Implementation              | Growth | Encode                   | Decode
----------------------------|--------|--------------------------|------------------
.NET Framework Base64       | 1.33x  | 0.09                     | 0.20
SimpleBase Base16           | 2x     | 0.13 (1.5x slower)       | 0.09 (2.3x faster! YAY!)
SimpleBase Base32 Crockford | 1.6x   | 0.26 (3x slower)         | 0.18 (1.1x faster! YAY!)
SimpleBase Base85 Z85       | 1.25x  | 0.18 (2x slower)         | 0.25 (1.2x slower)
SimpleBase Base58           | 1.38x  | 6.07 (68.4x slower)      | 5.43 (27.5x slower)

Notes
-----
I'm sure there are areas for improvement. I didn't want to go further in optimizations which 
would hurt readability and extensibility. I might experiment on them in the future.

Test suite for Base32 isn't complete, I took most of it from RFC4648. Base58 really 
lacks a good spec or test vectors needed. I had to resort to using online converters to generate
preliminary test vectors.

It's interesting that I wasn't able to reach .NET Base64's performance with Base16 with a straightforward
managed code despite that it's much simpler. I was only able to match it after I converted Base16 to unsafe code with good 
independent interleaving so CPU pipeline optimizations could take place. Still not satisfied though.
Is .NET's Base64 implementation native? Perhaps.

Thanks
------
Chatting about this pet project with my friends [@detaybey](https://github.com/detaybey), 
[@vhallac](https://github.com/vhallac), [@alkimake](https://github.com/alkimake) and 
[@Utopians](https://github.com/Utopians) at one of our friend's birthday encouraged me to 
finish this. Thanks guys. Special thanks to my wife for unlimited tea and love.
