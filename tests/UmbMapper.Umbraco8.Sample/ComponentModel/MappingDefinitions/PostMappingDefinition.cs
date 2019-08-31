// <copyright file="PostMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.Extensions;
using UmbMapper.Umbraco8.Sample.Models.Pages;

namespace UmbMapper.Umbraco8.Sample.ComponentModel.Mappers
{
    /// <summary>
    /// Configures mapping for post pages
    /// </summary>
    public class PostMappingDefinition : PublishedPageMappingDefinition<Post>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostMap"/> class.
        /// </summary>
        public PostMappingDefinition()
        {
            // Map any additional properties that do not require specific configuration
            this.MapAll().ForEach(x => x.AsLazy());
        }
    }
}