// <copyright file="Base58.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Diagnostics;

namespace SimpleBase;

/// <summary>
/// Base58 Encoding/Decoding implementation.
/// </summary>
/// <remarks>
/// Base58 doesn't implement a Stream-based interface because it's not feasible to use
/// on large buffers.
/// </remarks>
public sealed class Base58 : IBaseCoder, INonAllocatingBaseCoder
{
    private const int reductionFactor = 733; // https://github.com/bitcoin/bitcoin/blob/master/src/base58.cpp#L48
    private const int divisor = 58;
    private static readonly Lazy<Base58> bitcoin = new(() => new Base58(Base58Alphabet.Bitcoin));
    private static readonly Lazy<Base58> ripple = new(() => new Base58(Base58Alphabet.Ripple));
    private static readonly Lazy<Base58> flickr = new(() => new Base58(Base58Alphabet.Flickr));

    /// <summary>
    /// Initializes a new instance of the <see cref="Base58"/> class
    /// using a custom alphabet.
    /// </summary>
    /// <param name="alphabet">Alphabet to use.</param>
    public Base58(Base58Alphabet alphabet)
    {
        Alphabet = alphabet;
        ZeroChar = alphabet.Value[0];
    }

    /// <summary>
    /// Gets Bitcoin flavor.
    /// </summary>
    public static Base58 Bitcoin => bitcoin.Value;

    /// <summary>
    /// Gets Ripple flavor.
    /// </summary>
    public static Base58 Ripple => ripple.Value;

    /// <summary>
    /// Gets Flickr flavor.
    /// </summary>
    public static Base58 Flickr => flickr.Value;

    /// <summary>
    /// Gets the encoding alphabet.
    /// </summary>
    public Base58Alphabet Alphabet { get; }

    /// <summary>
    /// Gets the character for zero.
    /// </summary>
    public char ZeroChar { get; }

    /// <inheritdoc/>
    public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        int textLen = text.Length;
        return GetSafeByteCountForDecoding(textLen, getPrefixCount(text, textLen, ZeroChar));
    }

    /// <summary>
    /// Retrieve safe byte count while avoiding multiple counting operations.
    /// </summary>
    /// <param name="textLen">Length of text.</param>
    /// <param name="numZeroes">Number of prefix zeroes.</param>
    /// <returns>Length of safe allocation.</returns>
    public static int GetSafeByteCountForDecoding(int textLen, int numZeroes)
    {
        Debug.Assert(textLen >= numZeroes, "Number of zeroes cannot be longer than text length");
        return numZeroes + ((textLen - numZeroes + 1) * reductionFactor / 1000) + 1;
    }

    /// <inheritdoc/>
    public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> bytes)
    {
        int bytesLen = bytes.Length;
        int numZeroes = getZeroCount(bytes);

        return getSafeCharCountForEncoding(bytesLen, numZeroes);
    }

    /// <summary>
    /// Encode to Base58 representation.
    /// </summary>
    /// <param name="bytes">Bytes to encode.</param>
    /// <returns>Encoded string.</returns>
    public unsafe string Encode(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
        {
            return string.Empty;
        }

        int numZeroes = getZeroCount(bytes);
        int outputLen = getSafeCharCountForEncoding(bytes.Length, numZeroes);
        Span<char> output = new char[outputLen];

        return internalEncode(bytes, output, numZeroes, out int numCharsWritten)
            ? new string(output[..numCharsWritten])
            : throw new InvalidOperationException("Output buffer with insufficient size generated");
    }

    /// <summary>
    /// Decode a Base58 representation.
    /// </summary>
    /// <param name="text">Base58 encoded text.</param>
    /// <returns>Decoded bytes.</returns>
    public unsafe byte[] Decode(ReadOnlySpan<char> text)
    {
        int textLen = text.Length;
        if (textLen == 0)
        {
            return Array.Empty<byte>();
        }

        char zeroChar = ZeroChar;
        int numZeroes = getPrefixCount(text, textLen, zeroChar);
        int outputLen = GetSafeByteCountForDecoding(textLen, numZeroes);
        byte[] output = new byte[outputLen];
        fixed (char* inputPtr = text)
        {
            fixed (byte* outputPtr = output)
            {
#pragma warning disable IDE0046 // Convert to conditional expression - prefer clarity
                if (!internalDecode(
                    inputPtr,
                    textLen,
                    outputPtr,
                    outputLen,
                    numZeroes,
                    out int numBytesWritten))
                {
                    throw new InvalidOperationException("Output buffer was too small while decoding Base58");
                }

                return output[..numBytesWritten];
#pragma warning restore IDE0046 // Convert to conditional expression
            }
        }
    }

    /// <inheritdoc/>
    public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int numCharsWritten)
    {
        int numZeroes = getZeroCount(input);
        return internalEncode(input, output, numZeroes, out numCharsWritten);
    }

    /// <inheritdoc/>
    public unsafe bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int numBytesWritten)
    {
        int inputLen = input.Length;
        if (inputLen == 0)
        {
            numBytesWritten = 0;
            return true;
        }

        int zeroCount = getPrefixCount(input, inputLen, ZeroChar);
        fixed (char* inputPtr = input)
        {
            fixed (byte* outputPtr = output)
            {
                return internalDecode(
                    inputPtr,
                    input.Length,
                    outputPtr,
                    output.Length,
                    zeroCount,
                    out numBytesWritten);
            }
        }
    }

    private unsafe bool internalDecode(
        char* inputPtr,
        int inputLen,
        byte* outputPtr,
        int outputLen,
        int numZeroes,
        out int numBytesWritten)
    {
        char* pInputEnd = inputPtr + inputLen;
        char* pInput = inputPtr + numZeroes;
        if (pInput == pInputEnd)
        {
            if (numZeroes > outputLen)
            {
                numBytesWritten = 0;
                return false;
            }

            byte* pOutput = outputPtr;
            for (int i = 0; i < numZeroes; i++)
            {
                *pOutput++ = 0;
            }

            numBytesWritten = numZeroes;
            return true;
        }

        var table = Alphabet.ReverseLookupTable;
        byte* pOutputEnd = outputPtr + outputLen - 1;
        byte* pMinOutput = pOutputEnd;
        while (pInput != pInputEnd)
        {
            char c = *pInput;
            int carry = table[c] - 1;
            if (carry < 0)
            {
                throw CodingAlphabet.InvalidCharacter(c);
            }

            for (byte* pOutput = pOutputEnd; pOutput >= outputPtr; pOutput--)
            {
                carry += 58 * (*pOutput);
                *pOutput = (byte)carry;
                if (pMinOutput > pOutput && carry != 0)
                {
                    pMinOutput = pOutput;
                }

                carry >>= 8;
            }

            pInput++;
        }

        int startIndex = (int)(pMinOutput - numZeroes - outputPtr);
        numBytesWritten = outputLen - startIndex;
        Buffer.MemoryCopy(outputPtr + startIndex, outputPtr, numBytesWritten, numBytesWritten);
        return true;
    }

    private bool internalEncode(
        ReadOnlySpan<byte> input,
        Span<char> output,
        int numZeroes,
        out int numCharsWritten)
    {
        if (input.Length == 0)
        {
            numCharsWritten = 0;
            return true;
        }

        ReadOnlySpan<char> alphabet = Alphabet.Value;
        if (numZeroes == input.Length)
        {
            return encodeAllZeroes(output, numZeroes, out numCharsWritten, alphabet[0]);
        }

        int numDigits = 0;
        int index = numZeroes;
        while (index < input.Length)
        {
            int carry = input[index++];
            int i = 0;
            for (int j = output.Length - 1; (carry != 0 || i < numDigits)
                && j >= 0; j--, i++)
            {
                carry += output[j] << 8;
                carry = Math.DivRem(carry, divisor, out int remainder);
                output[j] = (char)remainder;
            }

            numDigits = i;
        }

        translatedCopy(output[^numDigits..], output[numZeroes..], alphabet);
        if (numZeroes > 0)
        {
            output[..numZeroes].Fill(alphabet[0]);
        }

        numCharsWritten = numZeroes + numDigits;
        return true;
    }

    private static void translatedCopy(
        ReadOnlySpan<char> source,
        Span<char> destination,
        ReadOnlySpan<char> alphabet)
    {
        Debug.Assert(source.Length <= destination.Length, "source is too big");
        for (int n = 0; n < source.Length; n++)
        {
            destination[n] = alphabet[source[n]];
        }
    }

    private static bool encodeAllZeroes(
        Span<char> output,
        int numZeroes,
        out int numCharsWritten,
        char zeroChar)
    {
        if (output.Length < numZeroes)
        {
            // insufficient output buffer length
            numCharsWritten = 0;
            return false;
        }

        output[..numZeroes].Fill(zeroChar);
        numCharsWritten = numZeroes;
        return true;
    }

    private static int getZeroCount(ReadOnlySpan<byte> bytes)
    {
        int numZeroes = 0;
        for (int i = 0; i < bytes.Length; i++)
        {
            if (bytes[i] != 0)
            {
                break;
            }

            numZeroes += 1;
        }

        return numZeroes;
    }

    private static unsafe int getPrefixCount(ReadOnlySpan<char> input, int length, char value)
    {
        if (length == 0)
        {
            return 0;
        }

        int numZeroes = 0;
        fixed (char* inputPtr = input)
        {
            var pInput = inputPtr;
            while (*pInput == value && numZeroes < length)
            {
                numZeroes++;
                pInput++;
            }
        }

        return numZeroes;
    }

    private static int getSafeCharCountForEncoding(int bytesLen, int numZeroes)
    {
        const int growthPercentage = 138;

        return numZeroes + ((bytesLen - numZeroes) * growthPercentage / 100) + 1;
    }
}
