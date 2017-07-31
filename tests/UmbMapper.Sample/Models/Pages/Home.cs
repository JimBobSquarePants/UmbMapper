// <copyright file="Home.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Web;

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
    }
}