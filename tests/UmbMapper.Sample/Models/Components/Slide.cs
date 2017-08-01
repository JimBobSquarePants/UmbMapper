// <copyright file="Slide.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.Sample.Models.Media;

namespace UmbMapper.Sample.Models.Components
{
    /// <summary>
    /// Represents a single gallery slide
    /// </summary>
    public class Slide
    {
        /// <summary>
        /// Gets or sets the image
        /// </summary>
        public virtual PublishedImage Image { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public virtual string Description { get; set; }
    }
}