// <copyright file="Base16.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SimpleBase;

/// <summary>
/// Base16 encoding/decoding.
/// </summary>
public sealed class Base16 : IBaseCoder, IBaseStreamCoder, INonAllocatingBaseCoder
{
    private static readonly Lazy<Base16> upperCase = new(() => new Base16(Base16Alphabet.UpperCase));
    private static readonly Lazy<Base16> lowerCase = new(() => new Base16(Base16Alphabet.LowerCase));
    private static readonly Lazy<Base16> modHex = new(() => new Base16(Base16Alphabet.ModHex));

    /// <summary>
    /// Initializes a new instance of the <see cref="Base16"/> class.
    /// </summary>
    /// <param name="alphabet">Alphabet to use.</param>
    public Base16(Base16Alphabet alphabet)
    {
        Alphabet = alphabet;
    }

    /// <summary>
    /// Gets upper case Base16 encoder. Decoding is case-insensitive.
    /// </summary>
    public static Base16 UpperCase => upperCase.Value;

    /// <summary>
    /// Gets lower case Base16 encoder. Decoding is case-insensitive.
    /// </summary>
    public static Base16 LowerCase => lowerCase.Value;

    /// <summary>
    /// Gets lower case Base16 encoder. Decoding is case-insensitive.
    /// </summary>
    public static Base16 ModHex => modHex.Value;

    /// <summary>
    /// Gets the alphabet used by the encoder.
    /// </summary>
    public Base16Alphabet Alphabet { get; }

    /// <summary>
    /// Decode Upper/Lowercase Base16 text into bytes.
    /// </summary>
    /// <param name="text">Hex string.</param>
    /// <returns>Decoded bytes.</returns>
    public static Span<byte> Decode(string text)
    {
        return UpperCase.Decode(text.AsSpan());
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Alphabet.GetHashCode();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{nameof(Base16)}_{Alphabet}";
    }

    /// <inheritdoc/>
    public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        int textLen = text.Length;
        if ((textLen & 1) != 0)
        {
            return 0;
        }

        return textLen / 2;
    }

    /// <inheritdoc/>
    public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> buffer)
    {
        return buffer.Length * 2;
    }

    /// <summary>
    /// Decode Base16 text through streams for generic use. Stream based variant tries to consume
    /// as little memory as possible, and relies of .NET's own underlying buffering mechanisms,
    /// contrary to their buffer-based versions.
    /// </summary>
    /// <param name="input">Stream that the encoded bytes would be read from.</param>
    /// <param name="output">Stream where decoded bytes will be written to.</param>
    /// <returns>Task that represents the async operation.</returns>
    public async Task DecodeAsync(TextReader input, Stream output)
    {
        await StreamHelper.DecodeAsync(input, output, buffer => Decode(buffer.Span))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Decode Base16 text through streams for generic use. Stream based variant tries to consume
    /// as little memory as possible, and relies of .NET's own underlying buffering mechanisms,
    /// contrary to their buffer-based versions.
    /// </summary>
    /// <param name="input">Stream that the encoded bytes would be read from.</param>
    /// <param name="output">Stream where decoded bytes will be written to.</param>
    public void Decode(TextReader input, Stream output)
    {
        StreamHelper.Decode(input, output, buffer => Decode(buffer.Span));
    }

    /// <summary>
    /// Encodes stream of bytes into a Base16 text.
    /// </summary>
    /// <param name="input">Stream that provides bytes to be encoded.</param>
    /// <param name="output">Stream that the encoded text is written to.</param>
    public void Encode(Stream input, TextWriter output)
    {
        StreamHelper.Encode(input, output, (buffer, lastBlock) => Encode(buffer.Span));
    }

    /// <summary>
    /// Encodes stream of bytes into a Base16 text.
    /// </summary>
    /// <param name="input">Stream that provides bytes to be encoded.</param>
    /// <param name="output">Stream that the encoded text is written to.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task EncodeAsync(Stream input, TextWriter output)
    {
        await StreamHelper.EncodeAsync(input, output, (buffer, lastBlock) =>
            Encode(buffer.Span)).ConfigureAwait(false);
    }

    /// <summary>
    /// Decode Base16 text into bytes.
    /// </summary>
    /// <param name="text">Base16 text.</param>
    /// <returns>Decoded bytes.</returns>
    public byte[] Decode(ReadOnlySpan<char> text)
    {
        int textLen = text.Length;
        if (textLen == 0)
        {
            return Array.Empty<byte>();
        }

        byte[] output = new byte[GetSafeByteCountForDecoding(text)];
        return TryDecode(text, output, out _)
            ? output
            : throw new ArgumentException("Invalid text", nameof(text));
    }

    /// <inheritdoc/>
    public bool TryDecode(ReadOnlySpan<char> text, Span<byte> output, out int numBytesWritten)
    {
        int textLen = text.Length;
        if (textLen == 0)
        {
            numBytesWritten = 0;
            return true;
        }

        if ((textLen & 1) != 0)
        {
            numBytesWritten = 0;
            Debug.WriteLine("Invalid input buffer length for Base16 decoding");
            return false;
        }

        int outputLen = textLen / 2;
        if (output.Length < outputLen)
        {
            numBytesWritten = 0;
            Debug.WriteLine("Insufficient output buffer length for Base16 decoding");
            return false;
        }

        var table = Alphabet.ReverseLookupTable;

        int o = 0;
        unchecked
        {
            for (int n = 0; n < textLen - 1; n += 2)
            {
                char c1 = text[n];
                char c2 = text[n + 1];
                int b1 = table[c1] - 1;
                int b2 = table[c2] - 1;
                if (b1 < 0 || b2 < 0)
                {
                    Debug.WriteLine($"Invalid hex character: {(b1 < 0 ? c1 : c2)}");
                    numBytesWritten = o;
                    return false;
                }

                output[o++] = (byte)((b1 * 16) | b2);
            }
        }

        numBytesWritten = o;
        return true;
    }

    /// <summary>
    /// Encode to Base16 representation.
    /// </summary>
    /// <param name="bytes">Bytes to encode.</param>
    /// <returns>Base16 string.</returns>
    public string Encode(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
        {
            return string.Empty;
        }

        Span<char> output = new char[GetSafeCharCountForEncoding(bytes)];
        internalEncode(bytes, output, Alphabet.Value);
        return new string(output);
    }

    /// <inheritdoc/>
    public bool TryEncode(ReadOnlySpan<byte> bytes, Span<char> output, out int numCharsWritten)
    {
        ReadOnlySpan<char> alphabet = Alphabet.Value;
        int outputLen = bytes.Length * 2;
        if (output.Length < outputLen)
        {
            numCharsWritten = 0;
            return false;
        }

        if (outputLen == 0)
        {
            numCharsWritten = 0;
            return true;
        }

        internalEncode(bytes, output, alphabet);
        numCharsWritten = outputLen;
        return true;
    }

    private static void internalEncode(
        ReadOnlySpan<byte> input,
        Span<char> output,
        ReadOnlySpan<char> alphabet)
    {
        for (int i = 0, o = 0; i < input.Length; i++, o += 2)
        {
            byte b = input[i];
            output[o] = alphabet[b >> 4];
            output[o + 1] = alphabet[b & 0x0F];
        }
    }
}
