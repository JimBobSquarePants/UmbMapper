// <copyright file="NamePickerPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using UmbMapper.PropertyMappers;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper.Umbraco8.Sample.ComponentModel.PropertyMappers
{
    /// <summary>
    /// Used to map names from document types
    /// </summary>
    public class NamePickerPropertyMapper : PropertyMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamePickerPropertyMapper"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        public NamePickerPropertyMapper(PropertyMapInfo info)
            : base(info)
        {
        }

        /// <inheritdoc/>
        public override object Map(IPublishedElement content, object value)
        {
            // Make sure you have Microsoft.Net.Compilers installed and up-to-date to use pattern matching
            switch (value)
            {
                case IPublishedContent publishedContent:
                    return publishedContent.Name;

                case IEnumerable<IPublishedContent> publishedContents:
                    return publishedContents.Select(x => x.Name);
            }

            return Enumerable.Empty<string>();
        }
    }
}