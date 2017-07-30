// <copyright file="PropertyMapInfo.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Globalization;
using System.Reflection;
using UmbMapper.Extensions;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Contains the propererties required for mapping an Umbraco property
    /// </summary>
    public class PropertyMapInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapInfo"/> class.
        /// </summary>
        /// <param name="property">The property</param>
        public PropertyMapInfo(PropertyInfo property)
        {
            this.Property = property;
            this.PropertyType = property.PropertyType;
            this.IsEnumerableType = this.PropertyType.IsEnumerableType();
            this.IsCastableEnumerableType = this.PropertyType.IsCastableEnumerableType();
            this.IsEnumerableOfKeyValueType = this.PropertyType.IsEnumerableOfKeyValueType();
        }

        /// <summary>
        /// Gets the property
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets the property type
        /// </summary>
        public Type PropertyType { get; }

        /// <summary>
        /// Gets a value indicating whether the specified type is an enumerable type.
        /// </summary>
        public bool IsEnumerableType { get; }

        /// <summary>
        /// Gets a value indicating whether the specified type is an enumerable type that is safe to cast
        /// following processing via a type converter
        /// </summary>
        public bool IsCastableEnumerableType { get; }

        /// <summary>
        /// Gets a value indicating whether the specified type is an enumerable type containing a
        /// key value pair as the generic type parameter.
        /// </summary>
        public bool IsEnumerableOfKeyValueType { get; }

        /// <summary>
        /// Gets the property aliases
        /// </summary>
        public string[] Aliases { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether to map the property recursively up the tree
        /// </summary>
        public bool Recursive { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether to lazily map the property
        /// </summary>
        public bool Lazy { get; internal set; }

        /// <summary>
        /// Gets the default value
        /// </summary>
        public object DefaultValue { get; internal set; }

        /// <summary>
        /// Gets the culture
        /// </summary>
        public CultureInfo Culture { get; internal set; }
    }
}