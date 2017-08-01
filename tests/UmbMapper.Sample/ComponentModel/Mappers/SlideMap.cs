// <copyright file="SlideMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.PropertyMappers;
using UmbMapper.Sample.Models.Components;

namespace UmbMapper.Sample.ComponentModel.Mappers
{
    /// <summary>
    /// Configures mapping for the gallery slides page
    /// </summary>
    public class SlideMap : MapperConfig<Slide>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlideMap"/> class.
        /// </summary>
        public SlideMap()
        {
            this.AddMap(x => x.Image).SetMapper<UmbracoPickerPropertyMapper>().AsLazy();
            this.AddMap(x => x.Description).AsLazy();
        }
    }
}