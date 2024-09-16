# 4.0.2

## Fixes
- Fixes #59 - Base32's `Encode(ulong)` and `DecodeUInt64()` works consistently among platforms with different endianness

# 4.0.1

## Fixes
- Fixes #58 - `Encode(long)` failing -- reported by Pascal Schwarz <@pschwarzpp> 

# 4.0.0

## Breaking changes
- This version is built with .NET 6 SDK.
- Benchmark now uses BenchmarkDotNet.
- Changed interface names from Encoder to Coder to signify encoding and
  decoding functionality better.
- Removed obsolete methods.
- Simple (aka allocating) versions of `Decode()` will now return `byte[]`'s instead of `Span<byte>`'s for correct 
  ownership semantics. It's even possible that some copying may be avoided in certain scenarios.
- `Base16.TryDecode()` doesn't throw on invalid input, but returns `false` instead.
- `Base32.Decode()` throws separate exceptions for encountered failures.

## New features
- Added [Bech32](https://en.bitcoin.it/wiki/Bech32) flavor to Base32 
- Added RFC 1924 (IPv6) flavor to Base85 along with 
  EncodeIpv6 and DecodeIpv6 functions https://tools.ietf.org/html/rfc1924
- Added `Base58.Bitcoin.EncodeCheck()` and `Base58.Bitcoin.TryDecodeCheck()` methods.
- Added `Base58.Bitcoin.EncodeCb58()` and `Base58.Bitcoin.TryDecodeCb58()` methods.

## Improvements
- Added more buffer overflow detection to Base32 coder
- Removed all unsafe code. New Span<T>-based optimizations make the code come close to unsafe perf.
- Removed slow and hard to read optimizations like bit shift operations for multiplication and division
  where compiler almost always does a better job of optimizing.

## Fixes 
- Fixed output buffer was too small error for certain Base58 cases.
- Avoid redundant memory copy operations
