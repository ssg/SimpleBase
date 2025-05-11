// <copyright file="Base36.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Base36 Encoding/Decoding implementation.
/// </summary>
/// <remarks>
/// Base36 doesn't implement a Stream-based interface because it's not feasible to use
/// on large buffers.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="Base45"/> class using a custom alphabet.
/// </remarks>
/// <param name="alphabet">Alphabet to use.</param>
public sealed class Base36(Base36Alphabet alphabet) : DividingCoder<Base36Alphabet>(alphabet)
{
    static readonly Lazy<Base36> upperCase = new(() => new Base36(Base36Alphabet.Upper));
    static readonly Lazy<Base36> lowerCase= new(() => new Base36(Base36Alphabet.Lower));

    /// <summary>
    /// Gets the uppercase Base36 encoder.
    /// </summary>
    public static Base36 UpperCase => upperCase.Value;

    /// <summary>
    /// Gets the lowercase Base36 encoder.
    /// </summary>
    public static Base36 LowerCase => lowerCase.Value;
}
