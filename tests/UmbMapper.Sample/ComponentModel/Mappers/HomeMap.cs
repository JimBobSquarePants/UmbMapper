// <copyright file="HomeMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.PropertyMappers;
using UmbMapper.PropertyMappers.NuPickers;
using UmbMapper.Sample.Models.Pages;

namespace UmbMapper.Sample.ComponentModel.Mappers
{
    /// <summary>
    /// Configures mapping for the home page
    /// </summary>
    public class HomeMap : MapperConfig<Home>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeMap"/> class.
        /// </summary>
        public HomeMap()
        {
            // Inherited properties
            this.AddMap(x => x.Id).AsLazy();
            this.AddMap(x => x.Name).AsLazy();
            this.AddMap(x => x.DocumentTypeAlias).AsLazy();
            this.AddMap(x => x.Level).AsLazy();
            this.AddMap(x => x.SortOrder).AsLazy();
            this.AddMap(x => x.CreateDate).AsLazy();
            this.AddMap(x => x.UpdateDate).AsLazy();
            this.AddMap(x => x.BrowserWebsiteTitle).AsLazy();
            this.AddMap(x => x.BrowserPageTitle).SetAlias(x => x.BrowserPageTitle, x => x.Name).AsLazy();
            this.AddMap(x => x.SwitchTitleOrder).AsLazy();
            this.AddMap(x => x.BrowserDescription).AsLazy();
            this.AddMap(x => x.OpenGraphTitle).AsLazy();
            this.AddMap(x => x.OpenGraphType).SetMapper<NuPickerEnumPropertyMapper>().AsLazy();
            this.AddMap(x => x.OpenGraphImage).SetMapper<UmbracoPickerPropertyMapper>().AsRecursive().AsLazy();

            // Home properties
            this.AddMap(x => x.BodyText).AsLazy();
        }
    }
}