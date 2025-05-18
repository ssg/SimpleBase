// <copyright file="Base10.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Base10 encoder.
/// </summary>
/// <param name="alphabet">Alphabet to use.</param>
public class Base10(Base10Alphabet alphabet): DividingCoder<Base10Alphabet>(alphabet)
{
    /// <summary>
    /// Default Base10 implementation.
    /// </summary>
    static readonly Lazy<Base10> @default = new(() => new Base10(Base10Alphabet.Default));

    /// <summary>
    /// Default Base10 implementation.
    /// </summary>
    public static Base10 Default => @default.Value;
}
