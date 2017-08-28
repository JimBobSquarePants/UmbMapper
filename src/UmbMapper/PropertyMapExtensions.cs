// <copyright file="PropertyMapExtensions.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;

namespace UmbMapper
{
    /// <summary>
    /// Extension methods for the <see cref="PropertyMap{T}"/> class
    /// </summary>
    public static class PropertyMapExtensions
    {
        /// <summary>
        /// Immediately executes the given action on each map in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of object to map</typeparam>
        /// <param name="source">The sequence of property mappings</param>
        /// <param name="action">The action to execute on each map.</param>
        public static void ForEach<T>(this IEnumerable<PropertyMap<T>> source, Action<PropertyMap<T>> action)
            where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (PropertyMap<T> element in source)
            {
                action(element);
            }
        }

        /// <summary>
        /// Immediately executes the given action on each map in the source sequence.
        /// Each element's index is used in the logic of the action.
        /// </summary>
        /// <typeparam name="T">The type of object to map</typeparam>>
        /// <param name="source">The sequence of property mappings</param>
        /// <param name="action">The action to execute on each map; the second parameter
        /// of the action represents the index of the source map.</param>
        public static void ForEachIndexed<T>(this IEnumerable<PropertyMap<T>> source, Action<PropertyMap<T>, int> action)
            where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            int index = 0;
            foreach (PropertyMap<T> element in source)
            {
                action(element, index++);
            }
        }
    }
}