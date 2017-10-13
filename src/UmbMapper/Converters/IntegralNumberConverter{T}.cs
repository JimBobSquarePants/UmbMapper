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
        /// Converts the given string to the type of this converter, using the specified culture information.
        /// </summary>
        /// <returns>
        /// An <see cref="string"/> that represents the converted value.
        /// </returns>
        /// <param name="culture">
        /// The <see cref="CultureInfo"/> to use as the current culture.
        /// </param>
        /// <param name="value">The <see cref="string"/> to convert. </param>
        /// <param name="propertyType">The property type that the converter will convert to.</param>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public static object ConvertFrom(CultureInfo culture, string value, Type propertyType)
        {
            if (value == null || Array.IndexOf(TypeConstants.IntegralTypes, propertyType) < 0)
            {
                return default(T);
            }

            // Round the value to the nearest decimal value
            // ReSharper disable once PossibleNullReferenceException
            decimal rounded = Math.Round((decimal)Convert.ChangeType(value, typeof(decimal), culture), MidpointRounding.AwayFromZero);

            // Now clamp it to the type ranges
            if (propertyType == TypeConstants.Sbyte)
            {
                rounded = rounded.Clamp(sbyte.MinValue, sbyte.MaxValue);
            }
            else if (propertyType == TypeConstants.Byte)
            {
                rounded = rounded.Clamp(byte.MinValue, byte.MaxValue);
            }
            else if (propertyType == TypeConstants.Short)
            {
                rounded = rounded.Clamp(short.MinValue, short.MaxValue);
            }
            else if (propertyType == TypeConstants.UShort)
            {
                rounded = rounded.Clamp(ushort.MinValue, ushort.MaxValue);
            }
            else if (propertyType == TypeConstants.Int)
            {
                rounded = rounded.Clamp(int.MinValue, int.MaxValue);
            }
            else if (propertyType == TypeConstants.UInt)
            {
                rounded = rounded.Clamp(uint.MinValue, uint.MaxValue);
            }
            else if (propertyType == TypeConstants.Long)
            {
                rounded = rounded.Clamp(long.MinValue, long.MaxValue);
            }
            else if (propertyType == TypeConstants.ULong)
            {
                rounded = rounded.Clamp(ulong.MinValue, ulong.MaxValue);
            }

            // Now it's rounded an clamped we should be able to correctly parse the string.
            // ReSharper disable once PossibleNullReferenceException
            return (T)Convert.ChangeType(rounded.ToString(CultureInfo.InvariantCulture), typeof(T), culture);
        }
    }
}