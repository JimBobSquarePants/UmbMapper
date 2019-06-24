// <copyright file="PropertyMapperBase.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using UmbMapper.Extensions;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// The base class for all property mappers
    /// </summary>
    public abstract class PropertyMapperBase : IPropertyMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapperBase"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        protected PropertyMapperBase(PropertyMapInfo info)
        {
            this.Info = info;
            this.Alias = info.Aliases[0];
        }

        /// <inheritdoc/>
        public PropertyMapInfo Info { get; }

        /// <inheritdoc/>
        public UmbracoContext UmbracoContext => this.GetUmbracoContext();

        /// <summary>
        /// Gets the current alias.
        /// </summary>
        protected string Alias { get; private set; }

        /// <inheritdoc/>
        public object GetRawValue(IPublishedContent content)
        {
            return this.GetRawValue(content, this.Info.Aliases);
        }

        /// <inheritdoc/>
        public abstract object Map(IPublishedContent content, object value);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CultureInfo GetRequestCulture()
        {
            CultureInfo culture = this.Info.Culture;
            if (culture != null)
            {
                return culture;
            }

            if (this.UmbracoContext?.PublishedRequest != null)
            {
                return this.UmbracoContext.PublishedRequest.Culture;
            }

            return CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Maps the raw property from the given content without conversion.
        /// </summary>
        /// <param name="content">The published content</param>
        /// <param name="aliases">The collection of alias to check against.</param>
        /// <returns>The <see cref="object"/></returns>
        protected object GetRawValue(IPublishedContent content, string[] aliases)
        {
            PropertyMapInfo info = this.Info;
            object value = info.DefaultValue;

            // First try custom properties
            for (int i = 0; i < aliases.Length; i++)
            {
                string alias = aliases[i];

                // Fallback updated to boolean to enum
                Fallback fallback =
                    info.Recursive
                    ? Fallback.ToAncestors
                    : Fallback.ToDefaultValue;

                value = content.Value(alias, null, null, fallback, null);
                if (!this.IsNullOrDefault(value))
                {
                    this.Alias = alias;
                    return value;
                }
            }

            // Then try class properties
            Type contentType = content.GetType();
            string key = contentType.AssemblyQualifiedName;

            if (key != null)
            {
                UmbMapperRegistry.ContentAccessorCache.TryGetValue(key, out FastPropertyAccessor accessor);
                if (accessor is null)
                {
                    accessor = new FastPropertyAccessor(contentType);
                    UmbMapperRegistry.ContentAccessorCache.TryAdd(key, accessor);
                }

                for (int i = 0; i < aliases.Length; i++)
                {
                    string alias = aliases[i];
                    value = accessor.GetValue(alias, content);
                    if (!this.IsNullOrDefault(value))
                    {
                        this.Alias = alias;
                        return value;
                    }
                }
            }

            return value ?? info.DefaultValue;
        }

        /// <summary>
        /// Checks the value to see if it is an instance of the given type and attempts to
        /// convert the value to the correct type if it is not.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>The <see cref="object"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected object CheckConvertType(object value)
        {
            Type propType = this.Info.PropertyType;
            if (value is null || propType.IsInstanceOfType(value))
            {
                return value;
            }

            Attempt<object> attempt = value.UmbMapperTryConvertTo(propType);
            if (attempt.Success)
            {
                return attempt.Result;
            }

            // Special case for IHtmlString to remove Html.Raw requirement
            if (value is string && propType == typeof(IHtmlString))
            {
                value = new HtmlString(value.ToString());
            }

            return value;
        }

        /// <summary>
        /// Returns a value indicating whether the given object is null of it's default value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>The <see cref="bool"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool IsNullOrDefault(object value)
        {
            return value.IsNullOrEmptyString() || value.Equals(this.Info.DefaultValue);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private UmbracoContext GetUmbracoContext()
        {
            return Umbraco.Web.Composing.Current.UmbracoContext ?? throw new InvalidOperationException("UmbracoContext.Current is null."); ;
        }
    }
}