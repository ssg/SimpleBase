// <copyright file="Base16Alphabet.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase
{
    /// <summary>
    /// Alphabet representation for Base16 encodings.
    /// </summary>
    public class Base16Alphabet : EncodingAlphabet
    {
        private static Lazy<Base16Alphabet> upperCaseAlphabet = new Lazy<Base16Alphabet>(
            () => new Base16Alphabet("0123456789ABCDEF"));

        private static Lazy<Base16Alphabet> lowerCaseAlphabet = new Lazy<Base16Alphabet>(
            () => new Base16Alphabet("0123456789abcdef"));

        private static Lazy<Base16Alphabet> modHexAlphabet = new Lazy<Base16Alphabet>(
            () => new Base16Alphabet("cbdefghijklnrtuv"));

        /// <summary>
        /// Initializes a new instance of the <see cref="Base16Alphabet"/> class.
        /// </summary>
        /// <param name="alphabet">Encoding alphabet.</param>
        public Base16Alphabet(string alphabet)
            : base(16, alphabet)
        {
        }

        /// <summary>
        /// Gets upper case Base16 alphabet.
        /// </summary>
        public static Base16Alphabet UpperCase { get; } = upperCaseAlphabet.Value;

        /// <summary>
        /// Gets lower case Base16 alphabet.
        /// </summary>
        public static Base16Alphabet LowerCase { get; } = lowerCaseAlphabet.Value;

        /// <summary>
        /// Gets ModHex Base16 alphabet, used by Yubico apps.
        /// </summary>
        public static Base16Alphabet ModHex { get; } = modHexAlphabet.Value;

        /// <summary>
        /// Gets a value indicating whether the decoding should be performed in a case insensitive fashion.
        /// This is the default behavior.
        /// </summary>
        public bool CaseInsensitive { get; } = true;

        /// <inheritdoc/>
        public override int GetAllocationByteCountForDecoding(ReadOnlySpan<char> encodedText)
        {
            return encodedText.Length / 2;
        }

        /// <inheritdoc/>
        public override int GetAllocationCharCountForEncoding(ReadOnlySpan<byte> buffer)
        {
            return buffer.Length * 2;
        }
    }
}
