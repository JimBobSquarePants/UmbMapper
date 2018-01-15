// <copyright file="IPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Defines properties for mapping a property
    /// </summary>
    public interface IPropertyMapper : IPropertyMapInfo
    {
        /// <summary>
        /// Gets the Umbraco context
        /// </summary>
        UmbracoContext UmbracoContext { get; }

        /// <summary>
        /// Gets the MembershipHelper instance
        /// </summary>
        MembershipHelper Members { get; }

        /// <summary>
        /// Gets the UmbracoHelper instance
        /// </summary>
        UmbracoHelper Umbraco { get; }

        /// <summary>
        /// Maps the property from the given content
        /// </summary>
        /// <param name="content">The published content</param>
        /// <param name="value">The current value</param>
        /// <returns>The <see cref="object"/></returns>
        object Map(IPublishedContent content, object value);
    }
}