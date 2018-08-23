// <copyright file="VortoPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Globalization;
using Our.Umbraco.Vorto.Extensions;
using Our.Umbraco.Vorto.Models;
using UmbMapper.Extensions;
using Umbraco.Core.Models;

namespace UmbMapper.PropertyMappers.Vorto
{
    /// <summary>
    /// Maps from a Vorto wrapped <see cref="IPublishedContent"/>.
    /// </summary>
    public class VortoPropertyMapper : PropertyMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VortoPropertyMapper"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        public VortoPropertyMapper(PropertyMapInfo info)
            : base(info)
        {
        }

        /// <inheritdoc/>
        public override object Map(IPublishedContent content, object value)
        {
            PropertyMapInfo info = this.Info;
            string culture = this.GetRequestCulture().Name;
            string fallbackCultureName = CultureInfo.CurrentCulture.Name;

            if (value is VortoValue vortoValue
                && content.HasVortoValue(this.Alias, culture, info.Recursive, fallbackCultureName))
            {
                value = this.CheckConvertType(content.GetVortoValue(this.Alias, culture, info.Recursive, fallbackCultureName));
            }
            else
            {
                // Vorto can have values in some cultures but not others.
                // We want to be able to provide fallbacks for when there is no value for the given culture we want.
                int index = Array.IndexOf(info.Aliases, this.Alias);
                value = this.CheckConvertType(this.GetRawValue(content, info.Aliases.RemoveAt(index)));
            }

            return value ?? info.DefaultValue;
        }
    }
}