# IBaseStreamCoder

Namespace: SimpleBase

Stream-based encoding functionality.

```csharp
public interface IBaseStreamCoder
```

Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Methods

### **Encode(Stream, TextWriter)**

Encodes stream of bytes into base-encoded text.

```csharp
void Encode(Stream input, TextWriter output)
```

#### Parameters

`input` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Stream that provides bytes to be encoded.

`output` [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br>
Stream that the encoded text is written to.

### **EncodeAsync(Stream, TextWriter)**

Encodes stream of bytes into base-encoded text.

```csharp
Task EncodeAsync(Stream input, TextWriter output)
```

#### Parameters

`input` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Stream that provides bytes to be encoded.

`output` [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br>
Stream that the encoded text is written to.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
A [Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) representing the asynchronous operation.

### **Decode(TextReader, Stream)**

Decode base-encoded text through streams. Stream based variant tries to consume
 as little memory as possible, and relies of .NET's own underlying buffering mechanisms,
 contrary to their buffer-based versions.

```csharp
void Decode(TextReader input, Stream output)
```

#### Parameters

`input` [TextReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.textreader)<br>
Stream that the encoded bytes would be read from.

`output` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Stream where decoded bytes will be written to.

### **DecodeAsync(TextReader, Stream)**

Decode base-encoded text through streams. Stream based variant tries to consume
 as little memory as possible, and relies of .NET's own underlying buffering mechanisms,
 contrary to their buffer-based versions.

```csharp
Task DecodeAsync(TextReader input, Stream output)
```

#### Parameters

`input` [TextReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.textreader)<br>
Stream that the encoded bytes would be read from.

`output` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
Stream where decoded bytes will be written to.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
A [Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) representing the asynchronous operation.
