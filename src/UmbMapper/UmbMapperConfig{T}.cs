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
using System.Runtime.CompilerServices;
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
        private readonly List<PropertyMap<T>> maps;
        private readonly bool hasIPublishedConstructor;
        private PropertyMap<T>[] nonLazyMaps;
        private PropertyMap<T>[] lazyMaps;
        private PropertyMap<T>[] nonLazyPredicateMaps;
        private PropertyMap<T>[] lazyPredicateMaps;
        private List<string> lazyNames;
        private FastPropertyAccessor propertyAccessor;
        private Type proxyType;
        private bool hasChecked;
        private bool hasLazy;
        private bool hasPredicate;
        private bool createProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbMapperConfig{T}"/> class.
        /// </summary>
        public UmbMapperConfig()
        {
            this.MappedType = typeof(T);

            // Check the validity of the mapped type constructor as early as possible.
            bool validConstructor = false;
            ParameterInfo[] constructorParams = this.MappedType.GetConstructorParameters();
            if (constructorParams != null)
            {
                // Is it PublishedContentModel or similar?
                if (constructorParams.Length == 1 && constructorParams[0].ParameterType == typeof(IPublishedContent))
                {
                    this.hasIPublishedConstructor = true;
                }

                if (constructorParams.Length == 0 || this.hasIPublishedConstructor)
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

            this.maps = new List<PropertyMap<T>>();
        }

        /// <inheritdoc/>
        public Type MappedType { get; }

        /// <inheritdoc/>
        public IEnumerable<IPropertyMap> Mappings => this.maps;

        /// <summary>
        /// Adds the map from the property to an equivalent Umbraco property
        /// </summary>
        /// <param name="propertyExpression">The property to map</param>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        /// <exception cref="ArgumentException">Thrown if the expression is not a property member expression.</exception>
        public PropertyMap<T> AddMap(Expression<Func<T, object>> propertyExpression)
        {
            if (!this.GetOrCreateMap(propertyExpression.ToPropertyInfo(), out PropertyMap<T> map))
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
            if (propertyExpressions is null)
            {
                return Enumerable.Empty<PropertyMap<T>>();
            }

            var mapsTemp = new List<PropertyMap<T>>();
            foreach (Expression<Func<T, object>> property in propertyExpressions)
            {
                if (!this.GetOrCreateMap(property.ToPropertyInfo(), out PropertyMap<T> map))
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
                if (!this.GetOrCreateMap(property, out PropertyMap<T> map))
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
            PropertyMap<T> map = this.maps.Find(m => m.Info.Property == property);

            return map != null && this.maps.Remove(map);
        }

        /// <inheritdoc/>
        void IUmbMapperConfig.Init()
        {
            // We run the initialization code here so we don't have to run it per mapping.
            lock (UmbMapperConfigStatics.Locker)
            {
                if (this.hasChecked)
                {
                    return;
                }

                // We need to organize mapping now into separate groups as ordering of mappings is important when using predicates.
                this.nonLazyMaps = this.maps.Where(m => !m.Info.HasPredicate && !m.Info.Lazy).ToArray();
                this.lazyMaps = this.maps.Where(m => !m.Info.HasPredicate && m.Info.Lazy).ToArray();
                this.hasLazy = this.lazyMaps.Length > 0;

                this.nonLazyPredicateMaps = this.maps.Where(m => m.Info.HasPredicate && !m.Info.Lazy).ToArray();
                this.lazyPredicateMaps = this.maps.Where(m => m.Info.HasPredicate && m.Info.Lazy).ToArray();
                this.hasPredicate = this.nonLazyPredicateMaps.Length > 0 || this.lazyPredicateMaps.Length > 0;

                this.createProxy = this.hasLazy || this.hasPredicate;

                if (this.createProxy)
                {
                    this.lazyNames = new List<string>();
                    this.lazyNames.AddRange(this.lazyMaps.Select(m => m.Info.Property.Name));
                    this.lazyNames.AddRange(this.lazyPredicateMaps.Select(m => m.Info.Property.Name));
                    this.proxyType = ProxyTypeFactory.CreateProxyType(this.MappedType, this.lazyNames);
                    this.propertyAccessor = new FastPropertyAccessor(this.proxyType);
                }
                else
                {
                    this.propertyAccessor = new FastPropertyAccessor(this.MappedType);
                }

                this.hasChecked = true;
            }
        }

        /// <inheritdoc/>
        object IUmbMapperConfig.CreateEmpty()
        {
            if (this.createProxy)
            {
                var proxy = (IProxy)this.proxyType.GetInstance();
                proxy.Interceptor = new LazyInterceptor(new Dictionary<string, Lazy<object>>());
                return proxy;
            }

            return this.MappedType.GetInstance();
        }

        /// <inheritdoc/>
        object IUmbMapperConfig.CreateEmpty(IPublishedContent content)
        {
            if (this.createProxy)
            {
                var proxy = (IProxy)this.proxyType.GetInstance(content);
                proxy.Interceptor = new LazyInterceptor(new Dictionary<string, Lazy<object>>());
                return proxy;
            }

            return this.MappedType.GetInstance(content);
        }

        /// <inheritdoc/>
        object IUmbMapperConfig.Map(IPublishedContent content)
        {
            object result;
            if (this.createProxy)
            {
                // Create a proxy instance to replace our object.
                result = this.hasIPublishedConstructor ? this.proxyType.GetInstance(content) : this.proxyType.GetInstance();

                // Map the lazy properties and predicate mappings
                Dictionary<string, Lazy<object>> lazyProperties = this.MapLazyProperties(content, result);

                // Set the interceptor and replace our result with the proxy
                ((IProxy)result).Interceptor = new LazyInterceptor(lazyProperties);
            }
            else
            {
                result = this.hasIPublishedConstructor ? this.MappedType.GetInstance(content) : this.MappedType.GetInstance();
            }

            // Users might want to use lazy loading with API controllers that do not inherit from UmbracoAPIController.
            // Certain mappers like Archetype require the context so we want to ensure it exists.
            EnsureUmbracoContext();

            // Now map the non-lazy properties and non-lazy predicate mappings
            this.MapNonLazyProperties(content, result);

            return result;
        }

        /// <inheritdoc/>
        public void Map(IPublishedContent content, object destination)
        {
            // Users might want to use lazy loading with API controllers that do not inherit from UmbracoAPIController.
            // Certain mappers like Archetype require the context so we want to ensure it exists.
            EnsureUmbracoContext();

            // We don't know whether the destination was created by UmbMapper or by something else so we have to check to see if it
            // is a proxy instance.
            if (destination is IProxy proxy)
            {
                // Map the lazy properties and predicate mappings
                Dictionary<string, Lazy<object>> lazyProperties = this.MapLazyProperties(content, destination);

                // Replace the interceptor with our new one.
                var interceptor = new LazyInterceptor(lazyProperties);
                proxy.Interceptor = interceptor;
            }
            else
            {
                // Map our collated lazy properties as non-lazy instead.
                this.MapLazyPropertiesAsNonLazy(content, destination);
            }

            // Map the non-lazy properties and non-lazy predicate mappings
            this.MapNonLazyProperties(content, destination);
        }

        /// <summary>
        /// Adds a map from each writable property in the class to an equivalent Umbraco property
        /// </summary>
        /// <returns>The <see cref="IEnumerable{T}"/></returns>
        internal IEnumerable<PropertyMap<T>> MapAllWritable()
        {
            foreach (PropertyInfo property in typeof(T).GetProperties(UmbMapperConstants.MappableFlags).Where(p => p.CanWrite))
            {
                if (!this.GetOrCreateMap(property, out PropertyMap<T> map))
                {
                    this.maps.Add(map);
                }
            }

            return this.maps;
        }

        private static object MapProperty(PropertyMap<T> map, IPublishedContent content, object result)
        {
            object value = null;

            // If we have a mapping function, use that and skip Umbraco.
            if (map.Info.HasPredicate)
            {
                value = map.Predicate.Invoke((T)result, content);
            }
            else
            {
                // Get the raw value from the content.
                value = map.PropertyMapper.GetRawValue(content);

                // Now map using the given mappers.
                value = map.PropertyMapper.Map(content, value);
            }

            PropertyMapInfo info = map.Info;

            // Try to return if the value is correct.
            if (!(value.IsNullOrEmptyString() || value.Equals(info.DefaultValue))
                && info.PropertyType.IsInstanceOfType(value))
            {
                return value;
            }

            // Ensure everything is the correct return type.
            value = SantizeValue(value, info);

            if (value != null)
            {
                value = RecursivelyMap(value, info);
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
                    Type genericType = info.EnumerableParamType;
                    if (genericType?.IsClass == true)
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
                Type valueType = value.GetType();
                if (valueType == info.PropertyType)
                {
                    return value;
                }

                bool valueIsConvertableEnumerable = valueType.IsConvertableEnumerableType();

                // You cannot set an enumerable of type from an empty object array.
                // This should allow the casting back of IEnumerable<T> to an empty List<T> Collection<T> etc.
                // I cant think of any that don't have an empty constructor
                if (value.Equals(UmbMapperConfigStatics.Empty) && propertyIsCastableEnumerable)
                {
                    Type typeArg = info.EnumerableParamType;
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
                        return EnumerableInvocations.Empty(info.EnumerableParamType);
                    }

                    // Concrete enumerables cannot be cast from Array so we create an instance and return it
                    // if we know it has an empty constructor.
                    ParameterInfo[] constructorParams = info.ConstructorParams;
                    if (constructorParams.Length == 0)
                    {
                        // Internally this uses Activator.CreateInstance which is heavily optimized.
                        return info.PropertyType.GetInstance();
                    }
                }
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureUmbracoContext()
        {
            if (UmbracoContext.Current != null)
            {
                return;
            }

            var dummyHttpContext = new HttpContextWrapper(new HttpContext(new SimpleWorkerRequest("/", string.Empty, new StringWriter())));
            UmbracoContext.EnsureContext(
                dummyHttpContext,
                ApplicationContext.Current,
                new WebSecurity(dummyHttpContext, ApplicationContext.Current),
                UmbracoConfig.For.UmbracoSettings(),
                UrlProviderResolver.Current.Providers,
                false);
        }

        private Dictionary<string, Lazy<object>> MapLazyProperties(IPublishedContent content, object result)
        {
            // First add any lazy mappings, use count to prevent allocations
            var lazyProperties = new Dictionary<string, Lazy<object>>(this.lazyNames.Count);
            for (int i = 0; i < this.lazyMaps.Length; i++)
            {
                // It's better to allocate the `int` via closure than PropertyMap<T>
                int i1 = i;
                lazyProperties[this.lazyMaps[i].Info.Property.Name] = new Lazy<object>(() =>
                {
                    EnsureUmbracoContext();
                    return MapProperty(this.lazyMaps[i1], content, result);
                });
            }

            // Then lazy predicate mappings
            for (int i = 0; i < this.lazyPredicateMaps.Length; i++)
            {
                // It's better to allocate the `int` via closure than PropertyMap<T>
                int i1 = i;
                lazyProperties[this.lazyPredicateMaps[i].Info.Property.Name] = new Lazy<object>(() =>
                {
                    EnsureUmbracoContext();
                    return MapProperty(this.lazyPredicateMaps[i1], content, result);
                });
            }

            return lazyProperties;
        }

        private void MapNonLazyProperties(IPublishedContent content, object destination)
        {
            // First map the non-lazy properties
            for (int i = 0; i < this.nonLazyMaps.Length; i++)
            {
                PropertyMap<T> map = this.nonLazyMaps[i];
                object value = MapProperty(map, content, destination);
                if (value != null)
                {
                    this.propertyAccessor.SetValue(map.Info.Property.Name, destination, value);
                }
            }

            // Then non-lazy predicate mappings
            for (int i = 0; i < this.nonLazyPredicateMaps.Length; i++)
            {
                PropertyMap<T> map = this.nonLazyPredicateMaps[i];
                object value = MapProperty(map, content, destination);
                if (value != null)
                {
                    this.propertyAccessor.SetValue(map.Info.Property.Name, destination, value);
                }
            }
        }

        private void MapLazyPropertiesAsNonLazy(IPublishedContent content, object destination)
        {
            // First map the lazy properties
            for (int i = 0; i < this.lazyMaps.Length; i++)
            {
                PropertyMap<T> map = this.lazyMaps[i];
                object value = MapProperty(map, content, destination);
                if (value != null)
                {
                    this.propertyAccessor.SetValue(map.Info.Property.Name, destination, value);
                }
            }

            // Then lazy predicate mappings
            for (int i = 0; i < this.lazyPredicateMaps.Length; i++)
            {
                PropertyMap<T> map = this.lazyPredicateMaps[i];
                object value = MapProperty(map, content, destination);
                if (value != null)
                {
                    this.propertyAccessor.SetValue(map.Info.Property.Name, destination, value);
                }
            }
        }

        private bool GetOrCreateMap(PropertyInfo property, out PropertyMap<T> map)
        {
            bool exists = true;
            map = this.maps.Find(x => x.Info.Property.Name == property.Name);

            if (map is null)
            {
                exists = false;
                map = new PropertyMap<T>(property);
            }

            return exists;
        }
    }
}