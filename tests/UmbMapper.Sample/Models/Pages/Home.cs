// <copyright file="Home.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Collections.Generic;
using System.Web;
using UmbMapper.Sample.Models.Components;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper.Sample.Models.Pages
{
    /// <summary>
    /// Represents the home page document type
    /// </summary>
    public class Home : PublishedPage
    {
        public Home(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets or sets the main body copy
        /// </summary>
        public virtual IHtmlString BodyText { get; set; }

        /// <summary>
        /// Gets or sets the image gallery
        /// </summary>
        public virtual IEnumerable<Slide> ArchetypeGallery { get; set; }

        /// <summary>
        /// Gets or sets the image gallery
        /// </summary>
        public virtual IEnumerable<Slide> Gallery { get; set; }
    }
}