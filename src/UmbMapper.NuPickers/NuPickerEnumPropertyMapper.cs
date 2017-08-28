// <copyright file="NuPickerEnumPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using nuPickers;
using Umbraco.Core.Models;

namespace UmbMapper.PropertyMappers.NuPickers
{
    /// <summary>
    /// Maps from a NuPicker value to an enum
    /// </summary>
    public class NuPickerEnumPropertyMapper : EnumPropertyMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuPickerEnumPropertyMapper"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        public NuPickerEnumPropertyMapper(PropertyMapInfo info)
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