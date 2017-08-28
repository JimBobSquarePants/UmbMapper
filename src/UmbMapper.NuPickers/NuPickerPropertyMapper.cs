// <copyright file="NuPickerPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using nuPickers;
using Umbraco.Core.Models;

namespace UmbMapper.PropertyMappers.NuPickers
{
    /// <summary>
    /// Maps NuPicker properties in the backoffice
    /// </summary>
    public class NuPickerPropertyMapper : UmbracoPickerPropertyMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuPickerPropertyMapper"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        public NuPickerPropertyMapper(PropertyMapInfo info)
            : base(info)
        {
        }

        /// <inheritdoc/>
        public override object Map(IPublishedContent content, object value)
        {
            value = value is Picker picker ? picker.SavedValue : value;

            return base.Map(content, value);
        }
    }
}