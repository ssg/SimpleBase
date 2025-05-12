# CodingAlphabet

Namespace: SimpleBase

A single encoding algorithm can support many different alphabets.
 EncodingAlphabet consists of a basis for implementing different
 alphabets for different encodings. It's suitable if you want to
 implement your own encoding based on the existing base classes.

```csharp
public abstract class CodingAlphabet : ICodingAlphabet
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CodingAlphabet](./simplebase.codingalphabet.md)<br>
Implements [ICodingAlphabet](./simplebase.icodingalphabet.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Length**

Gets the length of the alphabet.

```csharp
public int Length { get; private set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Value**

Gets the characters of the alphabet.

```csharp
public string Value { get; private set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **CodingAlphabet(Int32, String)**

Initializes a new instance of the [CodingAlphabet](./simplebase.codingalphabet.md) class.

```csharp
public CodingAlphabet(int length, string alphabet)
```

#### Parameters

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Length of the alphabe.

`alphabet` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Alphabet character.

## Methods

### **InvalidCharacter(Char)**

Generates a standard invalid character exception for alphabets.

```csharp
public static Exception InvalidCharacter(char c)
```

#### Parameters

`c` [Char](https://docs.microsoft.com/en-us/dotnet/api/system.char)<br>
Characters.

#### Returns

[Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>
Exception to be thrown.

**Remarks:**

The reason this is not a throwing method itself is
 that the compiler has no way of knowing whether the execution
 will end after the method call and can incorrectly assume
 reachable code.

### **ToString()**

Get the string representation of the alphabet.

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The characters of the encoding alphabet.

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Map(Char, Int32)**

Map a character to a value.

```csharp
protected void Map(char c, int value)
```

#### Parameters

`c` [Char](https://docs.microsoft.com/en-us/dotnet/api/system.char)<br>
Characters.

`value` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Corresponding value.
