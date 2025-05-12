# Base62Alphabet

Namespace: SimpleBase

Base62 alphabet.

```csharp
public sealed class Base62Alphabet : CodingAlphabet, ICodingAlphabet
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CodingAlphabet](./simplebase.codingalphabet.md) → [Base62Alphabet](./simplebase.base62alphabet.md)<br>
Implements [ICodingAlphabet](./simplebase.icodingalphabet.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

**Remarks:**

Initializes a new instance of the [Base58Alphabet](./simplebase.base58alphabet.md) class
 using a custom alphabet.

## Properties

### **Default**

Gets the standard, most common alphabet.

```csharp
public static Base62Alphabet Default { get; }
```

#### Property Value

[Base62Alphabet](./simplebase.base62alphabet.md)<br>

### **Alternative**

Gets Alternative alphabet.

```csharp
public static Base62Alphabet Alternative { get; }
```

#### Property Value

[Base62Alphabet](./simplebase.base62alphabet.md)<br>

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

### **Base62Alphabet(String)**

Base62 alphabet.

```csharp
public Base62Alphabet(string alphabet)
```

#### Parameters

`alphabet` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Alphabet to use.

**Remarks:**

Initializes a new instance of the [Base58Alphabet](./simplebase.base58alphabet.md) class
 using a custom alphabet.
