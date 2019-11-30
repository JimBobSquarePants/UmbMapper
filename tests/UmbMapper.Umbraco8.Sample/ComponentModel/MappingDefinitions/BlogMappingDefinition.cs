// <copyright file="BlogMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.Extensions;
using UmbMapper.Umbraco8.Sample.Models.Pages;

namespace UmbMapper.Umbraco8.Sample.ComponentModel.Mappers
{
    /// <summary>
    /// Configures mapping for Blog pages
    /// </summary>
    public class BlogMappingDefinition : PublishedPageMappingDefinition<Blog>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlogMappingDefinition"/> class.
        /// </summary>
        public BlogMappingDefinition()
        {
            // Map any additional properties that do not require specific configuration
            this.MapAll().ForEach(x => x.AsLazy());
        }
    }
}