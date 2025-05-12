# Base58

Namespace: SimpleBase

Base58 Encoding/Decoding implementation.

```csharp
public sealed class Base58 : IBaseCoder, INonAllocatingBaseCoder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Base58](./simplebase.base58.md)<br>
Implements [IBaseCoder](./simplebase.ibasecoder.md), [INonAllocatingBaseCoder](./simplebase.inonallocatingbasecoder.md)

**Remarks:**

Base58 doesn't implement a Stream-based interface because it's not feasible to use
 on large buffers.

## Properties

### **Bitcoin**

Gets Bitcoin flavor.

```csharp
public static Base58 Bitcoin { get; }
```

#### Property Value

[Base58](./simplebase.base58.md)<br>

### **Ripple**

Gets Ripple flavor.

```csharp
public static Base58 Ripple { get; }
```

#### Property Value

[Base58](./simplebase.base58.md)<br>

### **Flickr**

Gets Flickr flavor.

```csharp
public static Base58 Flickr { get; }
```

#### Property Value

[Base58](./simplebase.base58.md)<br>

### **Monero**

Gets Monero flavor.

```csharp
public static MoneroBase58 Monero { get; }
```

#### Property Value

[MoneroBase58](./simplebase.monerobase58.md)<br>

**Remarks:**

This uses a different algorithm for Base58 encoding. See [MoneroBase58](./simplebase.monerobase58.md) for details.

### **Alphabet**

Gets the encoding alphabet.

```csharp
public Base58Alphabet Alphabet { get; }
```

#### Property Value

[Base58Alphabet](./simplebase.base58alphabet.md)<br>

### **ZeroChar**

Gets the character for zero.

```csharp
public char ZeroChar { get; }
```

#### Property Value

[Char](https://docs.microsoft.com/en-us/dotnet/api/system.char)<br>

## Constructors

### **Base58(Base58Alphabet)**

Base58 Encoding/Decoding implementation.

```csharp
public Base58(Base58Alphabet alphabet)
```

#### Parameters

`alphabet` [Base58Alphabet](./simplebase.base58alphabet.md)<br>
Alphabet to use.

**Remarks:**

Base58 doesn't implement a Stream-based interface because it's not feasible to use
 on large buffers.

## Methods

### **GetSafeByteCountForDecoding(Int32, Int32)**

Retrieve safe byte count while avoiding multiple counting operations.

```csharp
public static int GetSafeByteCountForDecoding(int textLen, int numZeroes)
```

#### Parameters

`textLen` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Length of text.

`numZeroes` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of prefix zeroes.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Length of safe allocation.

### **GetSafeByteCountForDecoding(ReadOnlySpan&lt;Char&gt;)**

```csharp
public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
```

#### Parameters

`text` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **GetSafeCharCountForEncoding(ReadOnlySpan&lt;Byte&gt;)**

```csharp
public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> bytes)
```

#### Parameters

`bytes` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **EncodeCheck(ReadOnlySpan&lt;Byte&gt;, Byte)**

Generate a Base58Check string out of a version and payload.

```csharp
public string EncodeCheck(ReadOnlySpan<byte> payload, byte version)
```

#### Parameters

`payload` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Address data.

`version` [Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Address version.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Base58Check address.

### **TryDecodeCheck(ReadOnlySpan&lt;Char&gt;, Span&lt;Byte&gt;, Byte&, Int32&)**

Try to decode and verify a Base58Check address.

```csharp
public bool TryDecodeCheck(ReadOnlySpan<char> address, Span<byte> payload, Byte& version, Int32& bytesWritten)
```

#### Parameters

`address` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Address string.

`payload` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
Output address buffer.

`version` [Byte&](https://docs.microsoft.com/en-us/dotnet/api/system.byte&)<br>
Address version.

`bytesWritten` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
Number of bytes written in the output payload.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if address was decoded successfully and passed validation. False, otherwise.

### **EncodeCb58(ReadOnlySpan&lt;Byte&gt;)**

Generate an Avalanche CB58 string out of a version and payload.

```csharp
public string EncodeCb58(ReadOnlySpan<byte> payload)
```

#### Parameters

`payload` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Address data.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
CB58 address.

### **TryDecodeCb58(ReadOnlySpan&lt;Char&gt;, Span&lt;Byte&gt;, Int32&)**

Try to decode and verify an Avalanche CB58 address.

```csharp
public bool TryDecodeCb58(ReadOnlySpan<char> address, Span<byte> payload, Int32& bytesWritten)
```

#### Parameters

`address` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Address string.

`payload` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
Output address buffer.

`bytesWritten` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
Number of bytes written in the output payload.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if address was decoded successfully and passed validation. False, otherwise.

### **Encode(ReadOnlySpan&lt;Byte&gt;)**

Encode to Base58 representation.

```csharp
public string Encode(ReadOnlySpan<byte> bytes)
```

#### Parameters

`bytes` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Bytes to encode.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoded string.

### **Decode(ReadOnlySpan&lt;Char&gt;)**

Decode a Base58 representation.

```csharp
public Byte[] Decode(ReadOnlySpan<char> text)
```

#### Parameters

`text` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Base58 encoded text.

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Decoded bytes.

### **TryEncode(ReadOnlySpan&lt;Byte&gt;, Span&lt;Char&gt;, Int32&)**

```csharp
public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, Int32& numCharsWritten)
```

#### Parameters

`input` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`output` [Span&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

`numCharsWritten` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **TryDecode(ReadOnlySpan&lt;Char&gt;, Span&lt;Byte&gt;, Int32&)**

```csharp
public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, Int32& bytesWritten)
```

#### Parameters

`input` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`output` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

`bytesWritten` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
