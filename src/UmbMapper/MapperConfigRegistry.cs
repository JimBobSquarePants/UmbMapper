// <copyright file="MapperConfigRegistry.cs" company="James Jackson-South">
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
    public static class MapperConfigRegistry
    {
        /// <summary>
        /// Gets the collection of mappers
        /// </summary>
        internal static ConcurrentDictionary<Type, IMapperConfig> Mappers { get; } = new ConcurrentDictionary<Type, IMapperConfig>();

        /// <summary>
        /// Gets a readonly collection of the registered mappers
        /// </summary>
        /// <returns>The <see cref="IReadOnlyCollection{T}"/></returns>
        public static IReadOnlyCollection<IMapperConfig> CurrentMappers() => Mappers.Values.ToArray();

        /// <summary>
        /// Gets the collection of mapper configurations
        /// </summary>
        /// <param name="config">The mapper configuration</param>
        public static void AddMapper(IMapperConfig config)
        {
            Mappers.TryAdd(config.MappedType, config);
        }

        /// <summary>
        /// Clears the mappers from the registry
        /// </summary>
        public static void Clear()
        {
            Mappers.Clear();
        }
    }
}