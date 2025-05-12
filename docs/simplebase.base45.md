# Base45

Namespace: SimpleBase

Base45 encoding/decoding implementation.

```csharp
public sealed class Base45 : INonAllocatingBaseCoder, IBaseCoder, IBaseStreamCoder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Base45](./simplebase.base45.md)<br>
Implements [INonAllocatingBaseCoder](./simplebase.inonallocatingbasecoder.md), [IBaseCoder](./simplebase.ibasecoder.md), [IBaseStreamCoder](./simplebase.ibasestreamcoder.md)

## Properties

### **Default**

Gets the default flavor.

```csharp
public static Base45 Default { get; }
```

#### Property Value

[Base45](./simplebase.base45.md)<br>

## Constructors

### **Base45(Base45Alphabet)**

Base45 encoding/decoding implementation.

```csharp
public Base45(Base45Alphabet alphabet)
```

#### Parameters

`alphabet` [Base45Alphabet](./simplebase.base45alphabet.md)<br>
Alphabet to use.

## Methods

### **Decode(ReadOnlySpan&lt;Char&gt;)**

```csharp
public Byte[] Decode(ReadOnlySpan<char> text)
```

#### Parameters

`text` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### **Encode(ReadOnlySpan&lt;Byte&gt;)**

```csharp
public string Encode(ReadOnlySpan<byte> bytes)
```

#### Parameters

`bytes` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

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

```csharp
public void Encode(Stream input, TextWriter output)
```

#### Parameters

`input` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>

`output` [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br>

### **EncodeAsync(Stream, TextWriter)**

```csharp
public Task EncodeAsync(Stream input, TextWriter output)
```

#### Parameters

`input` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>

`output` [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br>

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>

### **Decode(TextReader, Stream)**

```csharp
public void Decode(TextReader input, Stream output)
```

#### Parameters

`input` [TextReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.textreader)<br>

`output` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>

### **DecodeAsync(TextReader, Stream)**

```csharp
public Task DecodeAsync(TextReader input, Stream output)
```

#### Parameters

`input` [TextReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.textreader)<br>

`output` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
