// <copyright file="UmbMapperConfigStatics.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace UmbMapper
{
    /// <summary>
    /// Provides a various static objects used to avoid creating multiple items for
    /// the generic mapping class.
    /// </summary>
    internal static class UmbMapperConfigStatics
    {
        /// <summary>
        /// The object to lock against when initializing maps.
        /// </summary>
        public static readonly object Locker = new object();

        /// <summary>
        /// An empty enumerable object to compare against
        /// </summary>
        public static readonly IEnumerable<object> Empty = Enumerable.Empty<object>();
    }
}