// <copyright file="MultibaseEncoding.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

namespace SimpleBase;

/// <summary>
/// Currently supported Multibase encodings.
/// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public enum MultibaseEncoding
{
    // marked as "final" in the spec at https://github.com/multiformats/multibase/blob/master/multibase.csv
    Base16Lower     = 'f',
    Base16Upper     = 'F',
    Base32Lower     = 'b',
    Base32Upper     = 'B',
    Base58Bitcoin   = 'z',
    Base64          = 'm',
    Base64Pad       = 'M',
    Base64Url       = 'u',
    Base64UrlPad    = 'U',

    // marked as "draft"
    Base32Z         = 'h',
    Base36Lower     = 'k',
    Base36Upper     = 'K',
    Base45          = 'R',

    // marked as "experimental"
    Base58Flickr    = 'Z',
    Base32HexLower  = 'v',
    Base32HexUpper  = 'V',

    Base256Emoji    = 0xD83D, // high surrogate of '🚀' UTF-16 pair - ends with low surrogate 0xDE80
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
