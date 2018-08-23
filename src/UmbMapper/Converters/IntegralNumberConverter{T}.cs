// <copyright file="IntegralNumberConverter{T}.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Globalization;
using UmbMapper.Extensions;

namespace UmbMapper.Converters
{
    /// <summary>
    /// The generic converter for integral types.
    /// </summary>
    /// <typeparam name="T">The type of object to convert to.</typeparam>
    internal static class IntegralNumberConverter<T>
    {
        /// <summary>
        /// The collection of integral number types
        /// </summary>
        private static readonly Type[] IntegralTypes =
        {
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong)
        };

        /// <summary>
        /// Converts the given string to the type of this converter, using the specified culture information.
        /// </summary>
        /// <returns>An <see cref="string"/> that represents the converted value.</returns>
        /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="string"/> to convert.</param>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public static object ConvertFrom(CultureInfo culture, string value)
        {
            Type propertyType = typeof(T);

            if (value is null || Array.IndexOf(IntegralTypes, propertyType) < 0)
            {
                return default(T);
            }

            // Round the value to the nearest decimal value
            // ReSharper disable once PossibleNullReferenceException
            decimal rounded = Math.Round((decimal)Convert.ChangeType(value, typeof(decimal), culture), MidpointRounding.AwayFromZero);

            // Now clamp it to the type ranges
            if (propertyType.Equals(typeof(sbyte)))
            {
                rounded = rounded.Clamp(sbyte.MinValue, sbyte.MaxValue);
            }
            else if (propertyType.Equals(typeof(byte)))
            {
                rounded = rounded.Clamp(byte.MinValue, byte.MaxValue);
            }
            else if (propertyType.Equals(typeof(short)))
            {
                rounded = rounded.Clamp(short.MinValue, short.MaxValue);
            }
            else if (propertyType.Equals(typeof(ushort)))
            {
                rounded = rounded.Clamp(ushort.MinValue, ushort.MaxValue);
            }
            else if (propertyType.Equals(typeof(int)))
            {
                rounded = rounded.Clamp(int.MinValue, int.MaxValue);
            }
            else if (propertyType.Equals(typeof(uint)))
            {
                rounded = rounded.Clamp(uint.MinValue, uint.MaxValue);
            }
            else if (propertyType.Equals(typeof(long)))
            {
                rounded = rounded.Clamp(long.MinValue, long.MaxValue);
            }
            else if (propertyType.Equals(typeof(ulong)))
            {
                rounded = rounded.Clamp(ulong.MinValue, ulong.MaxValue);
            }

            // Now it's rounded an clamped we should be able to correctly parse the string.
            // ReSharper disable once PossibleNullReferenceException
            return (T)Convert.ChangeType(rounded.ToString(CultureInfo.InvariantCulture), typeof(T), culture);
        }
    }
}