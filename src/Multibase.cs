// <copyright file="Multibase.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Text;

namespace SimpleBase;

/// <summary>
/// Currently supported Multibase encodings.
/// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public enum MultibaseEncoding
{
    // marked as "final" in the spec at https://github.com/multiformats/multibase/blob/master/multibase.csv
    Base16          = 'f',
    Base16Upper     = 'F',
    Base32          = 'b',
    Base32Upper     = 'B',
    Base58Bitcoin   = 'z',
    Base64          = 'm',
    Base64Url       = 'u',
    Base64UrlPad    = 'U',

    // marked as "draft"
    Base32Z         = 'h',

    // marked as "experimental"
    Base58Flickr    = 'Z',
    Base32Hex       = 'v',
    Base32HexUpper  = 'V',
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

/// <summary>
/// Multibase encoding and decoding.
/// </summary>
public static class Multibase
{
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
            MultibaseEncoding.Base16 => Base16.LowerCase.Decode(rest),
            MultibaseEncoding.Base16Upper => Base16.UpperCase.Decode(rest),
            MultibaseEncoding.Base32 => Base32.FileCoin.Decode(rest),
            MultibaseEncoding.Base32Upper => Base32.Rfc4648.Decode(rest),
            MultibaseEncoding.Base32Hex => Base32.ExtendedHexLower.Decode(rest),
            MultibaseEncoding.Base32HexUpper => Base32.ExtendedHex.Decode(rest),
            MultibaseEncoding.Base32Z => Base32.ZBase32.Decode(rest),
            MultibaseEncoding.Base58Bitcoin => Base58.Bitcoin.Decode(rest),
            MultibaseEncoding.Base58Flickr => Base58.Flickr.Decode(rest),
            MultibaseEncoding.Base64 => Convert.FromBase64String(rest.ToString()),
            MultibaseEncoding.Base64Url or MultibaseEncoding.Base64UrlPad => Base64.DecodeUrl(rest),
            _ => throw new InvalidOperationException($"Unsupported multibase prefix: {c}"),
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
        var builder = new StringBuilder()
            .Append((char)encoding)
            .Append(encoding switch
            {
                MultibaseEncoding.Base16 => Base16.LowerCase.Encode(bytes),
                MultibaseEncoding.Base16Upper => Base16.UpperCase.Encode(bytes),
                MultibaseEncoding.Base32 => Base32.FileCoin.Encode(bytes),
                MultibaseEncoding.Base32Upper => Base32.Rfc4648.Encode(bytes),
                MultibaseEncoding.Base32Hex => Base32.ExtendedHexLower.Encode(bytes),
                MultibaseEncoding.Base32HexUpper => Base32.ExtendedHex.Encode(bytes),
                MultibaseEncoding.Base32Z => Base32.ZBase32.Encode(bytes),
                MultibaseEncoding.Base58Bitcoin => Base58.Bitcoin.Encode(bytes),
                MultibaseEncoding.Base58Flickr => Base58.Flickr.Encode(bytes),
                MultibaseEncoding.Base64 => Convert.ToBase64String(bytes),
                MultibaseEncoding.Base64Url => Base64.EncodeUrl(bytes),
                MultibaseEncoding.Base64UrlPad => Base64.EncodeUrlPadded(bytes),
                _ => throw new ArgumentException($"Unsupported encoding type: {encoding}", nameof(encoding)),
            });
        return builder.ToString();
    }
}
