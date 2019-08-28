﻿using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using UmbMapper.PropertyMappers;

namespace UmbMapper.Models
{
    /// <summary>
    /// POCO class to define what goes into a PropertyMap<T>
    /// </summary>
    public class PropertyMapDefinition<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapDefinition{T}"/> class.
        /// adfasdf
        /// </summary>
        /// <param name="propertyExpression">The expression for this definition</param>
        internal PropertyMapDefinition(Expression<Func<T, object>> propertyExpression)
        {
            this.PropertyExpression = propertyExpression;
        }

        /// <summary>
        /// Gets the expression that defines the property mapping
        /// </summary>
        public Expression<Func<T, object>> PropertyExpression { get; private set; }

        /// <summary>
        /// Gets an array of aliases
        /// </summary>
        public string[] Aliases { get; private set; }

        /// <summary>
        /// Gets or sets the culture
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets the type of mapper this property uses
        /// </summary>
        public Type MapperType { get; private set; }

        /// <summary>
        /// Gets the Factory Mapper Type this property users
        /// </summary>
        public Type FactoryMapperType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this property is mapped recursively
        /// </summary>
        public bool Recursive { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this property is mapped lazily
        /// </summary>
        public bool Lazy { get; private set; }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Sets the aliases to check against when mapping the property
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to map.</typeparam>
        /// <param name="aliases">The collection of alias identifiers to map from; case insensitive</param>
        /// <returns>The <see cref="PropertyMapDefinition{T}"/></returns>
        public PropertyMapDefinition<T> SetAlias<TProperty>(params Expression<Func<T, TProperty>>[] aliases)
        {
            if (aliases is null)
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

            this.Aliases = altNames;

            return this;
        }

        /// <summary>
        /// Sets the aliases to check against when mapping the property
        /// </summary>
        /// <param name="aliases">The collection of alias identifiers to map from; case insensitive</param>
        /// <returns>The <see cref="PropertyMapDefinition{T}"/></returns>
        public PropertyMapDefinition<T> SetAlias(params string[] aliases)
        {
            if (aliases is null || aliases.Length == 0)
            {
                return this;
            }

            this.Aliases = aliases.Select(x => x.ToUpperInvariant()).ToArray();

            return this;
        }

        /// <summary>
        /// Sets the property mapper for the property
        /// </summary>
        /// <typeparam name="TMapper">The type of property mapper</typeparam>
        /// <returns>The <see cref="PropertyMapDefinition{T}"/></returns>
        public PropertyMapDefinition<T> SetMapper<TMapper>()
            where TMapper : IPropertyMapper
        {
            this.MapperType = typeof(TMapper);

            return this;
        }

        /// <summary>
        /// Sets the property mapper for the property
        /// </summary>
        /// <typeparam name="TMapper">The type of property mapper</typeparam>
        /// <returns>The <see cref="PropertyMapDefinition{T}"/></returns>
        public PropertyMapDefinition<T> SetFactoryMapper<TMapper>()
            where TMapper : FactoryPropertyMapperBase
        {
            this.FactoryMapperType = typeof(TMapper);

            return this;
        }

        /// <summary>
        /// Set this property to map Lazily
        /// </summary>
        /// <returns>The <see cref="PropertyMapDefinition{T}"/></returns>
        public PropertyMapDefinition<T> AsLazy()
        {
            this.Lazy = true;

            return this;
        }

        /// <summary>
        /// Sets this property to map recursively
        /// </summary>
        /// <returns>The <see cref="PropertyMapDefinition{T}"/></returns>
        public PropertyMapDefinition<T> AsRecursive()
        {
            this.Recursive = true;

            return this;
        }
    }
}
