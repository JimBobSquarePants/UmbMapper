// <copyright file="PublishedPageMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.Extensions;
using UmbMapper.Models;
using UmbMapper.Umbraco8.Sample.Models.Pages;

namespace UmbMapper.Umbraco8.Sample.ComponentModel.Mappers
{
    /// <summary>
    /// Configures mapping for basic page properties
    /// </summary>
    /// <typeparam name="T">The type of object to map.</typeparam>
    public class PublishedPageMappingDefinition<T> : MappingDefinition<T>// UmbMapperConfig<T>
        where T : PublishedPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublishedPageMappingDefinition{T}"/> class.
        /// </summary>
        public PublishedPageMappingDefinition()
        {
            // Map all items as lazy - This means we only map what properties you explicitly ask for in your calling code.
            this.MapAll().ForEach(x => x.AsLazy());

            // Update specific mappings with extra configuration
            // this.AddMap(x => x.BrowserPageTitle).SetAlias(x => x.BrowserPageTitle, x => x.Name).SetMapper<VortoPropertyMapper>();
            this.AddMappingDefinition(x => x.OpenGraphImage).AsRecursive();
        }
    }
}