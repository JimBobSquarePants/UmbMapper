// <copyright file="PropertyMapInfo.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UmbMapper.Extensions;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Contains the propererties required for mapping an Umbraco property
    /// </summary>
    public class PropertyMapInfo : IEquatable<PropertyMapInfo>
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
            this.IsConvertableEnumerableType = this.PropertyType.IsConvertableEnumerableType();
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
        /// Gets a value indicating whether the specified type is an enumerable type that is safe to convert
        /// from <see cref="IEnumerable{T}"/> to a single item following processing via a mapper.
        /// </summary>
        public bool IsConvertableEnumerableType { get; }

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
        public string[] Aliases { get; internal set; } = new string[0];

        /// <summary>
        /// Gets a value indicating whether to map the property recursively up the tree
        /// </summary>
        public bool Recursive { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether to lazily map the property
        /// </summary>
        public bool Lazy { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the property is evaluated via a function
        /// </summary>
        public bool HasFunction { get; internal set; }

        /// <summary>
        /// Gets the default value
        /// </summary>
        public object DefaultValue { get; internal set; }

        /// <summary>
        /// Gets the culture
        /// </summary>
        public CultureInfo Culture { get; internal set; } = CultureInfo.InvariantCulture;

        /// <inheritdoc/>
        public bool Equals(PropertyMapInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Property.Name == other.Property.Name
                && this.PropertyType == other.PropertyType
                && this.IsEnumerableType == other.IsEnumerableType
                && this.IsCastableEnumerableType == other.IsCastableEnumerableType
                && this.IsEnumerableOfKeyValueType == other.IsEnumerableOfKeyValueType
                && this.Aliases.SequenceEqual(other.Aliases)
                && this.Recursive == other.Recursive
                && this.Lazy == other.Lazy
                && this.HasFunction == other.HasFunction
                && Equals(this.DefaultValue, other.DefaultValue)
                && this.Culture.Name == other.Culture.Name;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((PropertyMapInfo)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        private static int GetHashCode(PropertyMapInfo info)
        {
            unchecked
            {
                int hashCode = info.Property != null ? info.Property.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (info.PropertyType != null ? info.PropertyType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ info.IsEnumerableType.GetHashCode();
                hashCode = (hashCode * 397) ^ info.IsCastableEnumerableType.GetHashCode();
                hashCode = (hashCode * 397) ^ info.IsEnumerableOfKeyValueType.GetHashCode();
                hashCode = (hashCode * 397) ^ (info.Aliases != null ? info.Aliases.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ info.Recursive.GetHashCode();
                hashCode = (hashCode * 397) ^ info.Lazy.GetHashCode();
                hashCode = (hashCode * 397) ^ info.HasFunction.GetHashCode();
                hashCode = (hashCode * 397) ^ (info.DefaultValue != null ? info.DefaultValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (info.Culture != null ? info.Culture.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}