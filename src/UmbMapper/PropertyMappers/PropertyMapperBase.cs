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
using System.Runtime.CompilerServices;
using System.Web;
using Umbraco.Core;
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
        /// <param name="info">The property map information</param>
        protected PropertyMapperBase(PropertyMapInfo info)
        {
            this.Property = info.Property;
            this.PropertyType = info.PropertyType;
            this.IsEnumerableType = info.IsEnumerableType;
            this.EnumerableParamType = info.EnumerableParamType;
            this.IsCastableEnumerableType = info.IsCastableEnumerableType;
            this.IsConvertableEnumerableType = info.IsConvertableEnumerableType;
            this.IsEnumerableOfKeyValueType = info.IsEnumerableOfKeyValueType;

            if (info.Aliases == null || !info.Aliases.Any())
            {
                this.Aliases = new[] { this.Property.Name.ToUpperInvariant() };
            }
            else
            {
                this.Aliases = info.Aliases.Select(x => x.ToUpperInvariant()).ToArray();
            }

            this.Recursive = info.Recursive;
            this.Lazy = info.Lazy;
            this.HasPredicate = info.HasPredicate;
            this.DefaultValue = info.DefaultValue ?? this.GetDefaultValue(this.PropertyType);
            this.culture = info.Culture;
        }

        /// <inheritdoc/>
        public PropertyInfo Property { get; }

        /// <inheritdoc/>
        public Type PropertyType { get; }

        /// <inheritdoc/>
        public ParameterInfo[] ConstructorParams { get; }

        /// <inheritdoc />
        public bool IsEnumerableType { get; }

        /// <inheritdoc />
        public Type EnumerableParamType { get; }

        /// <inheritdoc />
        public bool IsConvertableEnumerableType { get; }

        /// <inheritdoc />
        public bool IsCastableEnumerableType { get; }

        /// <inheritdoc />
        public bool IsEnumerableOfKeyValueType { get; }

        /// <inheritdoc/>
        public string[] Aliases { get; }

        /// <inheritdoc/>
        public bool Recursive { get; }

        /// <inheritdoc/>
        public bool Lazy { get; }

        /// <inheritdoc/>
        public bool HasPredicate { get; }

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

        /// <summary>
        /// Checks the value to see if it is an instance of the given type and attempts to
        /// convert the value to the correct type if it is not.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="object"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected object CheckConvertType(object value)
        {
            if (value == null || this.PropertyType.IsInstanceOfType(value))
            {
                return value;
            }

            try
            {
                Attempt<object> attempt = value.TryConvertTo(this.PropertyType);
                if (attempt.Success)
                {
                    return attempt.Result;
                }
            }
            catch
            {
                return value;
            }

            // Special case for IHtmlString top remove Html.Raw requirement
            if (value is string && this.PropertyType == typeof(IHtmlString))
            {
                value = new HtmlString(value.ToString());
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private UmbracoContext GetUmbracoContext()
        {
            return UmbracoContext.Current ?? throw new InvalidOperationException("UmbracoContext.Current is null.");
        }
    }
}