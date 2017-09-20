// <copyright file="UmbracoPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using UmbMapper.Extensions;
using Umbraco.Core;
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
        /// <param name="info">The property map information</param>
        public UmbracoPropertyMapper(PropertyMapInfo info)
            : base(info)
        {
        }

        /// <inheritdoc/>
        public override object Map(IPublishedContent content, object value)
        {
            // First try custom properties
            foreach (string name in this.Aliases)
            {
                value = this.CheckConvertType(content.GetPropertyValue(name, this.Recursive));
                if (!value.IsNullOrEmptyString())
                {
                    if (this.PropertyType.IsInstanceOfType(value))
                    {
                        break;
                    }
                }
            }

            // Then try class properties
            if (value.IsNullOrEmptyString() || value == this.DefaultValue)
            {
                Type contentType = content.GetType();
                string key = contentType.AssemblyQualifiedName;

                if (key != null)
                {
                    ContentAccessorCache.TryGetValue(key, out var accessor);

                    if (accessor == null)
                    {
                        accessor = new FastPropertyAccessor(contentType);
                        ContentAccessorCache.TryAdd(key, accessor);
                    }

                    foreach (string name in this.Aliases)
                    {
                        value = this.CheckConvertType(accessor.GetValue(name, content));
                        if (!value.IsNullOrEmptyString() && !value.Equals(this.DefaultValue))
                        {
                            break;
                        }
                    }
                }
            }

            return value ?? this.DefaultValue;
        }

        /// <summary>
        /// Checks the value to see if it is an instance of the given type and attempts to
        /// convert the value to the correct type if it is not.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="object"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object CheckConvertType(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (this.PropertyType.IsInstanceOfType(value))
            {
                return value;
            }

            try
            {
                Attempt<object> attempt = value.TryConvertTo(this.PropertyType);
                if (attempt.Success)
                {
                    return attempt.Result;
                }
            }
            catch
            {
                return value;
            }

            return value;
        }
    }
}