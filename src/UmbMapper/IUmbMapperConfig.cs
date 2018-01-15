// <copyright file="IUmbMapperConfig.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using Umbraco.Core.Models;

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

        /// <summary>
        /// Gets the collection of mappings registered with the mapper
        /// </summary>
        IEnumerable<IPropertyMap> Mappings { get; }

        /// <summary>
        /// Performs the mapping operation
        /// </summary>
        /// <param name="content">The published content</param>
        /// <returns>The <see cref="object"/></returns>
        object Map(IPublishedContent content);

        /// <summary>
        /// Runs any additional code required to setup the configuration
        /// </summary>
        void Init();
    }
}