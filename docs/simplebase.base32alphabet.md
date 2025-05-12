# Base32Alphabet

Namespace: SimpleBase

Base32 alphabet flavors.

```csharp
public class Base32Alphabet : CodingAlphabet, ICodingAlphabet
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CodingAlphabet](./simplebase.codingalphabet.md) → [Base32Alphabet](./simplebase.base32alphabet.md)<br>
Implements [ICodingAlphabet](./simplebase.icodingalphabet.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Crockford**

Gets Crockford alphabet.

```csharp
public static Base32Alphabet Crockford { get; }
```

#### Property Value

[Base32Alphabet](./simplebase.base32alphabet.md)<br>

### **Rfc4648**

Gets RFC4648 alphabet.

```csharp
public static Base32Alphabet Rfc4648 { get; }
```

#### Property Value

[Base32Alphabet](./simplebase.base32alphabet.md)<br>

### **ExtendedHex**

Gets Extended Hex alphabet.

```csharp
public static Base32Alphabet ExtendedHex { get; }
```

#### Property Value

[Base32Alphabet](./simplebase.base32alphabet.md)<br>

### **ExtendedHexLower**

Gets Extended Hex alphabet.

```csharp
public static Base32Alphabet ExtendedHexLower { get; }
```

#### Property Value

[Base32Alphabet](./simplebase.base32alphabet.md)<br>

### **ZBase32**

Gets z-base-32 alphabet.

```csharp
public static Base32Alphabet ZBase32 { get; }
```

#### Property Value

[Base32Alphabet](./simplebase.base32alphabet.md)<br>

### **Geohash**

Gets Geohash alphabet.

```csharp
public static Base32Alphabet Geohash { get; }
```

#### Property Value

[Base32Alphabet](./simplebase.base32alphabet.md)<br>

### **FileCoin**

Gets FileCoin alphabet.

```csharp
public static Base32Alphabet FileCoin { get; }
```

#### Property Value

[Base32Alphabet](./simplebase.base32alphabet.md)<br>

### **Base32H**

Gets Base32H alphabet.

```csharp
public static Base32Alphabet Base32H { get; }
```

#### Property Value

[Base32Alphabet](./simplebase.base32alphabet.md)<br>

### **Bech32**

Gets Bech32 alphabet.

```csharp
public static Base32Alphabet Bech32 { get; }
```

#### Property Value

[Base32Alphabet](./simplebase.base32alphabet.md)<br>

### **PaddingChar**

Gets the padding character used in encoding.

```csharp
public char PaddingChar { get; }
```

#### Property Value

[Char](https://docs.microsoft.com/en-us/dotnet/api/system.char)<br>

### **PaddingPosition**

Gets the position of the padding characters in the encoder output.

```csharp
public PaddingPosition PaddingPosition { get; }
```

#### Property Value

[PaddingPosition](./simplebase.paddingposition.md)<br>

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

### **Base32Alphabet(String)**

Initializes a new instance of the [Base32Alphabet](./simplebase.base32alphabet.md) class.

```csharp
public Base32Alphabet(string alphabet)
```

#### Parameters

`alphabet` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Characters.

### **Base32Alphabet(String, Char, PaddingPosition)**

Initializes a new instance of the [Base32Alphabet](./simplebase.base32alphabet.md) class.

```csharp
public Base32Alphabet(string alphabet, char paddingChar, PaddingPosition paddingPosition)
```

#### Parameters

`alphabet` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoding alphabet to use.

`paddingChar` [Char](https://docs.microsoft.com/en-us/dotnet/api/system.char)<br>
Padding character.

`paddingPosition` [PaddingPosition](./simplebase.paddingposition.md)<br>
Position of the padding characters in the encoder output.
