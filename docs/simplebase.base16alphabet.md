# Base16Alphabet

Namespace: SimpleBase

Alphabet representation for Base16 encodings.

```csharp
public class Base16Alphabet : CodingAlphabet, ICodingAlphabet
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CodingAlphabet](./simplebase.codingalphabet.md) → [Base16Alphabet](./simplebase.base16alphabet.md)<br>
Implements [ICodingAlphabet](./simplebase.icodingalphabet.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **UpperCase**

Gets upper case Base16 alphabet.

```csharp
public static Base16Alphabet UpperCase { get; }
```

#### Property Value

[Base16Alphabet](./simplebase.base16alphabet.md)<br>

### **LowerCase**

Gets lower case Base16 alphabet.

```csharp
public static Base16Alphabet LowerCase { get; }
```

#### Property Value

[Base16Alphabet](./simplebase.base16alphabet.md)<br>

### **ModHex**

Gets ModHex Base16 alphabet, used by Yubico apps.

```csharp
public static Base16Alphabet ModHex { get; }
```

#### Property Value

[Base16Alphabet](./simplebase.base16alphabet.md)<br>

### **CaseSensitive**

Gets a value indicating whether the decoding should be performed in a case sensitive fashion.
 The default is false.

```csharp
public bool CaseSensitive { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

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

### **Base16Alphabet(String)**

Initializes a new instance of the [Base16Alphabet](./simplebase.base16alphabet.md) class with
 case insensitive semantics.

```csharp
public Base16Alphabet(string alphabet)
```

#### Parameters

`alphabet` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoding alphabet.

### **Base16Alphabet(String, Boolean)**

Initializes a new instance of the [Base16Alphabet](./simplebase.base16alphabet.md) class.

```csharp
public Base16Alphabet(string alphabet, bool caseSensitive)
```

#### Parameters

`alphabet` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoding alphabet.

`caseSensitive` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If the decoding should be performed case sensitive.
