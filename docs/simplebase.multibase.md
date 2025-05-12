# Multibase

Namespace: SimpleBase

Multibase encoding and decoding.

```csharp
public static class Multibase
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Multibase](./simplebase.multibase.md)

## Methods

### **Decode(ReadOnlySpan&lt;Char&gt;)**

Decodes a multibase encoded string.

```csharp
public static Byte[] Decode(ReadOnlySpan<char> text)
```

#### Parameters

`text` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Input text.

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Decoded bytes.

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
If the text is empty or has unsupported encoding.

### **TryDecode(ReadOnlySpan&lt;Char&gt;, Span&lt;Byte&gt;, Int32&)**

Tries to decode a multibase encoded string into a span of bytes.

```csharp
public static bool TryDecode(ReadOnlySpan<char> text, Span<byte> bytes, Int32& bytesWritten)
```

#### Parameters

`text` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Input text.

`bytes` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
Output span.

`bytesWritten` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
Number of bytes written to the output span.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if successful, false otherwise.

### **Encode(ReadOnlySpan&lt;Byte&gt;, MultibaseEncoding)**

Encodes a byte array into a multibase encoded string with given encoding.

```csharp
public static string Encode(ReadOnlySpan<byte> bytes, MultibaseEncoding encoding)
```

#### Parameters

`bytes` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`encoding` [MultibaseEncoding](./simplebase.multibaseencoding.md)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Exceptions

[NotImplementedException](https://docs.microsoft.com/en-us/dotnet/api/system.notimplementedexception)<br>
