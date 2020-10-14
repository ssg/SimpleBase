// <copyright file="INumericBaseCoder.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase
{
    /// <summary>
    /// Number-based coding functions.
    /// </summary>
    public interface INumericBaseCoder
    {
        /// <summary>
        /// Encode the given number to Base32.
        /// </summary>
        /// <param name="number">Number to encode.</param>
        /// <returns>Encoded string.</returns>
        /// <remarks>Negative numbers are not supported.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">If number is negative.</exception>
        string Encode(long number);

        /// <summary>
        /// Encode the given number to Base32.
        /// </summary>
        /// <param name="number">Number to encode.</param>
        /// <returns>Encoded string.</returns>
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        string Encode(ulong number);
#pragma warning restore CS3001 // #pragma warning restore CS3002 // We provide a CLS-compliant alternative

        /// <summary>
        /// Decode a Base32 text to a number.
        /// </summary>
        /// <param name="text">Text to decode.</param>
        /// <returns>Decoded number.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the decoded number is larger to fit in a variable or is negative.
        /// </exception>
        long DecodeInt64(string text);

        /// <summary>
        /// Decode a Base32 text to a number.
        /// </summary>
        /// <param name="text">Text to decode.</param>
        /// <returns>Decoded number.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the decoded number is larger to fit in a variable.
        /// </exception>
#pragma warning disable CS3002 // Return type is not CLS-compliant
        ulong DecodeUInt64(string text);
#pragma warning restore CS3002 // We provide a CLS-compliant alternative
    }
}
