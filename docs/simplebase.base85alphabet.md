# Base85Alphabet

Namespace: SimpleBase

Base85 Alphabet.

```csharp
public sealed class Base85Alphabet : CodingAlphabet, ICodingAlphabet
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CodingAlphabet](./simplebase.codingalphabet.md) → [Base85Alphabet](./simplebase.base85alphabet.md)<br>
Implements [ICodingAlphabet](./simplebase.icodingalphabet.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

**Remarks:**

Initializes a new instance of the [Base85Alphabet](./simplebase.base85alphabet.md) class
 using custom settings.

## Properties

### **Z85**

Gets ZeroMQ Z85 Alphabet.

```csharp
public static Base85Alphabet Z85 { get; }
```

#### Property Value

[Base85Alphabet](./simplebase.base85alphabet.md)<br>

### **Ascii85**

Gets Adobe Ascii85 Alphabet (each character is directly produced by raw value + 33),
 also known as "btoa" encoding.

```csharp
public static Base85Alphabet Ascii85 { get; }
```

#### Property Value

[Base85Alphabet](./simplebase.base85alphabet.md)<br>

### **Rfc1924**

Gets Base85 encoding defined in RFC 1924.

```csharp
public static Base85Alphabet Rfc1924 { get; }
```

#### Property Value

[Base85Alphabet](./simplebase.base85alphabet.md)<br>

### **AllZeroShortcut**

Gets the character to be used for "all zeros".

```csharp
public Nullable<char> AllZeroShortcut { get; }
```

#### Property Value

[Nullable&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **AllSpaceShortcut**

Gets the character to be used for "all spaces".

```csharp
public Nullable<char> AllSpaceShortcut { get; }
```

#### Property Value

[Nullable&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **HasShortcut**

Gets a value indicating whether the alphabet uses one of shortcut characters for all spaces
 or all zeros.

```csharp
public bool HasShortcut { get; }
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

### **Base85Alphabet(String, Nullable&lt;Char&gt;, Nullable&lt;Char&gt;)**

Base85 Alphabet.

```csharp
public Base85Alphabet(string alphabet, Nullable<char> allZeroShortcut, Nullable<char> allSpaceShortcut)
```

#### Parameters

`alphabet` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Alphabet to use.

`allZeroShortcut` [Nullable&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
Character to substitute for all zero.

`allSpaceShortcut` [Nullable&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
Character to substitute for all space.

**Remarks:**

Initializes a new instance of the [Base85Alphabet](./simplebase.base85alphabet.md) class
 using custom settings.
