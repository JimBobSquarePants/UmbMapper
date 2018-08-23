// <copyright file="PublishedPageMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.PropertyMappers.NuPickers;
using UmbMapper.PropertyMappers.Vorto;
using UmbMapper.Sample.Models.Pages;

namespace UmbMapper.Sample.ComponentModel.Mappers
{
    /// <summary>
    /// Configures mapping for basic page properties
    /// </summary>
    /// <typeparam name="T">The type of object to map.</typeparam>
    public class PublishedPageMap<T> : UmbMapperConfig<T>
        where T : PublishedPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublishedPageMap{T}"/> class.
        /// </summary>
        public PublishedPageMap()
        {
            // Map all items as lazy - This means we only map what properties you explicitly ask for in your calling code.
            this.MapAll().ForEach(x => x.AsLazy());

            // Update specific mappings with extra configuration
            this.AddMap(x => x.BrowserPageTitle).SetAlias(x => x.BrowserPageTitle, x => x.Name).SetMapper<VortoPropertyMapper>();
            this.AddMap(x => x.OpenGraphType).SetMapper<NuPickerEnumPropertyMapper>();
            this.AddMap(x => x.OpenGraphImage).AsRecursive();
        }
    }
}