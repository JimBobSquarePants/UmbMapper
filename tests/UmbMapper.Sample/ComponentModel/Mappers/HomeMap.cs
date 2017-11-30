// <copyright file="HomeMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.PropertyMappers.Archetype;
using UmbMapper.Sample.ComponentModel.PropertyMappers;
using UmbMapper.Sample.Models.Pages;

namespace UmbMapper.Sample.ComponentModel.Mappers
{
    /// <summary>
    /// Configures mapping for the home page
    /// </summary>
    public class HomeMap : PageMap<Home>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeMap"/> class.
        /// </summary>
        public HomeMap()
        {
            // Map all items as lazy
            this.MapAll().ForEach(x => x.AsLazy());

            this.AddMap(x => x.VortoBodyText).SetMapper<VortoPropertyMapper>();

            // Only Archetype requires additional configuration, Nested Content just works!!
            this.AddMap(x => x.ArchetypeGallery).SetMapper<ArchetypeFactoryPropertyMapper>();
        }
    }
}