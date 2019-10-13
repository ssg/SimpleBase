// <copyright file="EncodingAlphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace SimpleBase
{
    /// <summary>
    /// A single encoding algorithm can support many different alphabets.
    /// EncodingAlphabet consists of a basis for implementing different
    /// alphabets for different encodings. It's suitable if you want to
    /// implement your own encoding based on the existing base classes.
    /// </summary>
    public abstract class EncodingAlphabet
    {
        /// <summary>
        /// Specifies the highest possible char value in an encoding alphabet
        /// Any char above with would raise an exception.
        /// </summary>
        private const int lookupLength = 127;

        /// <summary>
        /// Holds a mapping from character to an actual byte value
        /// The values are held as "value + 1" so a zero would denote "not set"
        /// and would cause an exception.
        /// </summary>
        /// <remarks>byte[] has no discernible perf impact and saves memory.</remarks>
        private readonly byte[] reverseLookupTable = new byte[lookupLength];

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingAlphabet"/> class.
        /// </summary>
        /// <param name="length">Length of the alphabe.</param>
        /// <param name="alphabet">Alphabet character.</param>
        public EncodingAlphabet(int length, string alphabet)
        {
            if (alphabet is null)
            {
                throw new ArgumentNullException(nameof(alphabet));
            }

            Debug.WriteLine($"Creating a new encoding alphabet with length = {length} and alphabet = {alphabet}");
            if (alphabet.Length != length)
            {
                throw new ArgumentException($"Required alphabet length is {length} but provided alphabet is "
                    + $"{alphabet.Length} characters long");
            }

            this.Length = length;
            this.Value = alphabet;

            for (short i = 0; i < length; i++)
            {
                this.Map(alphabet[i], i);
            }
        }

        /// <summary>
        /// Gets the length of the alphabet.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Gets the characters of the alphabet.
        /// </summary>
        public string Value { get; private set; }

        internal ReadOnlySpan<byte> ReverseLookupTable => this.reverseLookupTable.AsSpan();

        /// <summary>
        /// Generates a standard invalid character exception for alphabets.
        /// </summary>
        /// <remarks>
        /// The reason this is not a throwing method itself is
        /// that the compiler has no way of knowing whether the execution
        /// will end after the method call and can incorreclty assume
        /// reachable code.
        /// </remarks>
        /// <param name="c">Characters.</param>
        /// <returns>Exception to be thrown.</returns>
        public static Exception InvalidCharacter(char c)
        {
            return new ArgumentException($"Invalid character: {c}");
        }

        /// <summary>
        /// Gets safe number of bytes to be allocated to fit the decoded buffer.
        /// </summary>
        /// <param name="text">The text to be decoded.</param>
        /// <returns>A buffer size that would fit the decoded version of the input buffer.</returns>
        public abstract int GetAllocationByteCountForDecoding(ReadOnlySpan<char> text);

        /// <summary>
        /// Gets safe number of bytes to be allocated to fit the decoded buffer.
        /// </summary>
        /// <param name="text">The text to be decoded.</param>
        /// <returns>A buffer size that would fit the decoded version of the input buffer.</returns>
        public int GetAllocationByteCountForDecoding(string text)
        {
            return GetAllocationByteCountForDecoding(text.AsSpan());
        }

        /// <summary>
        /// Gets safe number of characters to be allocated to fit the encoded text.
        /// </summary>
        /// <param name="bytes">Buffer to be encoded.</param>
        /// <returns>Number of bytes equal or larger than the encoded text.</returns>
        public abstract int GetAllocationCharCountForEncoding(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Gets safe number of characters to be allocated to fit the encoded text.
        /// </summary>
        /// <param name="bytes">Buffer to be encoded.</param>
        /// <returns>Number of bytes equal or larger than the encoded text.</returns>
        public int GetAllocationCharCountForEncoding(byte[] bytes)
        {
            return GetAllocationCharCountForEncoding(bytes.AsSpan());
        }

        /// <summary>
        /// Get the string representation of the alphabet.
        /// </summary>
        /// <returns>The characters of the encoding alphabet.</returns>
        public override string ToString()
        {
            return this.Value;
        }

        /// <summary>
        /// Map a character to a value.
        /// </summary>
        /// <param name="c">Characters.</param>
        /// <param name="value">Corresponding value.</param>
        protected void Map(char c, int value)
        {
            if (c >= lookupLength)
            {
                throw new InvalidOperationException($"Alphabet contains character above {lookupLength}");
            }

            this.reverseLookupTable[c] = (byte)(value + 1);
        }
    }
}