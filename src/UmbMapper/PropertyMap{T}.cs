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

namespace UmbMapper
{
    /// <summary>
    /// Provides the mechanism for mapping a property from the Umbraco published content
    /// </summary>
    /// <typeparam name="T">The type of object to map</typeparam>
    public class PropertyMap<T>
        where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMap{T}"/> class.
        /// </summary>
        /// <param name="property">The property to map</param>
        public PropertyMap(PropertyInfo property)
        {
            this.Config = new PropertyMapperConfig { Property = property, PropertyType = property.PropertyType };
        }

        /// <summary>
        /// Gets the property type
        /// </summary>
        public Type PropertyType => this.Config.PropertyType;

        /// <summary>
        /// Gets the mapping configuration
        /// </summary>
        public PropertyMapperConfig Config { get; }

        /// <summary>
        /// Gets the property mapper
        /// </summary>
        public IPropertyMapper PropertyMapper { get; internal set; }

        /// <summary>
        /// Sets the aliases to check against when mapping the property
        /// </summary>
        /// <param name="aliases">The collection of alias identifiers to map from; case insensitive</param>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> SetAlias(params Expression<Func<T, object>>[] aliases)
        {
            if (aliases == null)
            {
                return this;
            }

            string[] altNames = new string[aliases.Length];
            for (int i = 0; i < aliases.Length; i++)
            {
                Expression<Func<T, object>> expression = aliases[i];

                // The property access might be getting converted to object to match the func
                // If so, get the operand and see if that's a member expression
                MemberExpression member = expression.Body as MemberExpression
                                          ?? (expression.Body as UnaryExpression)?.Operand as MemberExpression;

                if (member == null)
                {
                    throw new ArgumentException("Action must be a member expression.");
                }

                altNames[i] = member.Member.Name;
            }

            this.Config.Aliases = altNames;
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

            this.Config.Aliases = aliases;
            return this;
        }

        /// <summary>
        /// Sets the property mapper for the property
        /// </summary>
        /// <typeparam name="TMapper">The type of property mapper</typeparam>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> SetMapper<TMapper>()
            where TMapper : IPropertyMapper
        {
            this.PropertyMapper = (TMapper)typeof(TMapper).GetInstance(this.Config);
            return this;
        }

        /// <summary>
        /// Sets the culture to use when mapping the property
        /// </summary>
        /// <param name="culture">The specific culture</param>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> SetCulture(CultureInfo culture)
        {
            this.Config.Culture = culture;
            return this;
        }

        /// <summary>
        /// Instructs the mapper to map the property recursively up the tree
        /// </summary>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> AsRecursive()
        {
            this.Config.Recursive = true;
            return this;
        }

        /// <summary>
        /// Instructs the mapper to lazily map the property
        /// </summary>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> AsLazy()
        {
            this.Config.Lazy = true;
            return this;
        }

        /// <summary>
        /// Sets the default value for the mapper
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> DefaultValue(object value)
        {
            this.Config.DefaultValue = value;
            return this;
        }
    }
}