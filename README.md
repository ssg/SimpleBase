SimpleBase
==========
This is my own take for exotic base encodings like Base32 and Base58. I started to write it in 2013 
as coding practice and kept it as a small pet project. I suggest anyone who wants to brush up 
their coding skills to give those encoding problems a shot. They turned out to be more challenging 
than I expected. To grasp the algorithms I had to get a pen and paper to see how the math worked.

Features
--------
 - Base32: RFC 4648, Crockford and Extended Hex (BASE32-HEX) alphabets with Crockford 
character substitution (or any other custom alphabets you might want to use)
 - Base58: Bitcoin, Ripple and Flickr alphabets (and any custom alphabet you might have)
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
// ...
byte[] myBuffer;
string result = Base32.Crockford.Encode(myBuffer, padding: true);
// you can also use "ExtendedHex" or "Rfc4648" as encoder flavors
```

Decode a Base32-encoded string:

```csharp
using SimpleBase;
// ...
string myText;
byte[] result = Base32.Crockford.Decode(myText);
// you can also use "ExtendedHex" or "Rfc4648" as decoder flavors
```

### Base58

Encode a byte array:

```csharp
using SimpleBase;
// ...
byte[] myBuffer;
string result = Base58.Bitcoin.Encode(myBuffer);
// you can also use "Ripple" or "Flickr" as encoder flavors
```

Decode a Base58-encoded string:

```csharp
using SimpleBase;
// ...
string myText;
byte[] result = Base58.Bitcoin.Decode(myText);
// you can also use "Ripple" or "Flickr" as decoder flavors
```

Benchmark Results
-----------------
Small buffer sizes are used (64 characters). They are closer to real life applications. Base58 
performs really bad in decoding of larger buffer sizes, due to exponential complexity of 
numeric base conversions.

1,000,000 iterations in Release build.

Implementation       | Encode | Decode
---------------------|--------|------------
Microsoft.NET Base64 | 0.15s  | 0.17s
SimpleBase Base32    | 0.36s (~2.5x slower) | 0.25s (~1.5x slower)
SimpleBase Base58    | 13.9s (~85x slower) | 12.1s (~70x slower)

Notes
-----
I'm sure there are areas for improvement. I didn't want to go further in optimizations which 
would hurt readability and extensibility. I might experiment on them in the future.

Test suite for Base32 isn't complete, I took most of it from RFC4648. Base58 really 
lacks a good spec or test vectors needed. I had to resort to using online converters to generate
preliminary test vectors.

Thanks
------
Chatting about this pet project with my friends [@detaybey](https://github.com/detaybey), [@vhallac](https://github.com/vhallac), [@alkimake](https://github.com/alkimake) and [@Utopians](https://github.com/Utopians) at one of our friend's birthday encouraged me to finish this. Thanks guys. Special thanks to my wife for unlimited tea and love.
