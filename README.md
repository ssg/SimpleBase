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
 - An experimental Base16 just to see how far I can take the optimizations compared to .NET's 
 implementations.
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

string myText;
byte[] result = Base32.Crockford.Decode(myText);
// you can also use "ExtendedHex" or "Rfc4648" as decoder flavors
```

### Base58

Encode a byte array:

```csharp
using SimpleBase;

byte[] myBuffer;
string result = Base58.Bitcoin.Encode(myBuffer);
// you can also use "Ripple" or "Flickr" as encoder flavors
```

Decode a Base58-encoded string:

```csharp
using SimpleBase;

string myText;
byte[] result = Base58.Bitcoin.Decode(myText);
// you can also use "Ripple" or "Flickr" as decoder flavors
```

### Base16

Encode a byte array to hex string:

```csharp
using SimpleBase;

string result = Base16.EncodeUpper(myBuffer); // encode to uppercase
// or 
string result = Base16.EncodeLower(myBuffer); // encode to lowercase
```

To decode a valid hex string:

```csharp
using SimpleBase;

byte[] result = Base16.Decode(text); // decodes both upper and lowercase
```

Benchmark Results
-----------------
Small buffer sizes are used (64 characters). They are closer to real life applications. Base58 
performs really bad in decoding of larger buffer sizes, due to exponential complexity of 
numeric base conversions.

1,000,000 iterations on 64 byte buffer (encode) / 64 character string (decode)

Implementation              | Growth | Encode                   | Decode
----------------------------|--------|--------------------------|------------------
.NET Framework Base64       | 1.33x  | 0.14                     | 0.20
SimpleBase Base16           | 2x     | 0.16 (1.1x slower)       | 0.19 (about the same)
SimpleBase Base32 Crockford | 1.6x   | 0.33 (2.4x slower)       | 0.19 (about the same)
SimpleBase Base58           | 1.38x  | 11.02 (78.7x slower)     | 5.71 (28.6x slower)

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
