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
    public class PropertyMapInfo : IEquatable<PropertyMapInfo>, IPropertyMapInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapInfo"/> class.
        /// </summary>
        /// <param name="property">The property</param>
        public PropertyMapInfo(PropertyInfo property)
        {
            this.Property = property;
            this.PropertyType = property.PropertyType;
            this.ConstructorParams = this.PropertyType.GetConstructorParameters();
            this.IsEnumerableType = this.PropertyType.IsEnumerableType();
            this.EnumerableParamType = this.PropertyType.GetEnumerableType();
            this.IsConvertableEnumerableType = this.PropertyType.IsConvertableEnumerableType();
            this.IsCastableEnumerableType = this.PropertyType.IsCastableEnumerableType();
            this.IsEnumerableOfKeyValueType = this.PropertyType.IsEnumerableOfKeyValueType();
        }

        /// <inheritdoc/>
        public PropertyInfo Property { get; }

        /// <inheritdoc/>
        public Type PropertyType { get; }

        /// <inheritdoc/>
        public ParameterInfo[] ConstructorParams { get; private set; }

        /// <inheritdoc/>
        public Type EnumerableParamType { get; }

        /// <inheritdoc/>
        public bool IsEnumerableType { get; }

        /// <inheritdoc/>
        public bool IsConvertableEnumerableType { get; }

        /// <inheritdoc/>
        public bool IsCastableEnumerableType { get; }

        /// <inheritdoc/>
        public bool IsEnumerableOfKeyValueType { get; }

        /// <inheritdoc/>>
        public string[] Aliases { get; internal set; } = new string[0];

        /// <inheritdoc/>
        public bool Recursive { get; internal set; }

        /// <inheritdoc/>
        public bool Lazy { get; internal set; }

        /// <inheritdoc/>
        public bool HasPredicate { get; internal set; }

        /// <inheritdoc/>>
        public object DefaultValue { get; internal set; }

        /// <inheritdoc/>
        public CultureInfo Culture { get; internal set; }

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
                && this.ConstructorParams.SequenceEqual(other.ConstructorParams)
                && this.IsEnumerableType == other.IsEnumerableType
                && this.IsCastableEnumerableType == other.IsCastableEnumerableType
                && this.IsEnumerableOfKeyValueType == other.IsEnumerableOfKeyValueType
                && this.Aliases.SequenceEqual(other.Aliases)
                && this.Recursive == other.Recursive
                && this.Lazy == other.Lazy
                && this.HasPredicate == other.HasPredicate
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
                hashCode = (hashCode * 397) ^ info.ConstructorParams.GetHashCode();
                hashCode = (hashCode * 397) ^ (info.EnumerableParamType != null ? info.EnumerableParamType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ info.IsEnumerableType.GetHashCode();
                hashCode = (hashCode * 397) ^ info.IsCastableEnumerableType.GetHashCode();
                hashCode = (hashCode * 397) ^ info.IsEnumerableOfKeyValueType.GetHashCode();
                hashCode = (hashCode * 397) ^ (info.Aliases != null ? info.Aliases.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ info.Recursive.GetHashCode();
                hashCode = (hashCode * 397) ^ info.Lazy.GetHashCode();
                hashCode = (hashCode * 397) ^ info.HasPredicate.GetHashCode();
                hashCode = (hashCode * 397) ^ (info.DefaultValue != null ? info.DefaultValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (info.Culture != null ? info.Culture.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}