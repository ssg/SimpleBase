// <copyright file="Base64.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;

namespace SimpleBase;

/// <summary>
/// Internal Base64 helpers.
/// </summary>
static class Base64
{
    const char paddingChar = '=';

    internal static byte[] DecodeUrl(ReadOnlySpan<char> text)
    {
        string base64 = convertBase64UrlToBase64(text);
        return Convert.FromBase64String(base64);
    }

    static string convertBase64UrlToBase64(ReadOnlySpan<char> text)
    {
        int len = text.Length;
        if (len == 0)
        {
            return string.Empty;
        }

        // .NET's Base64 decoder requires padding to be present
        int padLen = 0;
        if (text[len - 1] != paddingChar)
        {
            padLen = 4 - (len % 4);
        }
        len += padLen;
        Span<char> result = len < Bits.SafeStackMaxAllocSize ? stackalloc char[len] : new char[len];
        for (int i = 0; i < text.Length; i++)
        {
            result[i] = text[i] switch
            {
                '-' => '+',
                '_' => '/',
                _ => text[i]
            };
        }
        if (padLen > 0)
        {
            result[text.Length..].Fill(paddingChar);
        }
        return result.ToString();
    }

    internal static bool TryDecodeUrl(ReadOnlySpan<char> text, Span<byte> bytes, out int bytesWritten)
    {
        string base64 = convertBase64UrlToBase64(text);
        return Convert.TryFromBase64Chars(base64.AsSpan(), bytes, out bytesWritten);
    }

    static string convertBase64ToBase64Url(string text)
    {
        // i tried writing custom code to perform this replacement faster
        // but it turned out slower despite having half the allocation.
        // NOTE: padding char is the same between base64 and base64url
        return text.Replace('+', '-').Replace('/', '_');
    }

    static string stripBase64Padding(string base64Text)
    {
        return base64Text.TrimEnd(paddingChar);
    }

    internal static string EncodeUrl(ReadOnlySpan<byte> bytes)
    {
        var base64 = stripBase64Padding(Convert.ToBase64String(bytes));
        return convertBase64ToBase64Url(base64);
    }

    internal static string EncodeUrlPadded(ReadOnlySpan<byte> bytes)
    {
        var base64 = Convert.ToBase64String(bytes);
        return convertBase64ToBase64Url(base64);
    }
}
