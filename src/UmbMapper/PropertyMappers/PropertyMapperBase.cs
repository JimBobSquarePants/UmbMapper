// <copyright file="PropertyMapperBase.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
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
        }

        /// <inheritdoc/>
        public PropertyMapInfo Info { get; }

        /// <inheritdoc/>
        public UmbracoContext UmbracoContext => this.GetUmbracoContext();

        /// <inheritdoc/>
        public MembershipHelper Members => new MembershipHelper(this.UmbracoContext);

        /// <inheritdoc/>
        public UmbracoHelper Umbraco => new UmbracoHelper(this.UmbracoContext);

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

            if (this.UmbracoContext?.PublishedContentRequest != null)
            {
                return this.UmbracoContext.PublishedContentRequest.Culture;
            }

            return CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Checks the value to see if it is an instance of the given type and attempts to
        /// convert the value to the correct type if it is not.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="object"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected object CheckConvertType(object value)
        {
            Type propType = this.Info.PropertyType;
            if (value == null || propType.IsInstanceOfType(value))
            {
                return value;
            }

            try
            {
                Attempt<object> attempt = value.TryConvertTo(propType);
                if (attempt.Success)
                {
                    return attempt.Result;
                }
            }
            catch
            {
                return value;
            }

            // Special case for IHtmlString to remove Html.Raw requirement
            if (value is string && propType == typeof(IHtmlString))
            {
                value = new HtmlString(value.ToString());
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private UmbracoContext GetUmbracoContext()
        {
            return UmbracoContext.Current ?? throw new InvalidOperationException("UmbracoContext.Current is null.");
        }
    }
}