﻿// <copyright file="CodingAlphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Diagnostics;

namespace SimpleBase;

/// <summary>
/// A single encoding algorithm can support many different alphabets.
/// EncodingAlphabet consists of a basis for implementing different
/// alphabets for different encodings. It's suitable if you want to
/// implement your own encoding based on the existing base classes.
/// </summary>
/// <remarks>
/// This only supports alphabets with lower ASCII character set (0-127).
/// </remarks>
public abstract class CodingAlphabet : ICodingAlphabet
{
    /// <summary>
    /// Specifies the highest possible char value in an encoding alphabet.
    /// Any char above this would raise an exception.
    /// </summary>
    const int maxLength = 127;

    /// <summary>
    /// Holds a mapping from character to an actual byte value
    /// The values are held as "value + 1" so a zero would denote "not set"
    /// and would cause an exception.
    /// </summary>
    /// <remarks>byte[] has no discernible perf impact and saves memory.</remarks>
    readonly byte[] reverseLookupTable = new byte[maxLength];

    /// <summary>
    /// Initializes a new instance of the <see cref="CodingAlphabet"/> class.
    /// </summary>
    /// <param name="length">Length of the alphabet.</param>
    /// <param name="alphabet">Alphabet character.</param>
    /// <param name="caseInsensitive">Use case-insensitive matching when decoding.</param>
    public CodingAlphabet(int length, string alphabet, bool caseInsensitive = false)
    {
        if (alphabet.Length != length)
        {
            throw new ArgumentException($"Required alphabet length is {length} but provided alphabet is "
                + $"{alphabet.Length} characters long");
        }

        Length = length;
        Value = alphabet;
        for (short i = 0; i < length; i++)
        {
            char c = alphabet[i];
            if (caseInsensitive && char.IsLetter(c))
            {
                char c2 = char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c);
                if (alphabet.Contains(c2))
                {
                    throw new ArgumentException($"Case-sensitivity can't be selected with an alphabet that contains both cases of the same letter", nameof(caseInsensitive));
                }
                Map(c2, i);
            }
            Map(c, i);
        }
    }

    /// <summary>
    /// Gets the length of the alphabet.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets the characters of the alphabet.
    /// </summary>
    public string Value { get; }

    internal ReadOnlySpan<byte> ReverseLookupTable => reverseLookupTable;

    /// <summary>
    /// Generates a standard invalid character exception for alphabets.
    /// </summary>
    /// <remarks>
    /// The reason this is not a throwing method itself is
    /// that the compiler has no way of knowing whether the execution
    /// will end after the method call and can incorrectly assume
    /// reachable code.
    /// </remarks>
    /// <param name="c">The character.</param>
    /// <returns>Exception to be thrown.</returns>
    public static Exception InvalidCharacter(char c)
    {
        return new ArgumentException($"Invalid character: {c}");
    }

    /// <summary>
    /// Get the string representation of the alphabet.
    /// </summary>
    /// <returns>The characters of the encoding alphabet.</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Value.GetHashCode(StringComparison.Ordinal);
    }

    /// <summary>
    /// Map a character to a value.
    /// </summary>
    /// <param name="c">The character.</param>
    /// <param name="value">Corresponding value.</param>
    protected void Map(char c, int value)
    {
        Debug.Assert(c < maxLength, $"Alphabet contains character above {maxLength}");
        reverseLookupTable[c] = (byte)(value + 1);
    }
}
