SimpleBase32
============

This is my own take for a Base32 encoder/decoder. I started to write it around a year ago as a coding practice and
kept it as a small pet project. I suggest anyone who wants to brush up their coding skills to give Base32 
a shot. It turned out to be more challenging than I expected. I had to get a pen and paper to see where 
bits go. I think in the end, I spent 20 hours on this in total.

Features
--------
 - Supports RFC 4648, Crockford and Extended Hex (BASE32-HEX) flavors.
 - Supports Crockford character substitution
 - Thread-safe
 - Simple to use

Usage
------

To encode:

    using SimpleBase32;
    // ...
    byte[] myBuffer;
    string result = Base32.Crockford.Encode(myBuffer, padding: true);
    // you can also use "ExtendedHex" or "Rfc4648" as encoder flavors

To decode:

    using SimpleBase32;
    // ...
    string myText;
    byte[] result = Base32.Crockford.Decode(myText);
    // you can also use "ExtendedHex" or "Rfc4648" as decoder flavors

Notes
-----
I'm sure there are areas for improvement. I included a benchmark suite against .NET's Base64 encoder/decoder. 
Base64 coding is simpler and Microsoft's implementation is very good in performance. Yet this suite is only ~7x
slower in encoding and ~2x slower in decoding on my machine. I didn't want to go further in optimizations
which would hurt readability and extensibility.

Test suite isn't complete, I took most of it from RFC4648. Increase to test coverage is appreciated.

Thanks
------
Chatting about this pet project with my friends @detaybey, @vhallac, @alkimake and @Utopians last night actually encouraged me to 
finish this. Thanks guys. Special thanks to my wife for unlimited tea and love.