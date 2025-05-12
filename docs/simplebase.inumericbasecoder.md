# INumericBaseCoder

Namespace: SimpleBase

Number-based coding functions.

```csharp
public interface INumericBaseCoder
```

Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Methods

### **Encode(Int64)**

Encode the given number.

```csharp
string Encode(long number)
```

#### Parameters

`number` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>
Number to encode.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoded string.

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>
If number is negative.

**Remarks:**

Negative numbers are not supported.

### **Encode(UInt64)**

Encode the given number.

```csharp
string Encode(ulong number)
```

#### Parameters

`number` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Number to encode.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoded string.

### **DecodeInt64(String)**

Decode text to a number.

```csharp
long DecodeInt64(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Text to decode.

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>
Decoded number.

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
If the decoded number is larger to fit in a variable or is negative.

### **DecodeUInt64(String)**

Decode text to a number.

```csharp
ulong DecodeUInt64(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Text to decode.

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Decoded number.

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
If the decoded number is larger to fit in a variable.

### **TryDecodeUInt64(String, UInt64&)**

Try to decode text into a number without throwing an exception.

```csharp
bool TryDecodeUInt64(string text, UInt64& number)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Input text.

`number` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>
Output number.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if successful, false otherwise.
