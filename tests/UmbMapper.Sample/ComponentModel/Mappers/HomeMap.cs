// <copyright file="HomeMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Globalization;
using UmbMapper.PropertyMappers.Archetype;
using UmbMapper.PropertyMappers.Vorto;
using UmbMapper.Sample.ComponentModel.PropertyMappers;
using UmbMapper.Sample.Models.Pages;

namespace UmbMapper.Sample.ComponentModel.Mappers
{
    /// <summary>
    /// Configures mapping for the home page
    /// </summary>
    public class HomeMap : PublishedPageMap<Home>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeMap"/> class.
        /// </summary>
        public HomeMap()
        {
            // Map all items as lazy
            this.MapAll().ForEach(x => x.AsLazy());

            // Use a custom mapper to grab just the names.
            // If you use the Posts property itself and grab the name you don't need this but this would
            // be faster if you don't need the other properties of the targeted document types.
            // I'm only using the alias so I can demonstrate both approaches with a single property
            this.AddMap(x => x.PostNames).SetAlias(x => x.Posts).SetMapper<NamePickerPropertyMapper>();

            // Vorto requires a custom mapper to call its API.
            // In this example we're falling back to the main body text if no Vorto value matching the
            // current culture is found.
            this.AddMap(x => x.VortoBodyText).SetAlias(x => x.VortoBodyText, x => x.BodyText).SetMapper<VortoPropertyMapper>();

            // In this example we're using the property alias from the doctype but explicity setting a known culture.
            this.AddMap(x => x.VortoBodyTextFr).SetAlias(x => x.VortoBodyText).SetCulture(new CultureInfo("fr-FR")).SetMapper<VortoPropertyMapper>();

            // Only Archetype requires additional configuration, Nested Content just works!!
            this.AddMap(x => x.ArchetypeGallery).SetMapper<ArchetypeFactoryPropertyMapper>();
        }
    }
}