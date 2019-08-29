using System;
using System.Collections.Generic;
using UmbMapper.Models;

namespace UmbMapper.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="PropertyMapDefinition{T}"/> class
    /// </summary>
    public static class PropertyMapDefinitionExtensions
    {
        /// <summary>
        /// Immediately executes the given action on each map in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of object to map</typeparam>
        /// <param name="source">The sequence of property mappings</param>
        /// <param name="action">The action to execute on each map.</param>
        public static void ForEach<T>(this IEnumerable<PropertyMapDefinition<T>> source, Action<PropertyMapDefinition<T>> action)
            where T : class
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (PropertyMapDefinition<T> element in source)
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
        public static void ForEachIndexed<T>(this IEnumerable<PropertyMapDefinition<T>> source, Action<PropertyMapDefinition<T>, int> action)
            where T : class
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            int index = 0;
            foreach (PropertyMapDefinition<T> element in source)
            {
                action(element, index++);
            }
        }
    }
}
