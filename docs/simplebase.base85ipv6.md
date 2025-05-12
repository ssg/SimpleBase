# Base85IPv6

Namespace: SimpleBase

Base85 implementation with additional IPv6 coding functions.

```csharp
public class Base85IPv6 : Base85, IBaseCoder, IBaseStreamCoder, INonAllocatingBaseCoder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Base85](./simplebase.base85.md) → [Base85IPv6](./simplebase.base85ipv6.md)<br>
Implements [IBaseCoder](./simplebase.ibasecoder.md), [IBaseStreamCoder](./simplebase.ibasestreamcoder.md), [INonAllocatingBaseCoder](./simplebase.inonallocatingbasecoder.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

**Remarks:**

RFC 1924 sucks, arguably because it's a very early proposal in the history of IPv6:
 - It contains special chars: It's prone to be confused with other syntactical elements.
 It can even cause security issues due to poor escaping, let alone UX problems.
 - Length gains are usually marginal: IPv6 uses zero elimination to reduce the address representation.
 - Slow. The algorithm is division based, instead of faster bitwise operations.
 So, that's why I only included a proof of concept implementation instead of working on optimizing it.
 RFC 1924 should die, and this code should only be used to support some obscure standard or code somewhere.

## Properties

### **Alphabet**

Gets the encoding alphabet.

```csharp
public Base85Alphabet Alphabet { get; }
```

#### Property Value

[Base85Alphabet](./simplebase.base85alphabet.md)<br>

## Constructors

### **Base85IPv6(Base85Alphabet)**

Base85 implementation with additional IPv6 coding functions.

```csharp
public Base85IPv6(Base85Alphabet alphabet)
```

#### Parameters

`alphabet` [Base85Alphabet](./simplebase.base85alphabet.md)<br>
Coding alphabet.

**Remarks:**

RFC 1924 sucks, arguably because it's a very early proposal in the history of IPv6:
 - It contains special chars: It's prone to be confused with other syntactical elements.
 It can even cause security issues due to poor escaping, let alone UX problems.
 - Length gains are usually marginal: IPv6 uses zero elimination to reduce the address representation.
 - Slow. The algorithm is division based, instead of faster bitwise operations.
 So, that's why I only included a proof of concept implementation instead of working on optimizing it.
 RFC 1924 should die, and this code should only be used to support some obscure standard or code somewhere.

## Methods

### **EncodeIPv6(IPAddress)**

Encode IPv6 address into RFC 1924 Base85 text.

```csharp
public string EncodeIPv6(IPAddress ip)
```

#### Parameters

`ip` IPAddress<br>
IPv6 address.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoded text.

### **DecodeIPv6(String)**

Decode an RFC 1924 encoded text into an IPv6 address.

```csharp
public IPAddress DecodeIPv6(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoded text.

#### Returns

IPAddress<br>
Decoded IPv6 address.

### **TryDecodeIPv6(String, IPAddress&)**

Try decoding an RFC 1924 encoded text into an IPv6 address.

```csharp
public bool TryDecodeIPv6(string text, IPAddress& ip)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoded text.

`ip` IPAddress&<br>
Resulting IPv6 address.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if successful, false otherwise.
