// <copyright file="MapperConfigLocker.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace UmbMapper
{
    /// <summary>
    /// Provides a static locker for use when initializing maps. Used to avoid creating multiple items for
    /// the generic mapping class.
    /// </summary>
    internal class MapperConfigLocker
    {
        /// <summary>
        /// The object to lock against
        /// </summary>
        public static readonly object Locker = new object();
    }
}
