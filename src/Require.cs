﻿// <copyright file="Require.cs" company="Sedat Kapanoglu">
// Copyright (c) 2014-2019 Sedat Kapanoglu
// Licensed under Apache-2.0 License (see LICENSE.txt file for details)
// </copyright>

/*
     Copyright 2014-2016 Sedat Kapanoglu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
namespace SimpleBase
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class Require
    {
#if !DEBUG
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static void NotNull<T>(T value, string argName)
            where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(argName);
            }
        }
    }
}