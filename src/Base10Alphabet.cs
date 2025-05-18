using System;

namespace SimpleBase;

/// <summary>
/// Base 10 encoding alphabet.
/// </summary>
/// <param name="alphabet">Characters to use for encoding.</param>
public class Base10Alphabet(string alphabet) : CodingAlphabet(10, alphabet)
{
    /// <summary>
    /// Default Base10 alphabet.
    /// </summary>
    static readonly Lazy<Base10Alphabet> @default = new(() => new Base10Alphabet("0123456789"));

    /// <summary>
    /// Standard Base10 alphabet ("0123456789").
    /// </summary>
    public static Base10Alphabet Default => @default.Value;
}