// <copyright file="CustomBooleanTypeConverter.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.ComponentModel;

namespace UmbMapper.Converters
{
    /// <summary>
    /// Allows for converting string representations of 0 and 1 to boolean.
    /// </summary>
    /// <remarks>
    /// Due to the original code being internal this has been copied from
    /// <see href="https://github.com/umbraco/Umbraco-CMS/blob/7e1e83b493ed1a8c7e33826a7673d10fa46ccd69/src/Umbraco.Core/CustomBooleanTypeConverter.cs"/>
    /// </remarks>
    internal class CustomBooleanTypeConverter : BooleanConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType.Equals(typeof(string)))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string str)
            {
                if (string.IsNullOrEmpty(str) || str == "0")
                {
                    return false;
                }

                if (str == "1")
                {
                    return true;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}