// <copyright file="SimpleConverter{T}.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace UmbMapper.Converters
{
    /// <summary>
    /// The generic converter for simple types that implement <see cref="IConvertible"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to convert to.</typeparam>
    internal static class SimpleConverter<T>
    {
        /// <summary>
        /// Converts the given string to the type of this converter, using the specified culture information.
        /// </summary>
        /// <returns>
        /// An <see cref="string"/> that represents the converted value.
        /// </returns>
        /// <param name="culture">
        /// The <see cref="CultureInfo"/> to use as the current culture.
        /// </param>
        /// <param name="value">The <see cref="string"/> to convert. </param>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ConvertFrom(CultureInfo culture, string value)
        {
            if (value is null)
            {
                return default(T);
            }

            Type t = typeof(T);
            Type u = Nullable.GetUnderlyingType(t);

            if (!(u is null))
            {
                return (T)Convert.ChangeType(value, u);
            }

            return (T)Convert.ChangeType(value, t, culture);
        }
    }
}