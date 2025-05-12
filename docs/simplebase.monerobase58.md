# MoneroBase58

Namespace: SimpleBase

Monero variant of Base58 Encoding/Decoding algorithm. Differently from other Base58 implementations,
 Monero encodes using 8-byte blocks and converts them into 11-byte blocks instead of going byte-by-byte.
 This makes Monero a bit less algorihmically complex. If the block size is smaller than 11 bytes, the
 rest is padded with encoded zeroes ("1" on Monero Base58 alphabet).

```csharp
public sealed class MoneroBase58 : IBaseCoder, INonAllocatingBaseCoder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [MoneroBase58](./simplebase.monerobase58.md)<br>
Implements [IBaseCoder](./simplebase.ibasecoder.md), [INonAllocatingBaseCoder](./simplebase.inonallocatingbasecoder.md)

## Properties

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

### **MoneroBase58(Base58Alphabet)**

Monero variant of Base58 Encoding/Decoding algorithm. Differently from other Base58 implementations,
 Monero encodes using 8-byte blocks and converts them into 11-byte blocks instead of going byte-by-byte.
 This makes Monero a bit less algorihmically complex. If the block size is smaller than 11 bytes, the
 rest is padded with encoded zeroes ("1" on Monero Base58 alphabet).

```csharp
public MoneroBase58(Base58Alphabet alphabet)
```

#### Parameters

`alphabet` [Base58Alphabet](./simplebase.base58alphabet.md)<br>
An optional custom alphabet to use. By default, monero uses Bitcoin alphabet.

### **MoneroBase58()**

Initializes a new instance of the [MoneroBase58](./simplebase.monerobase58.md) class

```csharp
public MoneroBase58()
```

## Methods

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
