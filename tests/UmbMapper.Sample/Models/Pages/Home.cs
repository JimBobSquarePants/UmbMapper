// <copyright file="Home.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Collections.Generic;
using System.Web;
using UmbMapper.Sample.ComponentModel.Mappers;
using UmbMapper.Sample.Models.Components;

namespace UmbMapper.Sample.Models.Pages
{
    /// <summary>
    /// Represents the home page document type
    /// </summary>
    public class Home : PublishedPage
    {
        /// <summary>
        /// Gets or sets the main body copy
        /// </summary>
        public virtual IHtmlString BodyText { get; set; }

        /// <summary>
        /// Gets or sets the main body copy
        /// </summary>
        public virtual IHtmlString VortoBodyText { get; set; }

        /// <summary>
        /// Gets or sets the image gallery
        /// </summary>
        public virtual IEnumerable<Slide> ArchetypeGallery { get; set; }

        /// <summary>
        /// Gets or sets the image gallery
        /// </summary>
        public virtual IEnumerable<Slide> Gallery { get; set; }

        /// <summary>
        /// Gets or sets the post names.
        /// In <see cref="HomeMap"/> you'll see that we map this using the <see cref="Posts"/> property.
        /// I'm doing this so I can demonstrate mapping both individual properties and complete classes.
        /// </summary>
        public virtual IEnumerable<string> PostNames { get; set; }

        /// <summary>
        /// Gets or sets the posts
        /// </summary>
        public virtual IEnumerable<Post> Posts { get; set; }
    }
}