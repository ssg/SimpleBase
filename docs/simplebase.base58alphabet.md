# Base58Alphabet

Namespace: SimpleBase

Base58 alphabet.

```csharp
public sealed class Base58Alphabet : CodingAlphabet, ICodingAlphabet
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CodingAlphabet](./simplebase.codingalphabet.md) → [Base58Alphabet](./simplebase.base58alphabet.md)<br>
Implements [ICodingAlphabet](./simplebase.icodingalphabet.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

**Remarks:**

Initializes a new instance of the [Base58Alphabet](./simplebase.base58alphabet.md) class
 using a custom alphabet.

## Properties

### **Bitcoin**

Gets Bitcoin alphabet. Monero also uses this alphabet but only works with MoneroBase58 encoding.

```csharp
public static Base58Alphabet Bitcoin { get; }
```

#### Property Value

[Base58Alphabet](./simplebase.base58alphabet.md)<br>

### **Ripple**

Gets Base58 alphabet.

```csharp
public static Base58Alphabet Ripple { get; }
```

#### Property Value

[Base58Alphabet](./simplebase.base58alphabet.md)<br>

### **Flickr**

Gets Flickr alphabet.

```csharp
public static Base58Alphabet Flickr { get; }
```

#### Property Value

[Base58Alphabet](./simplebase.base58alphabet.md)<br>

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

### **Base58Alphabet(String)**

Base58 alphabet.

```csharp
public Base58Alphabet(string alphabet)
```

#### Parameters

`alphabet` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Alphabet to use.

**Remarks:**

Initializes a new instance of the [Base58Alphabet](./simplebase.base58alphabet.md) class
 using a custom alphabet.
