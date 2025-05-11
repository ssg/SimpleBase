// <copyright file="Base62.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Base62 Encoding/Decoding implementation.
/// </summary>
/// <remarks>
/// Base62 doesn't implement a Stream-based interface because it's not feasible to use
/// on large buffers.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="Base45"/> class using a custom alphabet.
/// </remarks>
/// <param name="alphabet">Alphabet to use.</param>
public sealed class Base62(Base62Alphabet alphabet) : DividingCoder<Base62Alphabet>(alphabet)
{
    static readonly Lazy<Base62> defaultAlphabet = new(() => new Base62(Base62Alphabet.Default));
    static readonly Lazy<Base62> lowerFirst= new(() => new Base62(Base62Alphabet.Alternative));

    /// <summary>
    /// Gets the default flavor.
    /// </summary>
    public static Base62 Default => defaultAlphabet.Value;

    /// <summary>
    /// Gets the alphabet with the lowercase letters first.
    /// </summary>
    public static Base62 LowerFirst => lowerFirst.Value;
}
