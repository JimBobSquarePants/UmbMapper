// <copyright file="ArrayExtensions.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace UmbMapper.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Array"/> type.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns a new array with the item at the given index removed.
        /// </summary>
        /// <typeparam name="T">The type of array.</typeparam>
        /// <param name="array">The array to remove the item from.</param>
        /// <param name="index">The index of the item to remove.</param>
        /// <returns>The <see cref="T:T[]"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            var newArray = new T[array.Length - 1];
            for (int i = 0; i < newArray.Length; ++i)
            {
                newArray[i] = (i < index) ? array[i] : array[i + 1];
            }

            return newArray;
        }
    }
}