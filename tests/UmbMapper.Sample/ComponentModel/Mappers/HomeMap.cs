// <copyright file="HomeMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.PropertyMappers;
using UmbMapper.PropertyMappers.Archetype;
using UmbMapper.PropertyMappers.NuPickers;
using UmbMapper.Sample.Models.Pages;

namespace UmbMapper.Sample.ComponentModel.Mappers
{
    /// <summary>
    /// Configures mapping for the home page
    /// </summary>
    public class HomeMap : UmbMapperConfig<Home>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeMap"/> class.
        /// </summary>
        public HomeMap()
        {
            // Inherited properties
            this.AddMappings(
                x => x.Id,
                x => x.Name,
                x => x.DocumentTypeAlias,
                x => x.Level).ForEachIndexed((x, i) => x.AsLazy());

            this.AddMappings(
                x => x.SortOrder,
                x => x.CreateDate,
                x => x.UpdateDate,
                x => x.BrowserWebsiteTitle).ForEach(x => x.AsLazy());

            this.AddMap(x => x.BrowserPageTitle).SetAlias(x => x.BrowserPageTitle, x => x.Name).AsLazy();
            this.AddMap(x => x.SwitchTitleOrder).AsLazy();
            this.AddMap(x => x.BrowserDescription).AsLazy();
            this.AddMap(x => x.OpenGraphTitle).AsLazy();
            this.AddMap(x => x.OpenGraphType).SetMapper<NuPickerEnumPropertyMapper>().AsLazy();
            this.AddMap(x => x.OpenGraphImage).SetMapper<UmbracoPickerPropertyMapper>().AsRecursive().AsLazy();

            // Home properties
            this.AddMap(x => x.BodyText).AsLazy();
            this.AddMap(x => x.Gallery).SetMapper<ArchetypeFactoryPropertyMapper>().AsLazy();
        }
    }
}