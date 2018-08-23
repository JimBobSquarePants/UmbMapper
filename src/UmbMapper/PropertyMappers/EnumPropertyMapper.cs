// <copyright file="EnumPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UmbMapper.Extensions;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Maps from the <see cref="IPublishedContent"/> to an enum.
    /// </summary>
    public class EnumPropertyMapper : PropertyMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumPropertyMapper"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        public EnumPropertyMapper(PropertyMapInfo info)
            : base(info)
        {
        }

        /// <inheritdoc />
        public override object Map(IPublishedContent content, object value)
        {
            PropertyMapInfo info = this.Info;

            if (value is null)
            {
                return info.DefaultValue;
            }

            Type propertyType = info.PropertyType;
            CultureInfo culture = this.GetRequestCulture();

            if (value is string strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                if (strValue.IndexOf(',') != -1)
                {
                    long convertedValue = 0;

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (string v in strValue.ToDelimitedList())
                    {
                        // OR assignment. Stolen from ComponentModel EnumConverter.
                        convertedValue |= Convert.ToInt64((Enum)Enum.Parse(propertyType, v, true), culture);
                    }

                    return Enum.ToObject(propertyType, convertedValue);
                }

                return Enum.Parse(propertyType, strValue, true);
            }

            if (value is int)
            {
                // Should handle most cases.
                if (Enum.IsDefined(propertyType, value))
                {
                    return Enum.ToObject(propertyType, value);
                }
            }

            Type valueType = value.GetType();
            if (valueType.IsEnum)
            {
                // This should work for most cases where enums base type is int.
                return Enum.ToObject(propertyType, Convert.ToInt64(value, culture));
            }

            if (valueType.IsEnumerableOfType(typeof(string)))
            {
                long convertedValue = 0;
                var enumerable = ((IEnumerable<string>)value).ToList();

                if (enumerable.Count > 0)
                {
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (string v in enumerable)
                    {
                        convertedValue |= Convert.ToInt64((Enum)Enum.Parse(propertyType, v, true), culture);
                    }

                    return Enum.ToObject(propertyType, convertedValue);
                }

                return propertyType.GetInstance();
            }

            return info.DefaultValue;
        }
    }
}