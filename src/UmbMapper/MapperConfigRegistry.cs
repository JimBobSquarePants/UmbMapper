// <copyright file="MapperConfigRegistry.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Collections.Generic;

namespace UmbMapper
{
    /// <summary>
    /// The registry for mapper configurations
    /// </summary>
    public static class MapperConfigRegistry
    {
        /// <summary>
        /// Gets the collection of mapper configurations
        /// </summary>
        public static List<IMapperConfig> Mappers { get; } = new List<IMapperConfig>();
    }
}
