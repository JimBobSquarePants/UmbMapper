// <copyright file="VortoPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Globalization;
using Our.Umbraco.Vorto.Extensions;
using UmbMapper.PropertyMappers;
using Umbraco.Core.Models;

namespace UmbMapper.Sample.ComponentModel.PropertyMappers
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
            foreach (string name in info.Aliases)
            {
                if (content.HasVortoValue(name, culture, info.Recursive, CultureInfo.CurrentCulture.Name))
                {
                    value = this.CheckConvertType(content.GetVortoValue(name, culture, info.Recursive));

                    if (info.PropertyType.IsInstanceOfType(value))
                    {
                        break;
                    }
                }
            }

            return value ?? info.DefaultValue;
        }
    }
}