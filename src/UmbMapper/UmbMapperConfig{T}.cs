// <copyright file="UmbMapperConfig{T}.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using UmbMapper.Extensions;
using UmbMapper.Invocations;
using UmbMapper.PropertyMappers;
using UmbMapper.Proxy;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace UmbMapper
{
    /// <summary>
    /// Configures mapping of type properties to Umbraco properties
    /// </summary>
    /// <typeparam name="T">The type of object to map</typeparam>
    public class UmbMapperConfig<T> : IUmbMapperConfig
        where T : class
    {
        private readonly FastPropertyAccessor propertyAccessor;
        private readonly List<PropertyMap<T>> maps;
        private readonly bool hasIPublishedContructor;
        private IEnumerable<PropertyMap<T>> nonLazyMaps;
        private IEnumerable<PropertyMap<T>> lazyMaps;
        private IEnumerable<PropertyMap<T>> nonLazyPredicateMaps;
        private IEnumerable<PropertyMap<T>> lazyPredicateMaps;
        private bool hasChecked;
        private bool hasLazy;
        private bool hasPredicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbMapperConfig{T}"/> class.
        /// </summary>
        public UmbMapperConfig()
        {
            Type type = typeof(T);
            this.MappedType = type;

            // Check the validity of the mpped type constructor as early as possible.
            bool validConstructor = false;
            ParameterInfo[] constructorParams = this.MappedType.GetConstructorParameters();
            if (constructorParams != null)
            {
                // Is it PublishedContentmModel or similar?
                if (constructorParams.Length == 1 && constructorParams[0].ParameterType == typeof(IPublishedContent))
                {
                    this.hasIPublishedContructor = true;
                }

                if (constructorParams.Length == 0 || this.hasIPublishedContructor)
                {
                    validConstructor = true;
                }
            }

            if (!validConstructor)
            {
                throw new InvalidOperationException(
                    $"Cannot convert IPublishedContent to {this.MappedType} as it has no valid constructor. " +
                    "A valid constructor is either an empty one, or one accepting a single IPublishedContent parameter.");
            }

            this.propertyAccessor = new FastPropertyAccessor(type);
            this.maps = new List<PropertyMap<T>>();
        }

        /// <inheritdoc/>
        public Type MappedType { get; }

        /// <summary>
        /// Gets the collection of mappings registered with the mapper
        /// </summary>
        public IReadOnlyCollection<PropertyMap<T>> Mappings => this.maps;

        /// <summary>
        /// Adds the map from the property to an equivalent Umbraco property
        /// </summary>
        /// <param name="propertyExpression">The property to map</param>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        /// <exception cref="ArgumentException">Thrown if the expression is not a property member expression.</exception>
        public PropertyMap<T> AddMap(Expression<Func<T, object>> propertyExpression)
        {
            if (!this.GetOrCreateMap(propertyExpression.ToPropertyInfo(), out var map))
            {
                this.maps.Add(map);
            }

            return map;
        }

        /// <summary>
        /// Adds the map from each property to an equivalent Umbraco property
        /// </summary>
        /// <param name="propertyExpressions">The properties to map</param>
        /// <returns>The <see cref="IEnumerable{T}"/></returns>
        /// <exception cref="ArgumentException">Thrown if the expression is not a property member expression.</exception>
        public IEnumerable<PropertyMap<T>> AddMappings(params Expression<Func<T, object>>[] propertyExpressions)
        {
            if (propertyExpressions == null)
            {
                return Enumerable.Empty<PropertyMap<T>>();
            }

            var mapsTemp = new List<PropertyMap<T>>();
            foreach (Expression<Func<T, object>> property in propertyExpressions)
            {
                if (!this.GetOrCreateMap(property.ToPropertyInfo(), out var map))
                {
                    this.maps.Add(map);
                }

                mapsTemp.Add(map);
            }

            // We only want to return the new maps for subsequent augmentation
            return this.maps.Intersect(mapsTemp);
        }

        /// <summary>
        /// Adds a map from each property in the class to an equivalent Umbraco property
        /// </summary>
        /// <returns>The <see cref="IEnumerable{T}"/></returns>
        public IEnumerable<PropertyMap<T>> MapAll()
        {
            foreach (PropertyInfo property in typeof(T).GetProperties(UmbMapperConstants.MappableFlags))
            {
                if (!this.GetOrCreateMap(property, out var map))
                {
                    this.maps.Add(map);
                }
            }

            return this.maps;
        }

        /// <summary>
        /// Removes the property in the class from the collection of mapped properties
        /// </summary>
        /// <param name="propertyExpression">The property to map</param>
        /// <returns>The <see cref="bool"/></returns>
        public bool Ignore(Expression<Func<T, object>> propertyExpression)
        {
            var property = propertyExpression.ToPropertyInfo();
            PropertyMap<T> map = this.maps.FirstOrDefault(m => m.Info.Property == property);

            return map != null && this.maps.Remove(map);
        }

        /// <inheritdoc/>
        object IUmbMapperConfig.Map(IPublishedContent content)
        {
            // Handle both empty and IPublishedContent contructor
            object result = this.hasIPublishedContructor ? this.MappedType.GetInstance(content) : this.MappedType.GetInstance();

            IProxy proxy = null;

            if (this.hasLazy || this.hasPredicate)
            {
                // Create a proxy instance to replace our object.
                var factory = new ProxyFactory();
                proxy = this.hasIPublishedContructor ? factory.CreateProxy(this.MappedType, content) : factory.CreateProxy(this.MappedType);

                // A dictionary to store lazily invoked value results
                var lazyProperties = new Dictionary<string, Lazy<object>>();

                // First add any lazy mappings
                foreach (PropertyMap<T> map in this.lazyMaps)
                {
                    // Prevent closure allocation
                    object localResult = result;
                    lazyProperties.Add(map.Info.Property.Name, new Lazy<object>(() => MapProperty(map, content, this.propertyAccessor, localResult, proxy)));
                }

                // Then lazy predicate mappings
                foreach (PropertyMap<T> map in this.lazyPredicateMaps)
                {
                    // Prevent closure allocation
                    object localResult = result;
                    lazyProperties.Add(map.Info.Property.Name, new Lazy<object>(() => MapProperty(map, content, this.propertyAccessor, localResult, proxy)));
                }

                // Set the interceptor and replace our result with the proxy
                var interceptor = new LazyInterceptor(result, lazyProperties);
                proxy.Interceptor = interceptor;
            }

            // Now map the non-lazy properties
            foreach (PropertyMap<T> map in this.nonLazyMaps)
            {
                MapProperty(map, content, this.propertyAccessor, proxy ?? result, proxy);
            }

            // Then non-lazy predicate mappings
            foreach (PropertyMap<T> map in this.nonLazyPredicateMaps)
            {
                MapProperty(map, content, this.propertyAccessor, proxy ?? result, proxy);
            }

            return proxy ?? result;
        }

        /// <inheritdoc/>
        void IUmbMapperConfig.Init()
        {
            // We run the initialization code here so we don't have to run it per mapping.
            lock (UmbMapperConfigLocker.Locker)
            {
                if (this.hasChecked)
                {
                    return;
                }

                // We need to organize mapping now into separate groups as ordering of mappings is important when using predicates.
                this.nonLazyMaps = this.maps.Where(m => !m.Info.HasPredicate && !m.Info.Lazy).ToArray();
                this.lazyMaps = this.maps.Where(m => !m.Info.HasPredicate && m.Info.Lazy).ToArray();
                this.hasLazy = this.lazyMaps.Any();

                this.nonLazyPredicateMaps = this.maps.Where(m => m.Info.HasPredicate && !m.Info.Lazy).ToArray();
                this.lazyPredicateMaps = this.maps.Where(m => m.Info.HasPredicate && m.Info.Lazy).ToArray();
                this.hasPredicate = this.nonLazyPredicateMaps.Any() || this.lazyPredicateMaps.Any();
                this.hasChecked = true;
            }
        }

        /// <summary>
        /// Adds a map from each writable property in the class to an equivalent Umbraco property
        /// </summary>
        /// <returns>The <see cref="IEnumerable{T}"/></returns>
        internal IEnumerable<PropertyMap<T>> MapAllWritable()
        {
            foreach (PropertyInfo property in typeof(T).GetProperties(UmbMapperConstants.MappableFlags).Where(p => p.CanWrite))
            {
                if (!this.GetOrCreateMap(property, out var map))
                {
                    this.maps.Add(map);
                }
            }

            return this.maps;
        }

        private static object MapProperty(PropertyMap<T> map, IPublishedContent content, FastPropertyAccessor propertyAccessor, object result, IProxy proxy)
        {
            // Users might want to use lazy loading with API controllers that do not inherit from UmbracoAPIController.
            // Certain mappers like Archtype require the context so we want to ensure it exists.
            EnsureUmbracoContext();

            object value = null;

            // If we have a mapping function, use that and skip Umbraco
            if (map.Info.HasPredicate)
            {
                // If we have a proxy we have to go via that class to get lazy mapped properties
                value = proxy == null ? map.Predicate.Invoke((T)result, content) : map.Predicate.Invoke((T)proxy, content);
            }
            else
            {
                // We don't have to explictly set a mapper.
                if (map.PropertyMapper == null)
                {
                    map.PropertyMapper = new UmbracoPropertyMapper(map.Info);
                }

                // Ensure the property mapper is always invoked first
                if (!(map.PropertyMapper is UmbracoPropertyMapper))
                {
                    value = new UmbracoPropertyMapper(map.Info).Map(content, null);
                }

                // Other mappers
                value = map.PropertyMapper.Map(content, value);
            }

            PropertyMapInfo info = map.Info;
            value = SantizeValue(value, info);

            if (value != null)
            {
                value = RecursivelyMap(value, info);
                map.Info.Property.SetValue(result, value);
                // propertyAccessor.SetValue(map.Info.Property.Name, result, value);
            }

            return value;
        }

        private static object RecursivelyMap(object value, PropertyMapInfo info)
        {
            if (!info.PropertyType.IsInstanceOfType(value))
            {
                // If the property value is an IPublishedContent, then we can map it to the target type.
                if (value is IPublishedContent content && info.PropertyType.IsClass)
                {
                    return content.MapTo(info.PropertyType);
                }

                // If the property value is an IEnumerable<IPublishedContent>, then we can map it to the target type.
                if (value.GetType().IsEnumerableOfType(typeof(IPublishedContent)) && info.IsEnumerableType)
                {
                    Type genericType = info.PropertyType.GetEnumerableType();
                    if (genericType != null && genericType.IsClass)
                    {
                        return ((IEnumerable<IPublishedContent>)value).MapTo(genericType);
                    }
                }
            }

            return value;
        }

        private static object SantizeValue(object value, PropertyMapInfo info)
        {
            bool propertyIsCastableEnumerable = info.IsCastableEnumerableType;
            bool propertyIsConvertableEnumerable = info.IsConvertableEnumerableType;

            if (value != null)
            {
                bool valueIsConvertableEnumerable = value.GetType().IsConvertableEnumerableType();

                // You cannot set an enumerable of type from an empty object array.
                // This should allow the casting back of IEnumerable<T> to an empty List<T> Collection<T> etc.
                // I cant think of any that don't have an empty constructor
                if (value.Equals(Enumerable.Empty<object>()) && propertyIsCastableEnumerable)
                {
                    Type typeArg = info.PropertyType.GetTypeInfo().GenericTypeArguments.First();
                    return info.PropertyType.IsInterface ? EnumerableInvocations.Cast(typeArg, (IEnumerable)value) : info.PropertyType.GetInstance();
                }

                // Ensure only a single item is returned when requested.
                if (valueIsConvertableEnumerable && !propertyIsConvertableEnumerable)
                {
                    // Property is not enumerable, but value is, so grab first item
                    IEnumerator enumerator = ((IEnumerable)value).GetEnumerator();
                    return enumerator.MoveNext() ? enumerator.Current : null;
                }

                // And now check for the reverse situation.
                if (!valueIsConvertableEnumerable && propertyIsConvertableEnumerable)
                {
                    var array = Array.CreateInstance(value.GetType(), 1);
                    array.SetValue(value, 0);
                    return array;
                }
            }
            else
            {
                if (propertyIsCastableEnumerable)
                {
                    if (info.PropertyType.IsInterface && !info.IsEnumerableOfKeyValueType)
                    {
                        // Value is null, but property is enumerable interface, so return empty enumerable
                        return EnumerableInvocations.Empty(info.PropertyType.GenericTypeArguments.First());
                    }

                    // Concrete enumerables cannot be cast from Array so we create an instance and return it
                    // if we know it has an empty constructor.
                    ParameterInfo[] constructorParams = info.PropertyType.GetConstructorParameters();
                    if (constructorParams != null && constructorParams.Length == 0)
                    {
                        // Internally this uses Activator.CreateInstance which is heavily optimized.
                        return info.PropertyType.GetInstance();
                    }
                }
            }

            return value;
        }

        private static void EnsureUmbracoContext()
        {
            if (UmbracoContext.Current == null)
            {
                var dummyHttpContext = new HttpContextWrapper(new HttpContext(new SimpleWorkerRequest("/", string.Empty, new StringWriter())));
                UmbracoContext.EnsureContext(
                    dummyHttpContext,
                    ApplicationContext.Current,
                    new WebSecurity(dummyHttpContext, ApplicationContext.Current),
                    UmbracoConfig.For.UmbracoSettings(),
                    UrlProviderResolver.Current.Providers,
                    false);
            }
        }

        private bool GetOrCreateMap(PropertyInfo property, out PropertyMap<T> map)
        {
            bool exists = true;
            map = this.maps.FirstOrDefault(x => x.Info.Property.Name == property.Name);

            if (map == null)
            {
                exists = false;
                map = new PropertyMap<T>(property);
            }

            return exists;
        }
    }
}