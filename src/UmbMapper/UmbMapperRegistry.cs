// <copyright file="UmbMapperRegistry.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace UmbMapper
{
    public interface IUmbMapperRegistry
    {
        ConcurrentDictionary<Type, IUmbMapperConfig> Mappers { get; }
        IEnumerable<Type> CurrentMappedTypes();

        void AddMapperFor<T>()
            where T : class;

        void AddMapper(IUmbMapperConfig config);
    }

    /// <summary>
    /// The registry for mapper configurations
    /// </summary>
    public class UmbMapperRegistry : IUmbMapperRegistry, IDisposable
    {
        private readonly IUmbracoContextFactory umbracoContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbMapperRegistry"/> class.
        /// </summary>
        /// <param name="umbracoContextFactory">Umbraco Context factory</param>
        public UmbMapperRegistry(IUmbracoContextFactory umbracoContextFactory)
        {
            this.umbracoContextFactory = umbracoContextFactory;
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

            config.Init(this.umbracoContextFactory);
            this.Mappers.TryAdd(config.MappedType, config);
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
            ((IUmbMapperConfig)config).Init(this.umbracoContextFactory);

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