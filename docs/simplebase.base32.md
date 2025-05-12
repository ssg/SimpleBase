# Base32

Namespace: SimpleBase

Base32 encoding/decoding functions.

```csharp
public sealed class Base32 : IBaseCoder, IBaseStreamCoder, INonAllocatingBaseCoder, INumericBaseCoder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Base32](./simplebase.base32.md)<br>
Implements [IBaseCoder](./simplebase.ibasecoder.md), [IBaseStreamCoder](./simplebase.ibasestreamcoder.md), [INonAllocatingBaseCoder](./simplebase.inonallocatingbasecoder.md), [INumericBaseCoder](./simplebase.inumericbasecoder.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Crockford**

Gets Douglas Crockford's Base32 flavor with substitution characters.

```csharp
public static Base32 Crockford { get; }
```

#### Property Value

[Base32](./simplebase.base32.md)<br>

### **Rfc4648**

Gets RFC 4648 variant of Base32 coder.

```csharp
public static Base32 Rfc4648 { get; }
```

#### Property Value

[Base32](./simplebase.base32.md)<br>

### **ExtendedHex**

Gets Extended Hex variant of Base32 coder.

```csharp
public static Base32 ExtendedHex { get; }
```

#### Property Value

[Base32](./simplebase.base32.md)<br>

**Remarks:**

Also from RFC 4648.

### **ExtendedHexLower**

Gets Extended Hex variant of Base32 coder.

```csharp
public static Base32 ExtendedHexLower { get; }
```

#### Property Value

[Base32](./simplebase.base32.md)<br>

**Remarks:**

Also from RFC 4648.

### **ZBase32**

Gets z-base-32 variant of Base32 coder.

```csharp
public static Base32 ZBase32 { get; }
```

#### Property Value

[Base32](./simplebase.base32.md)<br>

**Remarks:**

This variant is used in Mnet, ZRTP and Tahoe-LAFS.

### **Geohash**

Gets Geohash variant of Base32 coder.

```csharp
public static Base32 Geohash { get; }
```

#### Property Value

[Base32](./simplebase.base32.md)<br>

### **Bech32**

Gets Bech32 variant of Base32 coder.

```csharp
public static Base32 Bech32 { get; }
```

#### Property Value

[Base32](./simplebase.base32.md)<br>

### **FileCoin**

Gets FileCoin variant of Base32 coder Also known as RFC 4648 lowercase.

```csharp
public static Base32 FileCoin { get; }
```

#### Property Value

[Base32](./simplebase.base32.md)<br>

### **Alphabet**

Gets the encoding alphabet.

```csharp
public Base32Alphabet Alphabet { get; }
```

#### Property Value

[Base32Alphabet](./simplebase.base32alphabet.md)<br>

## Constructors

### **Base32(Base32Alphabet)**

Initializes a new instance of the [Base32](./simplebase.base32.md) class with a
 custom alphabet.

```csharp
public Base32(Base32Alphabet alphabet)
```

#### Parameters

`alphabet` [Base32Alphabet](./simplebase.base32alphabet.md)<br>
Alphabet to use.

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
public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> buffer)
```

#### Parameters

`buffer` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Encode(Int64)**

```csharp
public string Encode(long number)
```

#### Parameters

`number` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Encode(UInt64)**

```csharp
public string Encode(ulong number)
```

#### Parameters

`number` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **DecodeUInt64(String)**

```csharp
public ulong DecodeUInt64(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **TryDecodeUInt64(String, UInt64&)**

```csharp
public bool TryDecodeUInt64(string text, UInt64& number)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`number` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **DecodeInt64(String)**

```csharp
public long DecodeInt64(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **Encode(ReadOnlySpan&lt;Byte&gt;)**

Encode a memory span into a Base32 string without padding.

```csharp
public string Encode(ReadOnlySpan<byte> bytes)
```

#### Parameters

`bytes` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Buffer to be encoded.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoded string.

### **Encode(ReadOnlySpan&lt;Byte&gt;, Boolean)**

Encode a memory span into a Base32 string.

```csharp
public string Encode(ReadOnlySpan<byte> bytes, bool padding)
```

#### Parameters

`bytes` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Buffer to be encoded.

`padding` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Append padding characters in the output.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoded string.

### **Decode(ReadOnlySpan&lt;Char&gt;)**

Decode a Base32 encoded string into bytes.

```csharp
public Byte[] Decode(ReadOnlySpan<char> text)
```

#### Parameters

`text` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Encoded Base32 string.

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Decoded bytes.

### **Encode(Stream, TextWriter)**

Encode a binary stream to a Base32 text stream without padding.

```csharp
public void Encode(Stream input, TextWriter output)
```

#### Parameters

`input` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Input bytes.

`output` [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br>
The writer the output is written to.

### **Encode(Stream, TextWriter, Boolean)**

Encode a binary stream to a Base32 text stream.

```csharp
public void Encode(Stream input, TextWriter output, bool padding)
```

#### Parameters

`input` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Input bytes.

`output` [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br>
The writer the output is written to.

`padding` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to use padding at the end of the output.

### **EncodeAsync(Stream, TextWriter)**

Encode a binary stream to a Base32 text stream without padding.

```csharp
public Task EncodeAsync(Stream input, TextWriter output)
```

#### Parameters

`input` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Input bytes.

`output` [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br>
The writer the output is written to.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
A [Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) representing the asynchronous operation.

### **EncodeAsync(Stream, TextWriter, Boolean)**

Encode a binary stream to a Base32 text stream.

```csharp
public Task EncodeAsync(Stream input, TextWriter output, bool padding)
```

#### Parameters

`input` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Input bytes.

`output` [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br>
The writer the output is written to.

`padding` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to use padding at the end of the output.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
A [Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) representing the asynchronous operation.

### **Decode(TextReader, Stream)**

Decode a text stream into a binary stream.

```csharp
public void Decode(TextReader input, Stream output)
```

#### Parameters

`input` [TextReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.textreader)<br>
TextReader open on the stream.

`output` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Binary output stream.

### **DecodeAsync(TextReader, Stream)**

Decode a text stream into a binary stream.

```csharp
public Task DecodeAsync(TextReader input, Stream output)
```

#### Parameters

`input` [TextReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.textreader)<br>
TextReader open on the stream.

`output` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Binary output stream.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
A [Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) representing the asynchronous operation.

### **TryEncode(ReadOnlySpan&lt;Byte&gt;, Span&lt;Char&gt;, Int32&)**

```csharp
public bool TryEncode(ReadOnlySpan<byte> bytes, Span<char> output, Int32& numCharsWritten)
```

#### Parameters

`bytes` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`output` [Span&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

`numCharsWritten` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **TryEncode(ReadOnlySpan&lt;Byte&gt;, Span&lt;Char&gt;, Boolean, Int32&)**

Encode to the given preallocated buffer.

```csharp
public bool TryEncode(ReadOnlySpan<byte> bytes, Span<char> output, bool padding, Int32& numCharsWritten)
```

#### Parameters

`bytes` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Input bytes.

`output` [Span&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
Output buffer.

`padding` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to use padding characters at the end.

`numCharsWritten` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
Number of characters written to the output.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if encoding is successful, false if the output is invalid.

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
