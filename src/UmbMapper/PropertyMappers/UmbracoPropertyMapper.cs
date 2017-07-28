// <copyright file="UmbracoPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Concurrent;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Maps directly from the published content
    /// </summary>
    public class UmbracoPropertyMapper : PropertyMapperBase
    {
        private static readonly ConcurrentDictionary<string, FastPropertyAccessor> ContentAccessorCache
            = new ConcurrentDictionary<string, FastPropertyAccessor>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPropertyMapper"/> class.
        /// </summary>
        /// <param name="config">The property configuration</param>
        public UmbracoPropertyMapper(PropertyMapperConfig config)
            : base(config)
        {
        }

        /// <inheritdoc/>
        public override object Map(IPublishedContent content, object value)
        {
            // First try class properties
            Type contentType = content.GetType();
            string key = contentType.AssemblyQualifiedName;

            if (key != null)
            {
                FastPropertyAccessor accessor;
                ContentAccessorCache.TryGetValue(key, out accessor);

                if (accessor == null)
                {
                    accessor = new FastPropertyAccessor(contentType);
                    ContentAccessorCache.TryAdd(key, accessor);
                }

                foreach (string name in this.Aliases)
                {
                    value = accessor.GetValue(name, content);
                    if (value != null && !value.Equals(this.DefaultValue))
                    {
                        break;
                    }
                }
            }

            // Then try custom properties
            if (value == null || value == this.DefaultValue)
            {
                foreach (string name in this.Aliases)
                {
                    value = content.GetPropertyValue(name, this.Recursive);
                    if (value != null)
                    {
                        break;
                    }
                }
            }

            return value ?? this.DefaultValue;
        }
    }
}