// <copyright file="UmbMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace UmbMapper
{
    /// <summary>
    /// The registry for mapper configurations
    /// </summary>
    public static class UmbMapper
    {
        /// <summary>
        /// Gets the collection of mappers
        /// </summary>
        internal static ConcurrentDictionary<Type, IUmbMapperConfig> Mappers { get; } = new ConcurrentDictionary<Type, IUmbMapperConfig>();

        /// <summary>
        /// Gets a readonly collection of the registered mappers
        /// </summary>
        /// <returns>The <see cref="IReadOnlyCollection{T}"/></returns>
        public static IReadOnlyCollection<IUmbMapperConfig> CurrentMappers() => Mappers.Values.ToArray();

        /// <summary>
        /// Adds the mapper configuration to the mapping registry
        /// </summary>
        /// <param name="config">The mapper configuration</param>
        public static void AddMapper(IUmbMapperConfig config)
        {
            if (Mappers.ContainsKey(config.MappedType))
            {
                return;
            }

            config.Init();
            Mappers.TryAdd(config.MappedType, config);
        }

        /// <summary>
        /// Creates a mapper for the given type, adding that mapper to the mapping registry
        /// </summary>
        /// <remarks>Any properties marked <code>virtual</code> are automatically lazy mapped.</remarks>
        /// <typeparam name="T">The type of object to map</typeparam>
        public static void AddMapperFor<T>()
            where T : class, new()
        {
            if (Mappers.ContainsKey(typeof(T)))
            {
                return;
            }

            var config = new UmbMapperConfig<T>();
            config.MapAllWritable().ForEach(x => x.AsAutoLazy());
            ((IUmbMapperConfig)config).Init();

            Mappers.TryAdd(config.MappedType, config);
        }

        /// <summary>
        /// Clears the mappers from the registry
        /// </summary>
        public static void ClearMappers()
        {
            Mappers.Clear();
        }
    }
}