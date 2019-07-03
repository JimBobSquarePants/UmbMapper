// <copyright file="PublishedPage.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using UmbMapper.Umbraco8.Sample.Models.Media;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper.Umbraco8.Sample.Models.Pages
{
    /// <summary>
    /// The base class for all published pages
    /// </summary>
    public class PublishedPage : IPublishedEntity, IMeta, IRoutable
    {
        /// <inheritdoc/>
        public virtual int Id { get; set; }

        /// <inheritdoc/>
        public virtual string Name { get; set; }

        /// <inheritdoc/>
        public virtual string DocumentTypeAlias { get; set; }

        /// <inheritdoc/>
        public virtual int Level { get; set; }

        /// <inheritdoc/>
        public virtual int SortOrder { get; set; }

        /// <inheritdoc/>
        public virtual DateTime CreateDate { get; set; }

        /// <inheritdoc/>
        public virtual DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public virtual string BrowserWebsiteTitle { get; set; }

        /// <inheritdoc/>
        public virtual string BrowserPageTitle { get; set; }

        /// <inheritdoc/>
        public virtual bool? SwitchTitleOrder { get; set; }

        /// <inheritdoc/>
        public virtual string BrowserDescription { get; set; }

        /// <inheritdoc/>
        public virtual string OpenGraphTitle { get; set; }

        /// <inheritdoc/>
        public virtual PublishedImage OpenGraphImage { get; set; }

        /// <inheritdoc/>
        public virtual string AlternativeUrl { get; set; }

        /// <inheritdoc/>
        public virtual string AdditionalUrl { get; set; }

        /// <inheritdoc/>
        public virtual IPublishedContent TemporaryRedirect { get; set; }

        /// <inheritdoc/>
        public virtual IPublishedContent TransparentRedirect { get; set; }
    }
}