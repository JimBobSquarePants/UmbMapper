﻿// <copyright file="PublishedImageMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.Extensions;
using UmbMapper.Models;
using UmbMapper.Umbraco8.Sample.Models.Media;
using Umbraco.Core;

namespace UmbMapper.Umbraco8.Sample.ComponentModel.Mappers
{
    /// <summary>
    /// Configures mapping of published images
    /// </summary>
    public class PublishedImageMappingDefinition : MappingDefinition<PublishedImage> //UmbMapperConfig<PublishedImage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublishedImageMap"/> class.
        /// </summary>
        public PublishedImageMappingDefinition()
        {
            this.MapAll().ForEach(x => x.AsLazy());

            // Additional configuration to set the alias of certain properties.
            // Umbraco prefixes those special properties with "umbraco"
            this.AddMappingDefinition(x => x.FileName).SetAlias(Constants.Conventions.Media.File).AsLazy();
            this.AddMappingDefinition(x => x.Bytes).SetAlias(Constants.Conventions.Media.Bytes).AsLazy();
            this.AddMappingDefinition(x => x.Extension).SetAlias(Constants.Conventions.Media.Extension).AsLazy();
            this.AddMappingDefinition(x => x.Width).SetAlias(Constants.Conventions.Media.Width).AsLazy();
            this.AddMappingDefinition (x => x.Height).SetAlias(Constants.Conventions.Media.Height).AsLazy();
            this.AddMappingDefinition (x => x.Crops).SetAlias(Constants.Conventions.Media.File).AsLazy();
        }
    }
}