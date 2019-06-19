// <copyright file="CsvPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UmbMapper.Converters;
using UmbMapper.Extensions;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Maps from the <see cref="IPublishedContent"/> containing to a collection of items.
    /// </summary>
    public sealed class CsvPropertyMapper : PropertyMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvPropertyMapper"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        public CsvPropertyMapper(PropertyMapInfo info)
            : base(info)
        {
        }

        /// <inheritdoc/>
        public override object Map(IPublishedContent content, object value)
        {
            if (value.IsNullOrWhiteSpaceString())
            {
                return Enumerable.Empty<object>();
            }

            PropertyMapInfo info = this.Info;
            CultureInfo culture = this.GetRequestCulture();
            Type typeArg = info.IsCastableEnumerableType
                ? info.EnumerableParamType
                : info.PropertyType;

            // Default to returning the string.
            Func<CultureInfo, string, object> func = (_, s) => s;

            if (typeArg == typeof(sbyte))
            {
                func = IntegralNumberConverter<sbyte>.ConvertFrom;
            }
            else if (typeArg == typeof(byte))
            {
                func = IntegralNumberConverter<byte>.ConvertFrom;
            }
            else if (typeArg == typeof(short))
            {
                func = IntegralNumberConverter<short>.ConvertFrom;
            }
            else if (typeArg == typeof(ushort))
            {
                func = IntegralNumberConverter<ushort>.ConvertFrom;
            }
            else if (typeArg == typeof(int))
            {
                func = IntegralNumberConverter<int>.ConvertFrom;
            }
            else if (typeArg == typeof(uint))
            {
                func = IntegralNumberConverter<uint>.ConvertFrom;
            }
            else if (typeArg == typeof(long))
            {
                func = IntegralNumberConverter<long>.ConvertFrom;
            }
            else if (typeArg == typeof(ulong))
            {
                func = IntegralNumberConverter<ulong>.ConvertFrom;
            }
            else if (typeArg == typeof(float))
            {
                func = SimpleConverter<float>.ConvertFrom;
            }
            else if (typeArg == typeof(double))
            {
                func = SimpleConverter<double>.ConvertFrom;
            }
            else if (typeArg == typeof(decimal))
            {
                func = SimpleConverter<decimal>.ConvertFrom;
            }

            var result = new List<object>();
            string valueString = value.ToString();
            foreach (string s in GetStringArray(valueString, culture))
            {
                object item = func.Invoke(culture, s);
                if (item != null)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Splits a string by separator to return an array of string values.
        /// </summary>
        /// <param name="input">The input string to split.</param>
        /// <param name="culture">A <see cref="CultureInfo"/>. The current culture to split string by.</param>
        /// <returns>The <see cref="T:String[]"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string[] GetStringArray(string input, CultureInfo culture)
        {
            char separator = culture.TextInfo.ListSeparator[0];
            string[] split = input.Split(separator);
            string[] result = new string[split.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = split[i].Trim();
            }

            return result;
        }
    }
}