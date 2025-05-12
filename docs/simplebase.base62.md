# Base62

Namespace: SimpleBase

Base62 Encoding/Decoding implementation.

```csharp
public sealed class Base62 : DividingCoder`1, IBaseCoder, INonAllocatingBaseCoder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DividingCoder&lt;Base62Alphabet&gt;](./simplebase.dividingcoder-1.md) → [Base62](./simplebase.base62.md)<br>
Implements [IBaseCoder](./simplebase.ibasecoder.md), [INonAllocatingBaseCoder](./simplebase.inonallocatingbasecoder.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

**Remarks:**

Base62 doesn't implement a Stream-based interface because it's not feasible to use
 on large buffers.

## Properties

### **Default**

Gets the default flavor.

```csharp
public static Base62 Default { get; }
```

#### Property Value

[Base62](./simplebase.base62.md)<br>

### **LowerFirst**

Gets the alphabet with the lowercase letters first.

```csharp
public static Base62 LowerFirst { get; }
```

#### Property Value

[Base62](./simplebase.base62.md)<br>

### **Alphabet**

Gets the encoding alphabet.

```csharp
public Base62Alphabet Alphabet { get; }
```

#### Property Value

[Base62Alphabet](./simplebase.base62alphabet.md)<br>

## Constructors

### **Base62(Base62Alphabet)**

Base62 Encoding/Decoding implementation.

```csharp
public Base62(Base62Alphabet alphabet)
```

#### Parameters

`alphabet` [Base62Alphabet](./simplebase.base62alphabet.md)<br>
Alphabet to use.

**Remarks:**

Base62 doesn't implement a Stream-based interface because it's not feasible to use
 on large buffers.
