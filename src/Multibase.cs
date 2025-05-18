// <copyright file="Multibase.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Multibase encoding and decoding.
/// </summary>
public static class Multibase
{
    const char base256EmojiLowSurrogate = '\uDE80';
    const string base256EmojiPrefix = "🚀";

    /// <summary>
    /// Decodes a multibase encoded string.
    /// </summary>
    /// <param name="text">Input text.</param>
    /// <returns>Decoded bytes.</returns>
    /// <exception cref="ArgumentException">If the text is empty or has unsupported encoding.</exception>
    public static byte[] Decode(ReadOnlySpan<char> text)
    {
        if (text.Length == 0)
        {
            throw new ArgumentException("Text cannot be empty", nameof(text));
        }

        char c = text[0];
        var encoding = (MultibaseEncoding)c;
        var rest = text[1..];
        return encoding switch
        {
            MultibaseEncoding.Base2 => Base2.Default.Decode(rest),
            MultibaseEncoding.Base8 => Base8.Default.Decode(rest),
            MultibaseEncoding.Base10 => Base10.Default.Decode(rest),
            MultibaseEncoding.Base16Lower => Base16.LowerCase.Decode(rest),
            MultibaseEncoding.Base16Upper => Base16.UpperCase.Decode(rest),
            MultibaseEncoding.Base32Lower => Base32.FileCoin.Decode(rest),
            MultibaseEncoding.Base32Upper => Base32.Rfc4648.Decode(rest),
            MultibaseEncoding.Base32HexLower => Base32.ExtendedHexLower.Decode(rest),
            MultibaseEncoding.Base32HexUpper => Base32.ExtendedHex.Decode(rest),
            MultibaseEncoding.Base32Z => Base32.ZBase32.Decode(rest),
            MultibaseEncoding.Base36Lower => Base36.LowerCase.Decode(rest),
            MultibaseEncoding.Base36Upper => Base36.UpperCase.Decode(rest),
            MultibaseEncoding.Base45 => Base45.Default.Decode(rest),
            MultibaseEncoding.Base58Bitcoin => Base58.Bitcoin.Decode(rest),
            MultibaseEncoding.Base58Flickr => Base58.Flickr.Decode(rest),
            MultibaseEncoding.Base64Pad => Convert.FromBase64String(rest.ToString()),
            MultibaseEncoding.Base64 => Base64.DecodeWithoutPadding(rest),
            MultibaseEncoding.Base64Url or MultibaseEncoding.Base64UrlPad => Base64.DecodeUrl(rest),
            MultibaseEncoding.Base256Emoji
                when rest.Length == 0
                    || rest[0] == base256EmojiLowSurrogate => Base256Emoji.Default.Decode(rest[1..]),
            _ => throw new InvalidOperationException($"Unsupported multibase prefix: {c}"),
        };
    }

    /// <summary>
    /// Tries to decode a multibase encoded string into a span of bytes.
    /// </summary>
    /// <param name="text">Input text.</param>
    /// <param name="bytes">Output span.</param>
    /// <param name="bytesWritten">Number of bytes written to the output span.</param>
    /// <returns>True if successful, false otherwise.</returns>
    public static bool TryDecode(ReadOnlySpan<char> text, Span<byte> bytes, out int bytesWritten)
    {
        bytesWritten = 0;
        if (text.Length == 0)
        {
            return false;
        }
        char c = text[0];
        var encoding = (MultibaseEncoding)c;
        var rest = text[1..];
        return encoding switch
        {
            MultibaseEncoding.Base2 => Base2.Default.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base8 => Base8.Default.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base10 => Base10.Default.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base16Lower => Base16.LowerCase.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base16Upper => Base16.UpperCase.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base32Lower => Base32.FileCoin.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base32Upper => Base32.Rfc4648.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base32HexLower => Base32.ExtendedHexLower.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base32HexUpper => Base32.ExtendedHex.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base32Z => Base32.ZBase32.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base36Lower => Base36.LowerCase.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base36Upper => Base36.UpperCase.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base45 => Base45.Default.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base58Bitcoin => Base58.Bitcoin.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base58Flickr => Base58.Flickr.TryDecode(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base64 => Base64.TryDecodeWithoutPadding(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base64Pad => Convert.TryFromBase64Chars(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base64Url or MultibaseEncoding.Base64UrlPad => Base64.TryDecodeUrl(rest, bytes, out bytesWritten),
            MultibaseEncoding.Base256Emoji
                when rest.Length == 0
                    || rest[0] == base256EmojiLowSurrogate => Base256Emoji.Default.TryDecode(rest[1..], bytes, out bytesWritten),
            _ => false,
        };
    }

    /// <summary>
    /// Encodes a byte array into a multibase encoded string with given encoding.
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static string Encode(ReadOnlySpan<byte> bytes, MultibaseEncoding encoding)
    {
        return encoding switch
        {
            MultibaseEncoding.Base256Emoji => base256EmojiPrefix + Base256Emoji.Default.Encode(bytes),
            _ => (char)encoding + encoding switch
            {
                MultibaseEncoding.Base2 => Base2.Default.Encode(bytes),
                MultibaseEncoding.Base8 => Base8.Default.Encode(bytes),
                MultibaseEncoding.Base10 => Base10.Default.Encode(bytes),
                MultibaseEncoding.Base16Lower => Base16.LowerCase.Encode(bytes),
                MultibaseEncoding.Base16Upper => Base16.UpperCase.Encode(bytes),
                MultibaseEncoding.Base32Lower => Base32.FileCoin.Encode(bytes),
                MultibaseEncoding.Base32Upper => Base32.Rfc4648.Encode(bytes),
                MultibaseEncoding.Base32HexLower => Base32.ExtendedHexLower.Encode(bytes),
                MultibaseEncoding.Base32HexUpper => Base32.ExtendedHex.Encode(bytes),
                MultibaseEncoding.Base32Z => Base32.ZBase32.Encode(bytes),
                MultibaseEncoding.Base36Lower => Base36.LowerCase.Encode(bytes),
                MultibaseEncoding.Base36Upper => Base36.UpperCase.Encode(bytes),
                MultibaseEncoding.Base45 => Base45.Default.Encode(bytes),
                MultibaseEncoding.Base58Bitcoin => Base58.Bitcoin.Encode(bytes),
                MultibaseEncoding.Base58Flickr => Base58.Flickr.Encode(bytes),
                MultibaseEncoding.Base64 => Base64.EncodeWithoutPadding(bytes),
                MultibaseEncoding.Base64Pad => Convert.ToBase64String(bytes),
                MultibaseEncoding.Base64Url => Base64.EncodeUrl(bytes),
                MultibaseEncoding.Base64UrlPad => Base64.EncodeUrlPadded(bytes),
                _ => throw new ArgumentException($"Unsupported encoding type: {encoding}", nameof(encoding)),
            }
        };
    }
}
