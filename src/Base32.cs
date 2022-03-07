// <copyright file="Base32.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SimpleBase;

/// <summary>
/// Base32 encoding/decoding functions.
/// </summary>
public sealed class Base32 : IBaseCoder, IBaseStreamCoder, INonAllocatingBaseCoder,
    INumericBaseCoder
{
    private const int bitsPerByte = 8;
    private const int bitsPerChar = 5;

    private static readonly Lazy<Base32> crockford = new(() => new Base32(Base32Alphabet.Crockford));
    private static readonly Lazy<Base32> rfc4648 = new(() => new Base32(Base32Alphabet.Rfc4648));
    private static readonly Lazy<Base32> extendedHex = new(() => new Base32(Base32Alphabet.ExtendedHex));
    private static readonly Lazy<Base32> zBase32 = new(() => new Base32(Base32Alphabet.ZBase32));
    private static readonly Lazy<Base32> geohash = new(() => new Base32(Base32Alphabet.Geohash));
    private static readonly Lazy<Base32> bech32 = new(() => new Base32(Base32Alphabet.Bech32));

    /// <summary>
    /// Initializes a new instance of the <see cref="Base32"/> class with a
    /// custom alphabet.
    /// </summary>
    /// <param name="alphabet">Alphabet to use.</param>
    public Base32(Base32Alphabet alphabet)
    {
        if (alphabet.PaddingPosition != PaddingPosition.End)
        {
            throw new ArgumentException(
                "Only alphabets with paddings at the end are supported by this implementation",
                nameof(alphabet));
        }

        Alphabet = alphabet;
    }

    /// <summary>
    /// Gets Douglas Crockford's Base32 flavor with substitution characters.
    /// </summary>
    public static Base32 Crockford => crockford.Value;

    /// <summary>
    /// Gets RFC 4648 variant of Base32 coder.
    /// </summary>
    public static Base32 Rfc4648 => rfc4648.Value;

    /// <summary>
    /// Gets Extended Hex variant of Base32 coder.
    /// </summary>
    /// <remarks>Also from RFC 4648.</remarks>
    public static Base32 ExtendedHex => extendedHex.Value;

    /// <summary>
    /// Gets z-base-32 variant of Base32 coder.
    /// </summary>
    /// <remarks>This variant is used in Mnet, ZRTP and Tahoe-LAFS.</remarks>
    public static Base32 ZBase32 => zBase32.Value;

    /// <summary>
    /// Gets Geohash variant of Base32 coder.
    /// </summary>
    public static Base32 Geohash => geohash.Value;

    /// <summary>
    /// Gets Bech32 variant of Base32 coder.
    /// </summary>
    public static Base32 Bech32 => bech32.Value;

    /// <summary>
    /// Gets the encoding alphabet.
    /// </summary>
    public Base32Alphabet Alphabet { get; }

    /// <inheritdoc/>
    public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        return getAllocationByteCountForDecoding(text.Length - getPaddingCharCount(text));
    }

    /// <inheritdoc/>
    public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> buffer)
    {
        return (((buffer.Length - 1) / bitsPerChar) + 1) * bitsPerByte;
    }

    /// <inheritdoc/>
    public string Encode(long number)
    {
        return number >= 0
            ? Encode((ulong)number)
            : throw new ArgumentOutOfRangeException(nameof(number), "Number is negative");
    }

    /// <inheritdoc/>
    public string Encode(ulong number)
    {
        var buffer = BitConverter.GetBytes(number);
        bool little = BitConverter.IsLittleEndian;
        int numBytes = sizeof(ulong);
        int index = little ? 0 : (numBytes - 1);
        int increment = little ? -1 : 1;
        while (buffer[index] == 0)
        {
            index += increment;
        }

        var span = buffer.AsSpan();
        return Encode(little ? span[..index] : span[index..]);
    }

    /// <inheritdoc/>
    public ulong DecodeUInt64(string text)
    {
        var buffer = Decode(text);
        return buffer.Length <= sizeof(ulong)
            ? BitConverter.ToUInt64(buffer)
            : throw new InvalidOperationException("Decoded text is too long to fit in a buffer");
    }

    /// <inheritdoc/>
    public long DecodeInt64(string text)
    {
        return (long)DecodeUInt64(text);
    }

    /// <summary>
    /// Encode a memory span into a Base32 string without padding.
    /// </summary>
    /// <param name="bytes">Buffer to be encoded.</param>
    /// <returns>Encoded string.</returns>
    public string Encode(ReadOnlySpan<byte> bytes)
    {
        return Encode(bytes, padding: false);
    }

    /// <summary>
    /// Encode a memory span into a Base32 string.
    /// </summary>
    /// <param name="bytes">Buffer to be encoded.</param>
    /// <param name="padding">Append padding characters in the output.</param>
    /// <returns>Encoded string.</returns>
    public unsafe string Encode(ReadOnlySpan<byte> bytes, bool padding)
    {
        int bytesLen = bytes.Length;
        if (bytesLen == 0)
        {
            return string.Empty;
        }

        // we are ok with slightly larger buffer since the output string will always
        // have the exact length of the output produced.
        int outputLen = GetSafeCharCountForEncoding(bytes);
        string output = new('\0', outputLen);
        fixed (byte* inputPtr = bytes)
        {
            fixed (char* outputPtr = output)
            {
#pragma warning disable IDE0046 // Convert to conditional expression - prefer clarity
                if (!internalEncode(
                    inputPtr,
                    bytesLen,
                    outputPtr,
                    outputLen,
                    padding,
                    out int numCharsWritten))
                {
                    throw new InvalidOperationException("Internal error: couldn't calculate proper output buffer size for input");
                }

                return output[..numCharsWritten];
#pragma warning restore IDE0046 // Convert to conditional expression
            }
        }
    }

    /// <summary>
    /// Decode a Base32 encoded string into bytes.
    /// </summary>
    /// <param name="text">Encoded Base32 string.</param>
    /// <returns>Decoded bytes.</returns>
    public unsafe byte[] Decode(ReadOnlySpan<char> text)
    {
        int paddingLen = getPaddingCharCount(text);
        int textLen = text.Length - paddingLen;
        int outputLen = getAllocationByteCountForDecoding(textLen);
        if (outputLen == 0)
        {
            return Array.Empty<byte>();
        }

        var outputBuffer = new byte[outputLen];
        fixed (byte* outputPtr = outputBuffer)
        {
            fixed (char* inputPtr = text)
            {
                if (!internalDecode(inputPtr, textLen, outputPtr, outputLen, out _))
                {
                    throw new ArgumentException("Invalid input or output", nameof(text));
                }
            }
        }

        return outputBuffer;
    }

    /// <summary>
    /// Encode a binary stream to a Base32 text stream without padding.
    /// </summary>
    /// <param name="input">Input bytes.</param>
    /// <param name="output">The writer the output is written to.</param>
    public void Encode(Stream input, TextWriter output)
    {
        Encode(input, output, padding: false);
    }

    /// <summary>
    /// Encode a binary stream to a Base32 text stream.
    /// </summary>
    /// <param name="input">Input bytes.</param>
    /// <param name="output">The writer the output is written to.</param>
    /// <param name="padding">Whether to use padding at the end of the output.</param>
    public void Encode(Stream input, TextWriter output, bool padding)
    {
        StreamHelper.Encode(input, output, (buffer, lastBlock) =>
        {
            bool usePadding = lastBlock && padding;
            return Encode(buffer.Span, usePadding);
        });
    }

    /// <summary>
    /// Encode a binary stream to a Base32 text stream without padding.
    /// </summary>
    /// <param name="input">Input bytes.</param>
    /// <param name="output">The writer the output is written to.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task EncodeAsync(Stream input, TextWriter output)
    {
        return EncodeAsync(input, output, padding: false);
    }

    /// <summary>
    /// Encode a binary stream to a Base32 text stream.
    /// </summary>
    /// <param name="input">Input bytes.</param>
    /// <param name="output">The writer the output is written to.</param>
    /// <param name="padding">Whether to use padding at the end of the output.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task EncodeAsync(Stream input, TextWriter output, bool padding)
    {
        await StreamHelper.EncodeAsync(input, output, (buffer, lastBlock) =>
        {
            bool usePadding = lastBlock && padding;
            return Encode(buffer.Span, usePadding);
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Decode a text stream into a binary stream.
    /// </summary>
    /// <param name="input">TextReader open on the stream.</param>
    /// <param name="output">Binary output stream.</param>
    public void Decode(TextReader input, Stream output)
    {
        StreamHelper.Decode(input, output, buffer => Decode(buffer.Span));
    }

    /// <summary>
    /// Decode a text stream into a binary stream.
    /// </summary>
    /// <param name="input">TextReader open on the stream.</param>
    /// <param name="output">Binary output stream.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task DecodeAsync(TextReader input, Stream output)
    {
        await StreamHelper.DecodeAsync(input, output, buffer => Decode(buffer.Span))
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public bool TryEncode(ReadOnlySpan<byte> bytes, Span<char> output, out int numCharsWritten)
    {
        return TryEncode(bytes, output, padding: false, out numCharsWritten);
    }

    /// <summary>
    /// Encode to the given preallocated buffer.
    /// </summary>
    /// <param name="bytes">Input bytes.</param>
    /// <param name="output">Output buffer.</param>
    /// <param name="padding">Whether to use padding characters at the end.</param>
    /// <param name="numCharsWritten">Number of characters written to the output.</param>
    /// <returns>True if encoding is successful, false if the output is invalid.</returns>
    public unsafe bool TryEncode(
        ReadOnlySpan<byte> bytes,
        Span<char> output,
        bool padding,
        out int numCharsWritten)
    {
        int bytesLen = bytes.Length;
        if (bytesLen == 0)
        {
            numCharsWritten = 0;
            return true;
        }

        int outputLen = output.Length;

        fixed (byte* inputPtr = bytes)
        {
            fixed (char* outputPtr = output)
            {
                return internalEncode(inputPtr, bytesLen, outputPtr, outputLen, padding, out numCharsWritten);
            }
        }
    }

    /// <inheritdoc/>
    public unsafe bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int numBytesWritten)
    {
        int inputLen = input.Length - getPaddingCharCount(input);
        if (inputLen == 0)
        {
            numBytesWritten = 0;
            return true;
        }

        int outputLen = output.Length;
        if (outputLen == 0)
        {
            numBytesWritten = 0;
            return false;
        }

        fixed (char* inputPtr = input)
        {
            fixed (byte* outputPtr = output)
            {
                return internalDecode(inputPtr, inputLen, outputPtr, outputLen, out numBytesWritten);
            }
        }
    }

    private unsafe bool internalEncode(
       byte* inputPtr,
       int bytesLen,
       char* outputPtr,
       int outputLen,
       bool padding,
       out int numCharsWritten)
    {
        string table = Alphabet.Value;
        char* pOutput = outputPtr;
        char* pOutputEnd = outputPtr + outputLen;
        byte* pInput = inputPtr;
        byte* pInputEnd = pInput + bytesLen;

        for (int bitsLeft = bitsPerByte, currentByte = *pInput, outputPad; pInput != pInputEnd;)
        {
            if (bitsLeft > bitsPerChar)
            {
                bitsLeft -= bitsPerChar;
                outputPad = currentByte >> bitsLeft;
                *pOutput++ = table[outputPad];
                if (pOutput > pOutputEnd)
                {
                    goto Overflow;
                }

                currentByte &= (1 << bitsLeft) - 1;
            }

            int nextBits = bitsPerChar - bitsLeft;
            bitsLeft = bitsPerByte - nextBits;
            outputPad = currentByte << nextBits;
            if (++pInput != pInputEnd)
            {
                currentByte = *pInput;
                outputPad |= currentByte >> bitsLeft;
                currentByte &= (1 << bitsLeft) - 1;
            }

            *pOutput++ = table[outputPad];
            if (pOutput > pOutputEnd)
            {
                goto Overflow;
            }
        }

        if (padding)
        {
            char paddingChar = Alphabet.PaddingChar;
            while (pOutput != pOutputEnd)
            {
                *pOutput++ = paddingChar;
                if (pOutput > pOutputEnd)
                {
                    goto Overflow;
                }
            }
        }

        numCharsWritten = (int)(pOutput - outputPtr);
        return true;
    Overflow:
        numCharsWritten = (int)(pOutput - outputPtr);
        return false;
    }

    private static int getAllocationByteCountForDecoding(int textLenWithoutPadding)
    {
        return textLenWithoutPadding * bitsPerChar / bitsPerByte;
    }

    private int getPaddingCharCount(ReadOnlySpan<char> text)
    {
        char paddingChar = Alphabet.PaddingChar;
        int result = 0;
        int textLen = text.Length;

        if (Alphabet.PaddingPosition == PaddingPosition.Start)
        {
            foreach (char c in text)
            {
                if (c != paddingChar)
                {
                    return result;
                }

                result++;
            }

            return result;
        }

        while (textLen > 0 && text[--textLen] == paddingChar)
        {
            result++;
        }

        return result;
    }

    private unsafe bool internalDecode(
        char* inputPtr,
        int textLen,
        byte* outputPtr,
        int outputLen,
        out int numBytesWritten)
    {
        var table = Alphabet.ReverseLookupTable;
        int outputPad = 0;
        int bitsLeft = bitsPerByte;

        byte* pOutput = outputPtr;
        byte* pOutputEnd = pOutput + outputLen;
        char* pInput = inputPtr;
        char* pEnd = inputPtr + textLen;
        numBytesWritten = 0;
        while (pInput != pEnd)
        {
            char c = *pInput++;
            int b = table[c] - 1;
            if (b < 0)
            {
                numBytesWritten = (int)(pOutput - outputPtr);
                return false;
            }

            if (bitsLeft > bitsPerChar)
            {
                bitsLeft -= bitsPerChar;
                outputPad |= b << bitsLeft;
                continue;
            }

            int shiftBits = bitsPerChar - bitsLeft;
            outputPad |= b >> shiftBits;
            if (pOutput >= pOutputEnd)
            {
                Debug.WriteLine("Base32.internalDecode: output overflow");
                return false;
            }

            *pOutput++ = (byte)outputPad;
            numBytesWritten++;
            b &= (1 << shiftBits) - 1;
            bitsLeft = bitsPerByte - shiftBits;
            outputPad = b << bitsLeft;
        }

        return true;
    }
}
