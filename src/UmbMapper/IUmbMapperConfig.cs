// <copyright file="IUmbMapperConfig.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace UmbMapper
{
    /// <summary>
    /// Defines the base properties required for a mapper configuration
    /// </summary>
    public interface IUmbMapperConfig
    {
        /// <summary>
        /// Gets the mapped type
        /// </summary>
        Type MappedType { get; }

        bool CreateProxy { get; }
        Type ProxyType { get; }
        bool HasIPublishedConstructor { get; }

        /// <summary>
        /// Gets the collection of mappings registered with the mapper
        /// </summary>
        IEnumerable<IPropertyMap> Mappings { get; }

        /// <summary>
        /// Performs the mapping operation creating a new destination object
        /// </summary>
        /// <param name="content">The published content</param>
        /// <returns>The <see cref="object"/></returns>
        object Map(IPublishedContent content);

        /// <summary>
        /// Performs the mapping operation onto an existing destination object
        /// </summary>
        /// <param name="content">The published content</param>
        /// <param name="destination">The destination object</param>
        void Map(IPublishedContent content, object destination);

        /// <summary>
        /// Creates an empty instance of the mapped type.
        /// </summary>
        /// <returns>The <see cref="object"/></returns>
        object CreateEmpty();

        /// <summary>
        /// Creates an empty instance of the mapped type.
        /// </summary>
        /// <param name="content">The published content</param>
        /// <returns>The <see cref="object"/></returns>
        object CreateEmpty(IPublishedContent content);

        /// <summary>
        /// Runs any additional code required to setup the configuration
        /// </summary>
        void Init(IUmbracoContextFactory umbracoContextFactory);
    }
}