// <copyright file="UmbMapperConstants.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Reflection;

namespace UmbMapper
{
    /// <summary>
    /// Contains application wide constants
    /// </summary>
    public static class UmbMapperConstants
    {
        /// <summary>
        /// The binding flags for properties that can be mapped
        /// </summary>
        public const BindingFlags MappableFlags = BindingFlags.Public | BindingFlags.Instance;
    }
}
