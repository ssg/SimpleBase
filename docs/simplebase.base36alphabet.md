# Base36Alphabet

Namespace: SimpleBase

Base36 encoding/decoding alphabet.

```csharp
public class Base36Alphabet : CodingAlphabet, ICodingAlphabet
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CodingAlphabet](./simplebase.codingalphabet.md) → [Base36Alphabet](./simplebase.base36alphabet.md)<br>
Implements [ICodingAlphabet](./simplebase.icodingalphabet.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Upper**

Base36 alphabet with numbers and uppercase letters.

```csharp
public static Base36Alphabet Upper { get; }
```

#### Property Value

[Base36Alphabet](./simplebase.base36alphabet.md)<br>

### **Lower**

Base36 alphabet with numbers and lowercase letters.

```csharp
public static Base36Alphabet Lower { get; }
```

#### Property Value

[Base36Alphabet](./simplebase.base36alphabet.md)<br>

### **Length**

Gets the length of the alphabet.

```csharp
public int Length { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Value**

Gets the characters of the alphabet.

```csharp
public string Value { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **Base36Alphabet(String)**

Base36 encoding/decoding alphabet.

```csharp
public Base36Alphabet(string alphabet)
```

#### Parameters

`alphabet` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Alphabet to use.
