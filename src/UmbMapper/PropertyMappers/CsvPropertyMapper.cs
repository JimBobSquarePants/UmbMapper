// <copyright file="CsvPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UmbMapper.Converters;
using UmbMapper.Extensions;
using Umbraco.Core.Models;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Maps from the <see cref="IPublishedContent"/> containing to a collection of items.
    /// </summary>
    public class CsvPropertyMapper : PropertyMapperBase
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

            Type typeArg = this.IsCastableEnumerableType
                ? this.PropertyType.GenericTypeArguments.First()
                : this.PropertyType;

            // Default to returning the string.
            Func<CultureInfo, string, Type, object> func = (i, s, t) => s;

            if (typeArg == TypeConstants.Sbyte)
            {
                func = IntegralNumberConverter<sbyte>.ConvertFrom;
            }
            else if (typeArg == TypeConstants.Byte)
            {
                func = IntegralNumberConverter<byte>.ConvertFrom;
            }
            else if (typeArg == TypeConstants.Short)
            {
                func = IntegralNumberConverter<short>.ConvertFrom;
            }
            else if (typeArg == TypeConstants.UShort)
            {
                func = IntegralNumberConverter<ushort>.ConvertFrom;
            }
            else if (typeArg == TypeConstants.Int)
            {
                func = IntegralNumberConverter<int>.ConvertFrom;
            }
            else if (typeArg == TypeConstants.UInt)
            {
                func = IntegralNumberConverter<uint>.ConvertFrom;
            }
            else if (typeArg == TypeConstants.Long)
            {
                func = IntegralNumberConverter<long>.ConvertFrom;
            }
            else if (typeArg == TypeConstants.ULong)
            {
                func = IntegralNumberConverter<ulong>.ConvertFrom;
            }
            else if (typeArg == TypeConstants.Float)
            {
                func = SimpleConverter<float>.ConvertFrom;
            }
            else if (typeArg == TypeConstants.Double)
            {
                func = SimpleConverter<double>.ConvertFrom;
            }
            else if (typeArg == TypeConstants.Decimal)
            {
                func = SimpleConverter<decimal>.ConvertFrom;
            }

            var result = new List<object>();
            string valueString = value.ToString();
            string[] items = this.GetStringArray(valueString, this.Culture);

            foreach (string s in items)
            {
                object item = func.Invoke(this.Culture, s, typeArg);
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
        protected string[] GetStringArray(string input, CultureInfo culture)
        {
            char separator = culture.TextInfo.ListSeparator[0];
            string[] result = input.Split(separator).Select(s => s.Trim()).ToArray();

            return result;
        }
    }
}