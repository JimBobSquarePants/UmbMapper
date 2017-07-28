// <copyright file="PropertyMapperConfig.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Globalization;
using System.Reflection;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Contains the propererties required for mapping an Umbraco property
    /// </summary>
    public class PropertyMapperConfig
    {
        /// <summary>
        /// Gets or sets the property
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// Gets or sets the property type
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// Gets or sets the property aliases
        /// </summary>
        public string[] Aliases { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to map the property recursively up the tree
        /// </summary>
        public bool Recursive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to lazily map the property
        /// </summary>
        public bool Lazy { get; set; }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the culture
        /// </summary>
        public CultureInfo Culture { get; set; }
    }
}