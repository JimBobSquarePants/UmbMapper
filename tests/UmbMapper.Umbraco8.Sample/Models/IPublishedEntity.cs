// <copyright file="IPublishedEntity.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;

namespace UmbMapper.Umbraco8.Sample.Models
{
    /// <summary>
    /// Defines the contract for a content node defining the basic properties required to identify the object from the Umbraco back office.
    /// We use this so that we do not have to implement all the <see cref="Umbraco.Core.Models.IPublishedContent"/> properties in our implementations, only the essentials
    /// </summary>
    public interface IPublishedEntity
    {
        /// <summary>
        /// Gets or sets the Umbraco Id for this content, media, or member.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the Umbraco node name for this content, media, or member.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the document type alias for this content, or media.
        /// </summary>
        string DocumentTypeAlias { get; set; }

        /// <summary>
        /// Gets or sets the Umbraco node level for this content, or media.
        /// </summary>
        int Level { get; set; }

        /// <summary>
        /// Gets or sets the Umbraco node sort order for this content, or media.
        /// </summary>
        int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets  the Umbraco node created date for this content, media, or member.
        /// </summary>
        DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets  the Umbraco node updated date for this content, media, or member.
        /// </summary>
        DateTime UpdateDate { get; set; }
    }
}