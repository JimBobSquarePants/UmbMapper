// <copyright file="PropertyMap{T}.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UmbMapper.Extensions;
using UmbMapper.PropertyMappers;
using Umbraco.Core.Models;

namespace UmbMapper
{
    /// <summary>
    /// Provides the mechanism for mapping a property from the Umbraco published content
    /// </summary>
    /// <typeparam name="T">The type of object to map</typeparam>
    public class PropertyMap<T> : IPropertyMap, IEquatable<PropertyMap<T>>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMap{T}"/> class.
        /// </summary>
        /// <param name="property">The property to map</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="property"/> is not writable.</exception>
        public PropertyMap(PropertyInfo property)
        {
            if (!property.CanWrite)
            {
                throw new InvalidOperationException($"Property {property} in class {typeof(T).Name} must be writable in order to be mapped");
            }

            this.Info = new PropertyMapInfo(property);
            this.PropertyMapper = new UmbracoPropertyMapper(this.Info);
        }

        /// <inheritdoc/>
        public PropertyMapInfo Info { get; }

        /// <inheritdoc/>
        public IPropertyMapper PropertyMapper { get; set; }

        /// <summary>
        /// Gets the mapping predicate. Used for mapping from known values in the current instance.
        /// </summary>
        public Func<T, IPublishedContent, object> Predicate { get; internal set; }

        /// <summary>
        /// Sets the aliases to check against when mapping the property
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to map.</typeparam>
        /// <param name="aliases">The collection of alias identifiers to map from; case insensitive</param>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> SetAlias<TProperty>(params Expression<Func<T, TProperty>>[] aliases)
        {
            if (aliases == null)
            {
                return this;
            }

            string[] altNames = new string[aliases.Length];
            for (int i = 0; i < aliases.Length; i++)
            {
                Expression<Func<T, TProperty>> expression = aliases[i];

                // The property access might be getting converted to object to match the func
                // If so, get the operand and see if that's a member expression
                MemberExpression member = expression.Body as MemberExpression
                                          ?? (expression.Body as UnaryExpression)?.Operand as MemberExpression;

                altNames[i] = member?.Member.Name.ToUpperInvariant() ?? throw new ArgumentException("Action must be a member expression.");
            }

            this.Info.Aliases = altNames;
            return this;
        }

        /// <summary>
        /// Sets the aliases to check against when mapping the property
        /// </summary>
        /// <param name="aliases">The collection of alias identifiers to map from; case insensitive</param>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> SetAlias(params string[] aliases)
        {
            if (aliases == null || !aliases.Any())
            {
                return this;
            }

            this.Info.Aliases = aliases.Select(x => x.ToUpperInvariant()).ToArray();
            return this;
        }

        /// <summary>
        /// Sets the property mapper for the property
        /// </summary>
        /// <typeparam name="TMapper">The type of property mapper</typeparam>
        /// <returns>The <see cref="IPropertyMapper"/></returns>
        public TMapper SetMapper<TMapper>()
            where TMapper : IPropertyMapper
        {
            this.PropertyMapper = (TMapper)typeof(TMapper).GetInstance(this.Info);
            return (TMapper)this.PropertyMapper;
        }

        /// <summary>
        /// Sets the property mapping predicate. Used for mapping from known values in the current instance.
        /// </summary>
        /// <param name="predicate">The mapping predicate</param>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> MapFromInstance(Func<T, IPublishedContent, object> predicate)
        {
            this.Info.HasPredicate = true;
            this.Predicate = predicate;
            return this;
        }

        /// <summary>
        /// Sets the culture to use when mapping the property
        /// </summary>
        /// <param name="culture">The specific culture</param>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> SetCulture(CultureInfo culture)
        {
            this.Info.Culture = culture;
            return this;
        }

        /// <summary>
        /// Instructs the mapper to map the property recursively up the tree
        /// </summary>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> AsRecursive()
        {
            this.Info.Recursive = true;
            return this;
        }

        /// <summary>
        /// Instructs the mapper to lazily map the property
        /// </summary>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        /// <exception cref="InvalidOperationException">Thrown if the property is not marked as <code>virtual</code></exception>
        public PropertyMap<T> AsLazy()
        {
            if (!this.Info.Property.ShouldAttemptLazyLoad())
            {
                throw new InvalidOperationException($"Property {this.Info.Property.Name} in class {typeof(T).Name} must be marked with the 'virtual' keyword to be lazily mapped.");
            }

            this.Info.Lazy = true;
            return this;
        }

        /// <summary>
        /// Sets the default value for the mapper
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> DefaultValue(object value)
        {
            this.Info.DefaultValue = value;
            return this;
        }

        /// <inheritdoc/>
        public bool Equals(PropertyMap<T> other) => this.Info.Equals(other.Info);

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

            return this.Equals((PropertyMap<T>)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        /// <summary>
        /// Instructs the mapper to lazily map the property. This method will ignore any non <code>virtual</code> properties without warning.
        /// </summary>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        internal PropertyMap<T> AsAutoLazy()
        {
            if (this.Info.Property.ShouldAttemptLazyLoad())
            {
                this.Info.Lazy = true;
            }

            return this;
        }

        private static int GetHashCode(PropertyMap<T> map)
        {
            unchecked
            {
                return ((map.Info != null ? map.Info.GetHashCode() : 0) * 397) ^ (map.PropertyMapper != null ? map.PropertyMapper.GetHashCode() : 0);
            }
        }
    }
}