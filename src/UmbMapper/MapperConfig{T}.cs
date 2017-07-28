// <copyright file="MapperConfig{T}.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UmbMapper.Extensions;
using UmbMapper.Invocations;
using UmbMapper.PropertyMappers;
using UmbMapper.Proxy;
using Umbraco.Core.Models;

namespace UmbMapper
{
    /// <summary>
    /// Configures mapping of type properties to Umbraco properties
    /// </summary>
    /// <typeparam name="T">The type of object to map</typeparam>
    public class MapperConfig<T> : IMapperConfig
        where T : class, new()
    {
        private readonly FastPropertyAccessor propertyAccessor;
        private readonly List<PropertyMap<T>> maps;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapperConfig{T}"/> class.
        /// </summary>
        public MapperConfig()
        {
            Type type = typeof(T);
            this.MappedType = type;
            this.propertyAccessor = new FastPropertyAccessor(type);
            this.maps = new List<PropertyMap<T>>();
        }

        /// <inheritdoc/>
        public Type MappedType { get; }

        /// <summary>
        /// Adds the map to the property from an Umbraco property
        /// </summary>
        /// <param name="expression">The property to map</param>
        /// <returns>The <see cref="PropertyMap{T}"/></returns>
        public PropertyMap<T> AddMap(Expression<Func<T, object>> expression)
        {
            // The property access might be getting converted to object to match the func
            // If so, get the operand and see if that's a member expression
            MemberExpression member = expression.Body as MemberExpression
                ?? (expression.Body as UnaryExpression)?.Operand as MemberExpression;

            if (member == null)
            {
                throw new ArgumentException("Action must be a member expression.");
            }

            var map = new PropertyMap<T>(member.Member as PropertyInfo);
            this.maps.Add(map);
            return map;
        }

        /// <inheritdoc/>
        public object Map(IPublishedContent content)
        {
            object result = this.MappedType.GetInstance();

            // Gather any lazily mapped properties and assign the lazy mapping
            IEnumerable<PropertyMap<T>> lazyMaps = this.maps.Where(m => m.Config.Lazy && m.Config.Property.ShouldAttemptLazyLoad()).ToArray();
            if (lazyMaps.Any())
            {
                // A dictionary to store lazily invoked values.
                var lazyProperties = new Dictionary<string, Lazy<object>>();
                foreach (PropertyMap<T> map in lazyMaps)
                {
                    object localResult = result;
                    lazyProperties.Add(map.Config.Property.Name, new Lazy<object>(() => MapProperty(map, content, this.propertyAccessor, localResult)));
                }

                // Create a proxy instance to replace our object.
                var interceptor = new LazyInterceptor(result, lazyProperties);
                var factory = new ProxyFactory();
                result = factory.CreateProxy(this.MappedType, interceptor);
            }

            // Now map the non-lazy properties
            foreach (PropertyMap<T> map in this.maps.Where(m => !m.Config.Lazy && !m.Config.Property.ShouldAttemptLazyLoad()))
            {
                MapProperty(map, content, this.propertyAccessor, result);
            }

            return result;
        }

        private static object MapProperty(PropertyMap<T> map, IPublishedContent content, FastPropertyAccessor propertyAccessor, object result)
        {
            // We don't have to explictly set a mapper.
            if (map.PropertyMapper == null)
            {
                map.PropertyMapper = new UmbracoPropertyMapper(map.Config);
            }

            // Ensure the property mapper is always invoked first
            object value = null;
            if (!(map.PropertyMapper is UmbracoPropertyMapper))
            {
                value = new UmbracoPropertyMapper(map.Config).Map(content, null);
            }

            // Other mappers
            value = map.PropertyMapper.Map(content, value);
            if (value != null)
            {
                Type propertyType = map.PropertyType;
                value = SantizeValue(value, propertyType);
                value = RecursivelyMap(value, propertyType);

                if (propertyType.IsAssignableFrom(value))
                {
                    propertyAccessor.SetValue(map.PropertyMapper.Property.Name, result, value);
                }
            }

            return value;
        }

        private static object RecursivelyMap(object value, Type propertyType)
        {
            if (!propertyType.IsInstanceOfType(value))
            {
                // If the property value is an IPublishedContent, then we can map it to the target type.
                var content = value as IPublishedContent;
                if (content != null && propertyType.IsClass)
                {
                    return content.MapTo(propertyType);
                }

                // If the property value is an IEnumerable<IPublishedContent>, then we can map it to the target type.
                if (value.GetType().IsEnumerableOfType(typeof(IPublishedContent)) && propertyType.IsEnumerableType())
                {
                    Type genericType = propertyType.GetEnumerableType();
                    if (genericType != null && genericType.IsClass)
                    {
                        return ((IEnumerable<IPublishedContent>)value).MapTo(genericType);
                    }
                }
            }

            return value;
        }

        private static object SantizeValue(object value, Type propertyType)
        {
            bool propertyIsEnumerable = propertyType.IsCastableEnumerableType();

            if (value != null)
            {
                bool valueIsEnumerable = value.GetType().IsCastableEnumerableType();

                // You cannot set an enumerable of type from an empty object array.
                // This should allow the casting back of IEnumerable<T> to an empty List<T> Collection<T> etc.
                // I cant think of any that don't have an empty constructor
                if (value.Equals(Enumerable.Empty<object>()) && propertyIsEnumerable)
                {
                    Type typeArg = propertyType.GetTypeInfo().GenericTypeArguments.First();
                    return propertyType.IsInterface ? EnumerableInvocations.Cast(typeArg, (IEnumerable)value) : propertyType.GetInstance();
                }

                // Ensure only a single item is returned when requested.
                if (valueIsEnumerable && !propertyIsEnumerable)
                {
                    // Property is not enumerable, but value is, so grab first item
                    IEnumerator enumerator = ((IEnumerable)value).GetEnumerator();
                    return enumerator.MoveNext() ? enumerator.Current : null;
                }

                // And now check for the reverse situation.
                if (!valueIsEnumerable && propertyIsEnumerable)
                {
                    var array = Array.CreateInstance(value.GetType(), 1);
                    array.SetValue(value, 0);
                    return array;
                }
            }
            else
            {
                if (propertyIsEnumerable)
                {
                    if (propertyType.IsInterface && !propertyType.IsEnumerableOfKeyValueType())
                    {
                        // Value is null, but property is enumerable interface, so return empty enumerable
                        return EnumerableInvocations.Empty(propertyType.GenericTypeArguments.First());
                    }

                    // Concrete enumerables cannot be cast from Array so we create an instance and return it
                    // if we know it has an empty constructor.
                    ParameterInfo[] constructorParams = propertyType.GetConstructorParameters();
                    if (constructorParams != null && constructorParams.Length == 0)
                    {
                        // Internally this uses Activator.CreateInstance which is heavily optimized.
                        return propertyType.GetInstance();
                    }
                }
            }

            return value;
        }
    }
}