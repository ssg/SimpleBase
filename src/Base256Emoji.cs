// <copyright file="Base256.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SimpleBase;

/// <summary>
/// Base256 encoding using variable-length emojis.
/// </summary>
/// <remarks>
/// The encoded string might consist of one or more UTF-16
/// code points (char's) for each byte. Be wary of this
/// when processing the encoded strings.
/// </remarks>
public class Base256Emoji : IBaseCoder, INonAllocatingBaseCoder
{
    readonly string[] alphabet = createDefaultAlphabet();
    readonly Dictionary<int, byte> reverseAlphabet = new(256);

    static string[] createDefaultAlphabet()
    {
        // because emojis in this alphabet take differing lengths in UTF-16,
        // we can't calculate alphabet indices or buffer lengths solely
        // based on number of characters or index.
        var enumerator = StringInfo.GetTextElementEnumerator("🚀🪐☄🛰🌌🌑🌒🌓🌔🌕🌖🌗🌘🌍🌏🌎🐉☀💻🖥💾💿😂❤😍🤣😊🙏💕😭😘👍😅👏😁🔥🥰💔💖💙😢🤔😆🙄💪😉☺👌🤗💜😔😎😇🌹🤦🎉💞✌✨🤷😱😌🌸🙌😋💗💚😏💛🙂💓🤩😄😀🖤😃💯🙈👇🎶😒🤭❣😜💋👀😪😑💥🙋😞😩😡🤪👊🥳😥🤤👉💃😳✋😚😝😴🌟😬🙃🍀🌷😻😓⭐✅🥺🌈😈🤘💦✔😣🏃💐☹🎊💘😠☝😕🌺🎂🌻😐🖕💝🙊😹🗣💫💀👑🎵🤞😛🔴😤🌼😫⚽🤙☕🏆🤫👈😮🙆🍻🍃🐶💁😲🌿🧡🎁⚡🌞🎈❌✊👋😰🤨😶🤝🚶💰🍓💢🤟🙁🚨💨🤬✈🎀🍺🤓😙💟🌱😖👶🥴▶➡❓💎💸⬇😨🌚🦋😷🕺⚠🙅😟😵👎🤲🤠🤧📌🔵💅🧐🐾🍒😗🤑🌊🤯🐷☎💧😯💆👆🎤🙇🍑❄🌴💣🐸💌📍🥀🤢👅💡💩👐📸👻🤐🤮🎼🥵🚩🍎🍊👼💍📣🥂");
        var list = new List<string>(256);
        while (enumerator.MoveNext())
        {
            list.Add(enumerator.GetTextElement());
        }
        return list.ToArray();
    }

    static readonly Lazy<Base256Emoji> @default = new(() => new Base256Emoji(createDefaultAlphabet()));

    /// <summary>
    /// Default Base256Emoji instance.
    /// </summary>
    public static Base256Emoji Default => @default.Value;

    /// <summary>
    /// Create a new instance of Base256Emoji.
    /// </summary>
    /// <param name="alphabet">An array that contains 256 elements with emoji values corresponding to every byte.</param>
    public Base256Emoji(string[] alphabet)
    {
        if (alphabet.Length != 256)
        {
            throw new ArgumentException("Alphabet must be 256 characters long", nameof(alphabet));
        }

        this.alphabet = alphabet;
        reverseAlphabet = alphabet
            .Select((s, i) => (s, i))
            .ToDictionary(pair => char.ConvertToUtf32(pair.s, 0), pair => (byte)pair.i);
    }

    /// <inheritdoc/>
    public byte[] Decode(ReadOnlySpan<char> text)
    {
        int outputLen = GetSafeByteCountForDecoding(text);
        Span<byte> output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc byte[outputLen] : new byte[outputLen];
        return internalDecode(text, output, out int bytesWritten) switch
        {
            (DecodeResult.Success, _) => output[..bytesWritten].ToArray(),
            (DecodeResult.InvalidEmoji, int utf32) => throw new ArgumentException($"Invalid emoji: {char.ConvertFromUtf32(utf32)}", nameof(text)),
            (DecodeResult.MissingSurrogatePair, int utf32) => throw new ArgumentException($"Missing surrogate pair after char '\\u{(ushort)utf32:X4}'", nameof(text)),
            (DecodeResult.UnexpectedSurrogate, int utf32) => throw new ArgumentException($"Unexpected low surrogate character '\\u{(ushort)utf32:X4}'", nameof(text)),
            (DecodeResult.InsufficientOutputBuffer, _) => throw new InvalidOperationException("Buffer is too small for decoding -- likely a bug"),
            _ => throw new InvalidOperationException("Should never hit here -- likely a bug"),
        };
    }

    /// <inheritdoc/>
    public string Encode(ReadOnlySpan<byte> bytes)
    {
        int outputLen = GetSafeCharCountForEncoding(bytes);
        Span<char> output = outputLen < Bits.SafeStackMaxAllocSize ? stackalloc char[outputLen] : new char[outputLen];
        if (!TryEncode(bytes, output, out int charsWritten))
        {
            throw new ArgumentException("Buffer is too small for encoding");
        }
        return new string(output[..charsWritten]);
    }

    /// <inheritdoc/>
    public int GetSafeByteCountForDecoding(ReadOnlySpan<char> text)
    {
        // this is bigger than exact size but better than counting every character
        return text.Length;
    }

    /// <inheritdoc/>
    public int GetSafeCharCountForEncoding(ReadOnlySpan<byte> buffer)
    {
        // this is bigger than exact size but better than calculating every emoji footprint
        return buffer.Length * 2;
    }

    /// <inheritdoc/>
    public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        return internalDecode(input, output, out bytesWritten) is (DecodeResult.Success, _);
    }

    enum DecodeResult
    {
        Success,
        MissingSurrogatePair,
        UnexpectedSurrogate,
        InvalidEmoji,
        InsufficientOutputBuffer
    }

    (DecodeResult, int?) internalDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        bytesWritten = 0;
        int i = 0;
        while (i < input.Length)
        {
            char c = input[i++];
            int utf32;
            if (char.IsHighSurrogate(c))
            {
                if (i == input.Length)
                {
                    // string ends with an orphaned high surrogate
                    return (DecodeResult.MissingSurrogatePair, c);
                }
                utf32 = char.ConvertToUtf32(c, input[i++]);
            }
            else if (char.IsLowSurrogate(c))
            {
                // low surrogate without high surrogate
                return (DecodeResult.UnexpectedSurrogate, c);
            }
            else
            {
                utf32 = c;
            }
            if (!reverseAlphabet.TryGetValue(utf32, out byte b))
            {
                return (DecodeResult.InvalidEmoji, utf32);
            }
            if (bytesWritten >= output.Length)
            {
                return (DecodeResult.InsufficientOutputBuffer, null);
            }
            output[bytesWritten++] = b;
        }
        return (DecodeResult.Success, null);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// numCharsWritten will not correspond to the number of emojis written as one emoji
    /// can be encoded with multiple chars.
    /// </remarks>
    public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int numCharsWritten)
    {
        numCharsWritten = 0;
        for (int i = 0; i < input.Length; i++)
        {
            byte b = input[i];
            string value = alphabet[b];
            if (numCharsWritten + value.Length > output.Length)
            {
                // buffer overflow
                return false;
            }
            value.CopyTo(output[numCharsWritten..]);
            numCharsWritten += value.Length;
        }
        return true;
    }
}
