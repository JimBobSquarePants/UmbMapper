// <copyright file="PropertyInfoExtensions.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Reflection;

namespace UmbMapper.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="PropertyInfo"/>.
    /// </summary>
    internal static class PropertyInfoExtensions
    {
        /// <summary>
        /// Returns a value indicating whether a <see cref="PropertyInfo"/> is both virtual and overridable.
        /// </summary>
        /// <param name="source">The source <see cref="PropertyInfo"/>.</param>
        /// <returns>
        /// True if the <see cref="PropertyInfo"/> meets the conditions; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the given instance is null.
        /// </exception>
        public static bool IsVirtualAndOverridable(this PropertyInfo source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.CanWrite)
            {
                return false;
            }

            MethodInfo method = source.GetGetMethod();
            return method.IsVirtual && !method.IsFinal;
        }

        /// <summary>
        /// Checks to see if the given property should attempt to lazy load
        /// </summary>
        /// <param name="source">The property to check</param>
        /// <returns>True if a lazy load attempt should be make</returns>
        public static bool ShouldAttemptLazyLoad(this PropertyInfo source)
        {
            return source.IsVirtualAndOverridable();
        }
    }
}