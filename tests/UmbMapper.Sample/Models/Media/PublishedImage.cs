// <copyright file="PublishedImage.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.ComponentModel;
using Umbraco.Web.Models;

namespace UmbMapper.Sample.Models.Media
{
    /// <summary>
    /// Represents a single published image in the media section.
    /// </summary>
    public class PublishedImage : IPublishedEntity, IPublishedFile
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
        public virtual string FileName { get; set; }

        /// <inheritdoc/>
        public virtual int Bytes { get; set; }

        /// <inheritdoc/>
        public virtual string Extension { get; set; }

        /// <summary>
        /// Gets or sets the width in pixels.
        /// </summary>
        public virtual int Width { get; set; }

        /// <summary>
        /// Gets or sets the height in pixels.
        /// </summary>
        public virtual int Height { get; set; }

        /// <summary>
        /// Gets or sets the crops.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ImageCropDataSet Crops { get; set; }
    }
}