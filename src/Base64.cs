// <copyright file="Base64.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2025 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

using System;
using System.Xml.Serialization;

namespace SimpleBase;

/// <summary>
/// Internal Base64 helpers.
/// </summary>
static class Base64
{
    const char paddingChar = '=';
    static readonly char[] urlSource = ['+', '/'];
    static readonly char[] urlTarget = ['-', '_'];

    internal static byte[] DecodeUrl(ReadOnlySpan<char> text)
    {
        var base64url = convertBase64UrlToBase64(text.ToString());
        return Convert.FromBase64String(base64url);
    }

    static string convertBase64UrlToBase64(string text)
    {
        return replaceMultiple(text, urlTarget, urlSource);
    }

    static string replaceMultiple(string text, char[] src, char[] dst)
    {
        if (text.Length == 0)
        {
            return text;
        }
        return string.Create(text.Length, (text, src, dst), static (span, state) =>
        {            
            var text = state.text;
            var src = state.src;
            var dst = state.dst;

            int lastPos = 0;
            int i;
            while (true)
            {
                i = text.IndexOfAny(src);
                if (i < 0)
                {
                    text.AsSpan(lastPos).CopyTo(span[lastPos..]);
                    break;
                }
                else
                {

                }
            }
        });
    }

    static string convertBase64ToBase64Url(string text)
    {
        return replaceMultiple(text, urlSource, urlTarget);
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
