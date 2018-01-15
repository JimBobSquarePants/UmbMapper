// <copyright file="PropertyMapInfo.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
        /// The cache for storing created default types.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, object> TypeDefaultsCache = new ConcurrentDictionary<Type, object>();

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
            this.Aliases = new[] { this.Property.Name.ToUpperInvariant() };
            this.DefaultValue = GetDefaultValue(this.PropertyType);
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
        public string[] Aliases { get; internal set; }

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
            if (other is null)
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
            if (obj is null)
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

        /// <summary>
        /// Returns the default value for the given type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to return.</param>
        /// <returns>The <see cref="object"/> representing the default value.</returns>
        private static object GetDefaultValue(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return TypeDefaultsCache.GetOrAdd(type, t =>
            {
                // We want a Func<object> which returns the default value.
                // Create that expression, convert to object.
                // The default value, will always be what the runtime tells us.
                var e = Expression.Lambda<Func<object>>(Expression.Convert(Expression.Default(t), typeof(object)));

                return e.Compile()();
            });
        }
    }
}