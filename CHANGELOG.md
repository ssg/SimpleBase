# 5.5.1

## Improvements
- Remove dependency to System.Memory

# 5.5.0

## New features
- `EncodeCheck` and `TryDecodeCheck` variants with variable length version/prefix buffers
- `EncodeCheckSkipZeroes` that encodes for address formats like Tezos

# 5.4.1

## Fixes
- Remove unused function from CodingAlphabet

# 5.4.0

## New features
- CodingAlphabet now supports case-insensitivity

## Improvements
- Multibase decoding now supports case-insensitive decoding
- Base36 decoding is now case-insensitive

## Fixes
- Base32 case-insensitive decoding now works correctly

# 5.3.0

## New features
- AOT and trimming compatibility
- Base2, Base8, and Base10 support

## Improvements
- `Base58` now uses `DividingCoder` under the hood for less code duplication
- Several implementations now take bytesWritten into account when returning buffers, reducing the possibility of returning a buffer larger than necessary.

# 5.2.0

## New features
- Multibase support for Base36 (upper and lower)

# 5.1.0

## New features
- Base36
- Base256Emoji

# 5.0.0

## Breaking changes
- TryDecode/TryEncode methods no longer throw
- Base85 methods with `Ipv6` in them renamed to `IPv6` to match with .NET
- `numBytesWritten` parameters have all been renamed to `bytesWritten` to match with .NET
- The target framework was changed to .NET 8.0 around 4.2.0, but the version change did not
  reflect that breaking change, although in practice it shouldn't cause many issues.

## New features
- Base62
- Base45
- Multibase now supports Base45
- Base32 now has a non-throwing `TryDecodeUInt64()` method

## Improvements
- `Multibase.Encode()` now allocates less memory

# 4.3.0

## New features
- Added Multibase support that supports several Base16, Base32, Base58, and Base64 variants.

## Improvements
- Eliminated more memory allocations (by @Henr1k80))

# 4.2.0

## New features
- Monero Base58 algorithm support with `MoneroBase58` class. It can be accessed as `Base58.Monero`

## Improvements
- Eliminate some memory allocations

## Fixes
- Throw `ArgumentOutOfRangeException` with correct parameters in `Base32.DecodeInt64()`

# 4.1.0

## Improvements
- Reduce heap allocations (by @Henr1k80)

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
