# INonAllocatingBaseCoder

Namespace: SimpleBase

Efficient encoding functionality using pre-allocated memory buffers by the callers.

```csharp
public interface INonAllocatingBaseCoder
```

## Methods

### **TryEncode(ReadOnlySpan&lt;Byte&gt;, Span&lt;Char&gt;, Int32&)**

Encode a buffer into a base-encoded representation using pre-allocated buffers.

```csharp
bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, Int32& numCharsWritten)
```

#### Parameters

`input` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Bytes to encode.

`output` [Span&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
Output buffer.

`numCharsWritten` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
Actual number of characters written to the output.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether encoding was successful or not. If false, `numCharsWritten`
 will be zero and the content of `output` will be undefined.

### **TryDecode(ReadOnlySpan&lt;Char&gt;, Span&lt;Byte&gt;, Int32&)**

Decode an encoded character buffer into a pre-allocated output buffer.

```csharp
bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, Int32& bytesWritten)
```

#### Parameters

`input` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Encoded text.

`output` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
Output buffer.

`bytesWritten` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
Actual number of bytes written to the output.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether decoding was successful. If false, the value of `bytesWritten`
 will be zero and the content of `output` will be undefined.

### **GetSafeByteCountForDecoding(ReadOnlySpan&lt;Char&gt;)**

Gets a safe estimation about how many bytes decoding will take without performing
 the actual decoding operation. The estimation can be slightly larger than the actual
 output size.

```csharp
int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
```

#### Parameters

`text` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Text to be decoded.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of estimated bytes, or zero if the input length is invalid.

### **GetSafeCharCountForEncoding(ReadOnlySpan&lt;Byte&gt;)**

Gets a safe estimation about how many characters encoding a buffer will take without
 performing the actual encoding operation. The estimation can be slightly larger than the
 actual output size.

```csharp
int GetSafeCharCountForEncoding(ReadOnlySpan<byte> buffer)
```

#### Parameters

`buffer` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Bytes to be encoded.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of estimated characters, or zero if the input length is invalid.
