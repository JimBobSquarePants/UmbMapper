// <copyright file="PropertyMapperBase.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// The base class for all property mappers
    /// </summary>
    public abstract class PropertyMapperBase : IPropertyMapper
    {
        /// <summary>
        /// The cache for storing created default types.
        /// </summary>
        public static readonly ConcurrentDictionary<Type, object> TypeDefaultsCache = new ConcurrentDictionary<Type, object>();

        private readonly CultureInfo culture;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapperBase"/> class.
        /// </summary>
        /// <param name="config">The property configuration</param>
        protected PropertyMapperBase(PropertyMapperConfig config)
        {
            this.PropertyType = config.PropertyType;
            this.Property = config.Property;

            if (config.Aliases == null || !config.Aliases.Any())
            {
                this.Aliases = new[] { this.Property.Name };
            }
            else
            {
                this.Aliases = config.Aliases;
            }

            this.Recursive = config.Recursive;
            this.DefaultValue = config.DefaultValue ?? this.GetDefaultValue(this.PropertyType);
            this.culture = config.Culture;
        }

        /// <inheritdoc/>
        public PropertyInfo Property { get; }

        /// <inheritdoc/>
        public Type PropertyType { get; }

        /// <inheritdoc/>
        public string[] Aliases { get; }

        /// <inheritdoc/>
        public bool Recursive { get; }

        /// <inheritdoc/>
        public object DefaultValue { get; }

        /// <inheritdoc/>
        public CultureInfo Culture => this.GetCulture();

        /// <inheritdoc/>
        public UmbracoContext UmbracoContext => this.GetUmbracoContext();

        /// <inheritdoc/>
        public MembershipHelper Members => new MembershipHelper(this.UmbracoContext);

        /// <inheritdoc/>
        public UmbracoHelper Umbraco => new UmbracoHelper(this.UmbracoContext);

        /// <inheritdoc/>
        public abstract object Map(IPublishedContent content, object value);

        /// <summary>
        /// Returns the default value for the given type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to return.</param>
        /// <returns>The <see cref="object"/> representing the default value.</returns>
        public object GetDefaultValue(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // We want a Func<object> which returns the default value.
            // Create that expression, convert to object.
            // The default value, will always be what the runtime tells us.
            var e = Expression.Lambda<Func<object>>(Expression.Convert(Expression.Default(type), typeof(object)));

            return e.Compile()();
        }

        private CultureInfo GetCulture()
        {
            if (this.culture != null)
            {
                return this.culture;
            }

            if (this.UmbracoContext?.PublishedContentRequest != null)
            {
                return this.UmbracoContext.PublishedContentRequest.Culture;
            }

            return CultureInfo.CurrentCulture;
        }

        private UmbracoContext GetUmbracoContext()
        {
            return UmbracoContext.Current ?? throw new InvalidOperationException("UmbracoContext.Current is null.");
        }
    }
}