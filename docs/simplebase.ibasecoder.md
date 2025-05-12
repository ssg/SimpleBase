# IBaseCoder

Namespace: SimpleBase

Basic encoding functionality.

```csharp
public interface IBaseCoder
```

## Methods

### **Encode(ReadOnlySpan&lt;Byte&gt;)**

Encode a buffer to base-encoded representation.

```csharp
string Encode(ReadOnlySpan<byte> bytes)
```

#### Parameters

`bytes` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Bytes to encode.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Base16 string.

### **Decode(ReadOnlySpan&lt;Char&gt;)**

Decode base-encoded text into bytes.

```csharp
Byte[] Decode(ReadOnlySpan<char> text)
```

#### Parameters

`text` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Base16 text.

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Decoded bytes.
