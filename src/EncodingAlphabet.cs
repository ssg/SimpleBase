// <copyright file="EncodingAlphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

namespace SimpleBase
{
    using System;
    using System.Diagnostics;

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
        /// Any char above with would raise an exception
        /// </summary>
        private const int lookupLength = 127;

        /// <summary>
        /// Holds a mapping from character to an actual byte value
        /// The values are held as "value + 1" so a zero would denote "not set"
        /// and would cause an exception.
        /// </summary>
        /// byte[] has no discernible perf impact and saves memory
        private readonly byte[] reverseLookupTable = new byte[lookupLength];

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingAlphabet"/> class.
        /// </summary>
        /// <param name="length">Length of the alphabet</param>
        /// <param name="alphabet">Alphabet characters</param>
        public EncodingAlphabet(int length, string alphabet)
        {
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
        /// Gets the length of the alphabet
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Gets the characters of the alphabet
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
        /// <param name="c">Character</param>
        /// <returns>Exception to be thrown</returns>
        public static Exception InvalidCharacter(char c)
        {
            return new ArgumentException($"Invalid character: {c}");
        }

        /// <summary>
        /// Get the string representation of the alphabet
        /// </summary>
        /// <returns>The characters of the encoding alphabet</returns>
        public override string ToString()
        {
            return this.Value;
        }

        /// <summary>
        /// Map a character to a value
        /// </summary>
        /// <param name="c">Character</param>
        /// <param name="value">Corresponding value</param>
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