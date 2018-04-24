// <copyright file="BlogMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.Sample.Models.Pages;

namespace UmbMapper.Sample.ComponentModel.Mappers
{
    /// <summary>
    /// Configures mapping for Blog pages
    /// </summary>
    public class BlogMap : PublishedPageMap<Blog>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlogMap"/> class.
        /// </summary>
        public BlogMap()
        {
            // Map any additional properties that do not require specific configuration
            this.MapAll().ForEach(x => x.AsLazy());
        }
    }
}