// <copyright file="IPropertyMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.PropertyMappers;

namespace UmbMapper
{
    /// <summary>
    /// Defines the contract for property maps
    /// </summary>
    public interface IPropertyMap
    {
        /// <summary>
        /// Gets the mapping property information
        /// </summary>
        PropertyMapInfo Info { get; }

        /// <summary>
        /// Gets or the property mapper
        /// </summary>
        IPropertyMapper PropertyMapper { get; }
    }
}