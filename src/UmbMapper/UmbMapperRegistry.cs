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
using UmbMapper.Factories;
using UmbMapper.Models;
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

        //void AddMapperFor<T>()
        //    where T : class;

        void AddMapper(IUmbMapperConfig config);

        //void AddMapper<T>(MappingDefinition<T> mappingDefinition)
        //    where T : class;

        //void AddMapper<TMapper, TDestination>()
        //    where TMapper : UmbMapperConfig<TDestination>
        //    where TDestination : class;

        IEnumerable<IUmbMapperConfig> CurrentMappers();

        T CreateEmpty<T>()
            where T : class;

        T CreateEmpty<T>(IPublishedElement content)
           where T : class;
    }

    /// <summary>
    /// The registry for mapper configurations
    /// Flatten out the nesting and put registry + factofies ins some 
    /// type of initializer?  Registry only requires these factories
    /// for creating maps - once maps are all there, we are just getting
    /// maps out of the registry, which we don't need all the factories for.
    /// Is this true for lazy maps?? 
    /// </summary>
    public class UmbMapperRegistry : IUmbMapperRegistry, IDisposable
    {
        public UmbMapperRegistry()
        {
            //this.umbMapperConfigFactory = umbMapperConfigFactory;
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

        //public void AddMapper<T>(MappingDefinition<T> mappingDefinition)
        //    where T : class
        //{
        //    // Don't re-add a mapper for this type
        //    if (this.Mappers.ContainsKey(mappingDefinition.MappedType))
        //    {
        //        return;
        //    }

        //    IUmbMapperConfig mappingConfig = this.umbMapperConfigFactory.GenerateConfig<T>(mappingDefinition);

        //    mappingConfig.Init();
        //    this.Mappers.TryAdd(mappingConfig.MappedType, mappingConfig);
        //}

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

        //public void AddMapper<TMapper, T>()
        //    where TMapper : UmbMapperConfig<T>
        //    where T : class
        //{
        //    UmbMapperConfig<T> mapperConfig = Activator.CreateInstance(typeof(TMapper)) as UmbMapperConfig<T>;

        //    this.AddMapper<TMapper, T>(mapperConfig);
        //}

        //public void AddMapper<TMapper, T>(UmbMapperConfig<T> mapperConfig)
        //    where TMapper : UmbMapperConfig<T>
        //    where T : class
        //{
        //    mapperConfig.OnMapAdded += this.Mapper_OnMapAdded;
        //    mapperConfig.OnMapsAdded += this.Mapper_OnMapsAdded;
        //    mapperConfig.OnAllMapsAdded += this.MapperConfig_OnAllMapsAdded;
        //    mapperConfig.OnAllMapsWriteableAdded += this.MapperConfig_OnAllMapsWriteableAdded;
        //    mapperConfig.OnMapIgnored += this.MapperConfig_OnMapIgnored;

        //    mapperConfig.Init();
        //    this.Mappers.TryAdd(mapperConfig.MappedType, mapperConfig);
        //}

        

        /// <summary>
        /// Creates a mapper for the given type, adding that mapper to the mapping registry
        /// </summary>
        /// <remarks>Any properties marked <code>virtual</code> are automatically lazy mapped.</remarks>
        /// <typeparam name="T">The type of object to map</typeparam>
        //public void AddMapperFor<T>()
        //    where T : class
        //{
        //    if (this.Mappers.ContainsKey(typeof(T)))
        //    {
        //        return;
        //    }

        //    var mapperConfig = new UmbMapperConfig<T>();
        //    //mapperConfig.OnAllMapsWriteableAdded += this.MapperConfig_OnAllMapsWriteableAdded;

        //    mapperConfig.MapAllWritable().ForEach(x => x.AsAutoLazy());

        //    mapperConfig.Init();
        //    this.Mappers.TryAdd(mapperConfig.MappedType, mapperConfig);
        //}

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
        public T CreateEmpty<T>(IPublishedElement content)
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