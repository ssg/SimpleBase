# Base85

Namespace: SimpleBase

Base58 encoding/decoding class.

```csharp
public class Base85 : IBaseCoder, IBaseStreamCoder, INonAllocatingBaseCoder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Base85](./simplebase.base85.md)<br>
Implements [IBaseCoder](./simplebase.ibasecoder.md), [IBaseStreamCoder](./simplebase.ibasestreamcoder.md), [INonAllocatingBaseCoder](./simplebase.inonallocatingbasecoder.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

**Remarks:**

Initializes a new instance of the [Base85](./simplebase.base85.md) class
 using a custom alphabet.

## Properties

### **Z85**

Gets Z85 flavor of Base85.

```csharp
public static Base85 Z85 { get; }
```

#### Property Value

[Base85](./simplebase.base85.md)<br>

### **Ascii85**

Gets Ascii85 flavor of Base85.

```csharp
public static Base85 Ascii85 { get; }
```

#### Property Value

[Base85](./simplebase.base85.md)<br>

### **Rfc1924**

Gets RFC 1924 IPv6 flavor of Base85.

```csharp
public static Base85IPv6 Rfc1924 { get; }
```

#### Property Value

[Base85IPv6](./simplebase.base85ipv6.md)<br>

### **Alphabet**

Gets the encoding alphabet.

```csharp
public Base85Alphabet Alphabet { get; }
```

#### Property Value

[Base85Alphabet](./simplebase.base85alphabet.md)<br>

## Constructors

### **Base85(Base85Alphabet)**

Base58 encoding/decoding class.

```csharp
public Base85(Base85Alphabet alphabet)
```

#### Parameters

`alphabet` [Base85Alphabet](./simplebase.base85alphabet.md)<br>
Alphabet to use.

**Remarks:**

Initializes a new instance of the [Base85](./simplebase.base85.md) class
 using a custom alphabet.

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

Encode the given bytes into Base85.

```csharp
public string Encode(ReadOnlySpan<byte> bytes)
```

#### Parameters

`bytes` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Bytes to encode.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoded text.

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

### **Encode(Stream, TextWriter)**

Encode a given stream into a text writer.

```csharp
public void Encode(Stream input, TextWriter output)
```

#### Parameters

`input` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Input stream.

`output` [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br>
Output writer.

### **EncodeAsync(Stream, TextWriter)**

Encode a given stream into a text writer.

```csharp
public Task EncodeAsync(Stream input, TextWriter output)
```

#### Parameters

`input` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Input stream.

`output` [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br>
Output writer.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
A [Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) representing the asynchronous operation.

### **Decode(TextReader, Stream)**

Decode a text reader into a stream.

```csharp
public void Decode(TextReader input, Stream output)
```

#### Parameters

`input` [TextReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.textreader)<br>
Input reader.

`output` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Output stream.

### **DecodeAsync(TextReader, Stream)**

Decode a text reader into a stream.

```csharp
public Task DecodeAsync(TextReader input, Stream output)
```

#### Parameters

`input` [TextReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.textreader)<br>
Input reader.

`output` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Output stream.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
A [Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) representing the asynchronous operation.

### **Decode(ReadOnlySpan&lt;Char&gt;)**

Decode given characters into bytes.

```csharp
public Byte[] Decode(ReadOnlySpan<char> text)
```

#### Parameters

`text` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Characters to decode.

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Decoded bytes.

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
