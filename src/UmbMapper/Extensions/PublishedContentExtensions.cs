// <copyright file="PublishedContentExtensions.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using UmbMapper.Invocations;
using Umbraco.Core.Models;

namespace UmbMapper.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="IPublishedContent"/> type
    /// </summary>
    public static class PublishedContentExtensions
    {
        /// <summary>
        /// Performs a mapping operation from the <see cref="IEnumerable{IPublishedContent}"/> to a new <see cref="IEnumerable{T}"/> instance
        /// </summary>
        /// <typeparam name="T">The type of object to map to</typeparam>
        /// <param name="content">The content to map</param>
        /// <returns>
        /// The converted <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<T> MapTo<T>(this IEnumerable<IPublishedContent> content)
            where T : class
        {
            return content.MapTo(typeof(T)).Select(x => x as T);
        }

        /// <summary>
        /// Performs a mapping operation from the <see cref="IEnumerable{IPublishedContent}"/> to a new <see cref="IEnumerable{Object}"/> instance
        /// </summary>
        /// <param name="content">The content to map</param>
        /// <param name="type">The <see cref="Type"/> of items to return.</param>
        /// <returns>
        /// The converted <see cref="IEnumerable{Object}"/>.
        /// </returns>
        public static IEnumerable<object> MapTo(this IEnumerable<IPublishedContent> content, Type type)
        {
            IEnumerable<object> typedItems = content.Select(x => x.MapTo(type));

            // We need to cast back here as nothing is strong typed anymore.
            return (IEnumerable<object>)EnumerableInvocations.Cast(type, typedItems);
        }

        /// <summary>
        /// Performs a mapping operation from the <see cref="IPublishedContent"/> to a new <typeparamref name="T"/> instance
        /// </summary>
        /// <typeparam name="T">The type of object to map to</typeparam>
        /// <param name="content">The content to map</param>
        /// <returns>The <typeparamref name="T"/></returns>
        public static T MapTo<T>(this IPublishedContent content)
            where T : class
        {
            Type type = typeof(T);
            return (T)MapTo(content, type);
        }

        /// <summary>
        /// Performs a mapping operation from the <see cref="IPublishedContent"/> to a new <see cref="object"/> instance
        /// </summary>
        /// <param name="content">The content to map</param>
        /// <param name="type">The type of object to map to</param>
        /// <returns>The <see cref="object"/></returns>
        public static object MapTo(this IPublishedContent content, Type type)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            UmbMapperRegistry.Mappers.TryGetValue(type, out var mapper);

            if (mapper == null)
            {
                throw new InvalidOperationException($"No mapper for the given type {type} has been registered.");
            }

            return mapper.Map(content);
        }
    }
}