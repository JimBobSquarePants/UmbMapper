// <copyright file="UmbMapperRegistry.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UmbMapper.Extensions;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace UmbMapper
{
    public interface IUmbMapperRegistry
    {
        ConcurrentDictionary<Type, IUmbMapperConfig> Mappers { get; }
        
        // This is here until I figure out how to get the generics correct
        //ConcurrentDictionary<Type, IMappingProcessor> MappingProcessors { get; }
        IEnumerable<Type> CurrentMappedTypes();

        void AddMapperFor<T>()
            where T : class;

        void AddMapper(IUmbMapperConfig config);

        void AddMapper<TMapper, TDestination>()
            where TMapper : UmbMapperConfig<TDestination>
            where TDestination : class;
    }

    /// <summary>
    /// The registry for mapper configurations
    /// </summary>
    public class UmbMapperRegistry : IUmbMapperRegistry, IDisposable
    {
        //private readonly IUmbracoContextFactory umbracoContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbMapperRegistry"/> class.
        /// </summary>
        /// <param name="umbracoContextFactory">Umbraco Context factory</param>
        public UmbMapperRegistry(IUmbracoContextFactory umbracoContextFactory)
        {
            //this.umbracoContextFactory = umbracoContextFactory;
        }

        /// <summary>
        /// Gets the content accessor cache for the different types of published content.
        /// </summary>
        internal static ConcurrentDictionary<string, FastPropertyAccessor> ContentAccessorCache { get; }
            = new ConcurrentDictionary<string, FastPropertyAccessor>();

        /// <summary>
        /// Gets the collection of mappers
        /// </summary>
        /// //TODO - why is this internal - do we need a separate class/interface for and internal and public members?
        public ConcurrentDictionary<Type, IUmbMapperConfig> Mappers { get; } = new ConcurrentDictionary<Type, IUmbMapperConfig>();

        //ConcurrentDictionary<Type, IMappingProcessor> MappingProcessors { get; } = new ConcurrentDictionary<Type, IMappingProcessor>();
        //internal ConcurrentDictionary<Type, IUmbMapperConfig> Mappers { get; } = new ConcurrentDictionary<Type, IUmbMapperConfig>();

        /// <summary>
        /// Gets a readonly collection of the registered mappers
        /// </summary>
        /// <returns>The <see cref="IReadOnlyCollection{T}"/></returns>
        public IEnumerable<IUmbMapperConfig> CurrentMappers()
            => this.Mappers.Values;

        /// <summary>
        /// Gets a readonly collection of the registered mapped types
        /// </summary>
        /// <returns>The <see cref="IReadOnlyCollection{T}"/></returns>
        public IEnumerable<Type> CurrentMappedTypes()
            => this.Mappers.Keys;

        /// <summary>
        /// Adds the mapper configuration to the mapping registry
        /// </summary>
        /// <param name="config">The mapper configuration</param>
        public void AddMapper(IUmbMapperConfig config)
        {
            if (this.Mappers.ContainsKey(config.MappedType))
            {
                return;
            }

            config.Init();
            this.Mappers.TryAdd(config.MappedType, config);
        }

        public void AddMapper<TMapper, T>()
            where TMapper : UmbMapperConfig<T>
            where T : class
        {
            UmbMapperConfig<T> mapperConfig = Activator.CreateInstance(typeof(TMapper)) as UmbMapperConfig<T>;
            mapperConfig.OnNewMapAdded += this.Mapper_OnNewMapAdded<T>;
            mapperConfig.OnNewMapsAdded += this.Mapper_OnNewMapsAdded;

            mapperConfig.Init();
            this.Mappers.TryAdd(mapperConfig.MappedType, mapperConfig);
        }

        private void Mapper_OnNewMapAdded<T>(UmbMapperConfig<T> mappingConfig, Expression<Func<T, object>> propertyExpression, out PropertyMap<T> map)
            where T : class
        {
            if (!this.GetOrCreateMap<T>(mappingConfig, propertyExpression.ToPropertyInfo(), out map))
            {
                mappingConfig.Maps.Add(map);
            }
        }

        private void Mapper_OnNewMapsAdded<T>(UmbMapperConfig<T> mappingConfig, out IEnumerable<PropertyMap<T>> maps, params Expression<Func<T, object>>[] propertyExpressions)
            where T : class
        {
            if (propertyExpressions is null)
            {
                maps = Enumerable.Empty<PropertyMap<T>>();
                return;
            }

            var mapsTemp = new List<PropertyMap<T>>();
            foreach (Expression<Func<T, object>> property in propertyExpressions)
            {
                if (!this.GetOrCreateMap(mappingConfig, property.ToPropertyInfo(), out PropertyMap<T> map))
                {
                    mappingConfig.Maps.Add(map);
                }

                mapsTemp.Add(map);
            }

            // We only want to return the new maps for subsequent augmentation
            maps = mappingConfig.Maps.Intersect(mapsTemp);
        }



        private bool GetOrCreateMap<T>(UmbMapperConfig<T> mapping, PropertyInfo property, out PropertyMap<T> map)
            where T : class
        {
            bool exists = true;
            map = mapping.Maps.Find(x => x.Info.Property.Name == property.Name);

            if (map is null)
            {
                exists = false;
                map = new PropertyMap<T>(property);
            }

            return exists;
        }

        /// <summary>
        /// Creates a mapper for the given type, adding that mapper to the mapping registry
        /// </summary>
        /// <remarks>Any properties marked <code>virtual</code> are automatically lazy mapped.</remarks>
        /// <typeparam name="T">The type of object to map</typeparam>
        public void AddMapperFor<T>()
            where T : class
        {
            if (this.Mappers.ContainsKey(typeof(T)))
            {
                return;
            }

            var config = new UmbMapperConfig<T>();
            config.MapAllWritable().ForEach(x => x.AsAutoLazy());
            ((IUmbMapperConfig)config).Init();

            this.Mappers.TryAdd(config.MappedType, config);
        }

        /// <summary>
        /// Creates an empty instance of the given type.
        /// If the configuration for the type contains lazy mappings a transparent proxy is returned.
        /// </summary>
        /// <typeparam name="T">The type of object to create</typeparam>
        /// <returns>The <typeparamref name="T"/></returns>
        public T CreateEmpty<T>()
            where T : class
        {
            this.Mappers.TryGetValue(typeof(T), out IUmbMapperConfig mapper);

            if (mapper is null)
            {
                throw new InvalidOperationException($"No mapper for the given type {typeof(T)} has been registered.");
            }

            return (T)mapper.CreateEmpty();
        }

        /// <summary>
        /// Creates an empty instance of the given type passing the published content to the constructor.
        /// If the configuration for the type contains lazy mappings a transparent proxy is returned.
        /// </summary>
        /// <typeparam name="T">The type of object to create</typeparam>
        /// <param name="content">The content that this instance will map from.</param>
        /// <returns>The <typeparamref name="T"/></returns>
        public T CreateEmpty<T>(IPublishedContent content)
            where T : class
        {
            this.Mappers.TryGetValue(typeof(T), out IUmbMapperConfig mapper);

            if (mapper is null)
            {
                throw new InvalidOperationException($"No mapper for the given type {typeof(T)} has been registered.");
            }

            return (T)mapper.CreateEmpty(content);
        }

        /// <summary>
        /// Clears the mappers from the registry
        /// </summary>
        public void ClearMappers()
        {
            this.Mappers.Clear();
        }

        public void Dispose()
        {
            this.ClearMappers();
        }
    }
}