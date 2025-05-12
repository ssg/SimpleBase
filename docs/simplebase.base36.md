# Base36

Namespace: SimpleBase

Base36 Encoding/Decoding implementation.

```csharp
public sealed class Base36 : DividingCoder`1, IBaseCoder, INonAllocatingBaseCoder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DividingCoder&lt;Base36Alphabet&gt;](./simplebase.dividingcoder-1.md) → [Base36](./simplebase.base36.md)<br>
Implements [IBaseCoder](./simplebase.ibasecoder.md), [INonAllocatingBaseCoder](./simplebase.inonallocatingbasecoder.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

**Remarks:**

Base36 doesn't implement a Stream-based interface because it's not feasible to use
 on large buffers.

## Properties

### **UpperCase**

Gets the uppercase Base36 encoder.

```csharp
public static Base36 UpperCase { get; }
```

#### Property Value

[Base36](./simplebase.base36.md)<br>

### **LowerCase**

Gets the lowercase Base36 encoder.

```csharp
public static Base36 LowerCase { get; }
```

#### Property Value

[Base36](./simplebase.base36.md)<br>

### **Alphabet**

Gets the encoding alphabet.

```csharp
public Base36Alphabet Alphabet { get; }
```

#### Property Value

[Base36Alphabet](./simplebase.base36alphabet.md)<br>

## Constructors

### **Base36(Base36Alphabet)**

Base36 Encoding/Decoding implementation.

```csharp
public Base36(Base36Alphabet alphabet)
```

#### Parameters

`alphabet` [Base36Alphabet](./simplebase.base36alphabet.md)<br>
Alphabet to use.

**Remarks:**

Base36 doesn't implement a Stream-based interface because it's not feasible to use
 on large buffers.
