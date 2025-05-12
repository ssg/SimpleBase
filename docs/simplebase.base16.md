# Base16

Namespace: SimpleBase

Base16 encoding/decoding.

```csharp
public sealed class Base16 : IBaseCoder, IBaseStreamCoder, INonAllocatingBaseCoder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Base16](./simplebase.base16.md)<br>
Implements [IBaseCoder](./simplebase.ibasecoder.md), [IBaseStreamCoder](./simplebase.ibasestreamcoder.md), [INonAllocatingBaseCoder](./simplebase.inonallocatingbasecoder.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

**Remarks:**

Initializes a new instance of the [Base16](./simplebase.base16.md) class.

## Properties

### **UpperCase**

Gets upper case Base16 encoder. Decoding is case-insensitive.

```csharp
public static Base16 UpperCase { get; }
```

#### Property Value

[Base16](./simplebase.base16.md)<br>

### **LowerCase**

Gets lower case Base16 encoder. Decoding is case-insensitive.

```csharp
public static Base16 LowerCase { get; }
```

#### Property Value

[Base16](./simplebase.base16.md)<br>

### **ModHex**

Gets lower case Base16 encoder. Decoding is case-insensitive.

```csharp
public static Base16 ModHex { get; }
```

#### Property Value

[Base16](./simplebase.base16.md)<br>

### **Alphabet**

Gets the alphabet used by the encoder.

```csharp
public Base16Alphabet Alphabet { get; }
```

#### Property Value

[Base16Alphabet](./simplebase.base16alphabet.md)<br>

## Constructors

### **Base16(Base16Alphabet)**

Base16 encoding/decoding.

```csharp
public Base16(Base16Alphabet alphabet)
```

#### Parameters

`alphabet` [Base16Alphabet](./simplebase.base16alphabet.md)<br>
Alphabet to use.

**Remarks:**

Initializes a new instance of the [Base16](./simplebase.base16.md) class.

## Methods

### **Decode(String)**

Decode Upper/Lowercase Base16 text into bytes.

```csharp
public static Span<byte> Decode(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Hex string.

#### Returns

[Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
Decoded bytes.

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

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

### **DecodeAsync(TextReader, Stream)**

Decode Base16 text through streams for generic use. Stream based variant tries to consume
 as little memory as possible, and relies of .NET's own underlying buffering mechanisms,
 contrary to their buffer-based versions.

```csharp
public Task DecodeAsync(TextReader input, Stream output)
```

#### Parameters

`input` [TextReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.textreader)<br>
Stream that the encoded bytes would be read from.

`output` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Stream where decoded bytes will be written to.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
Task that represents the async operation.

### **Decode(TextReader, Stream)**

Decode Base16 text through streams for generic use. Stream based variant tries to consume
 as little memory as possible, and relies of .NET's own underlying buffering mechanisms,
 contrary to their buffer-based versions.

```csharp
public void Decode(TextReader input, Stream output)
```

#### Parameters

`input` [TextReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.textreader)<br>
Stream that the encoded bytes would be read from.

`output` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Stream where decoded bytes will be written to.

### **Encode(Stream, TextWriter)**

Encodes stream of bytes into a Base16 text.

```csharp
public void Encode(Stream input, TextWriter output)
```

#### Parameters

`input` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Stream that provides bytes to be encoded.

`output` [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br>
Stream that the encoded text is written to.

### **EncodeAsync(Stream, TextWriter)**

Encodes stream of bytes into a Base16 text.

```csharp
public Task EncodeAsync(Stream input, TextWriter output)
```

#### Parameters

`input` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Stream that provides bytes to be encoded.

`output` [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br>
Stream that the encoded text is written to.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
A [Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) representing the asynchronous operation.

### **Decode(ReadOnlySpan&lt;Char&gt;)**

Decode Base16 text into bytes.

```csharp
public Byte[] Decode(ReadOnlySpan<char> text)
```

#### Parameters

`text` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Base16 text.

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Decoded bytes.

### **TryDecode(ReadOnlySpan&lt;Char&gt;, Span&lt;Byte&gt;, Int32&)**

```csharp
public bool TryDecode(ReadOnlySpan<char> text, Span<byte> output, Int32& bytesWritten)
```

#### Parameters

`text` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`output` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

`bytesWritten` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Encode(ReadOnlySpan&lt;Byte&gt;)**

Encode to Base16 representation.

```csharp
public string Encode(ReadOnlySpan<byte> bytes)
```

#### Parameters

`bytes` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Bytes to encode.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Base16 string.

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
