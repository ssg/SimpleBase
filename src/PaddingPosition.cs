// <copyright file="PaddingPosition.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

namespace SimpleBase;

/// <summary>
/// Position of the padding in an encoder output.
/// </summary>
public enum PaddingPosition
{
    /// <summary>
    /// Padding appears at the start of the encoded buffer.
    /// </summary>
    Start,

    /// <summary>
    /// Padding appears at the end of the buffer.
    /// </summary>
    End,
}
