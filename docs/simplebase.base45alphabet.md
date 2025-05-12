# Base45Alphabet

Namespace: SimpleBase

Base45 coding alphabet.

```csharp
public class Base45Alphabet : CodingAlphabet, ICodingAlphabet
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CodingAlphabet](./simplebase.codingalphabet.md) → [Base45Alphabet](./simplebase.base45alphabet.md)<br>
Implements [ICodingAlphabet](./simplebase.icodingalphabet.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Default**

Default Base45 alphabet per RFC 9285.

```csharp
public static Base45Alphabet Default { get; }
```

#### Property Value

[Base45Alphabet](./simplebase.base45alphabet.md)<br>

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

### **Base45Alphabet(String)**

Base45 coding alphabet.

```csharp
public Base45Alphabet(string alphabet)
```

#### Parameters

`alphabet` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The characters that build up the Base45 alphabet.
