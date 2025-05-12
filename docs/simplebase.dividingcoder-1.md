# DividingCoder&lt;TAlphabet&gt;

Namespace: SimpleBase

Generic dividing Encoding/Decoding implementation to be used by other dividing encoders.

```csharp
public abstract class DividingCoder<TAlphabet> : IBaseCoder, INonAllocatingBaseCoder
```

#### Type Parameters

`TAlphabet`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DividingCoder&lt;TAlphabet&gt;](./simplebase.dividingcoder-1.md)<br>
Implements [IBaseCoder](./simplebase.ibasecoder.md), [INonAllocatingBaseCoder](./simplebase.inonallocatingbasecoder.md)

**Remarks:**

This isn't used by Base58 because it handles zero-prefixes differently than other encodings.

## Properties

### **Alphabet**

Gets the encoding alphabet.

```csharp
public TAlphabet Alphabet { get; }
```

#### Property Value

TAlphabet<br>

## Constructors

### **DividingCoder(TAlphabet)**

Creates a new instance of DividingCoder with a given alphabet.

```csharp
public DividingCoder(TAlphabet alphabet)
```

#### Parameters

`alphabet` TAlphabet<br>
Alphabet to use. The length of alphabet is used as a divisor.

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

Encode to given base

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

Decode from a given base

```csharp
public Byte[] Decode(ReadOnlySpan<char> text)
```

#### Parameters

`text` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Encoded text.

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
