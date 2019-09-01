// <copyright file="UmbracoPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Maps directly from the published content
    /// </summary>
    public sealed class UmbracoPropertyMapper : PropertyMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPropertyMapper"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        public UmbracoPropertyMapper(PropertyMapInfo info)
            : base(info)
        {
        }

        public UmbracoPropertyMapper(PropertyMapInfo info, IUmbMapperRegistry umbMapperRegistry, IUmbMapperService umbMapperService, IUmbracoContextFactory umbracoContextFactory)
            : base(info, umbMapperRegistry, umbMapperService, umbracoContextFactory)
        { }

        /// <inheritdoc/>
        public override object Map(IPublishedElement content, object value)
        {
            object convertedValue = this.CheckConvertType(value);

            if (this.Info.PropertyType.IsInstanceOfType(convertedValue))
            {
                return convertedValue;
            }

            return value ?? this.Info.DefaultValue;
        }
    }
}