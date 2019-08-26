// <copyright file="PublishedContentExtensions.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UmbMapper.Invocations;
using Umbraco.Core.Models.PublishedContent;

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
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            // TODO
            // Done to get project compiling and initial tests running
            var mapperRegistry = DependencyResolver.Current.GetService<IUmbMapperRegistry>();
            mapperRegistry.Mappers.TryGetValue(type, out IUmbMapperConfig mapper);
            IUmbMapperService mappingService = DependencyResolver.Current.GetService<IUmbMapperService>();

            //UmbMapperRegistry.Mappers.TryGetValue(type, out IUmbMapperConfig mapper);

            if (mapper is null)
            {
                throw new InvalidOperationException($"No mapper for the given type {type} has been registered.");
            }

            var mappingProcessor = mapper.CreateProcessor(mappingService);

            return mappingProcessor.Map(content);
        }

        /// <summary>
        /// Performs a mapping operation from the <see cref="IPublishedContent"/> to an existing <typeparamref name="T"/> instance
        /// </summary>
        /// <typeparam name="T">The type of object to map to</typeparam>
        /// <param name="content">The content to map</param>
        /// <param name="destination">The destination object</param>
        public static void MapTo<T>(this IPublishedContent content, T destination)
            where T : class
        {
            Type type = typeof(T);
            MapTo(content, type, destination);
        }

        /// <summary>
        /// Performs a mapping operation from the <see cref="IPublishedContent"/> to an existing <see cref="object"/> instance
        /// </summary>
        /// <param name="content">The content to map</param>
        /// <param name="type">The type of object to map to</param>
        /// <param name="destination">The destination object</param>
        public static void MapTo(this IPublishedContent content, Type type, object destination)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            // TODO
            // Done to get project compiling and initial tests running
            var mapperRegistry = DependencyResolver.Current.GetService<IUmbMapperRegistry>();
            mapperRegistry.Mappers.TryGetValue(type, out IUmbMapperConfig mapper);
            IUmbMapperService mappingService = DependencyResolver.Current.GetService<IUmbMapperService>();
            //UmbMapperRegistry.Mappers.TryGetValue(type, out IUmbMapperConfig mapper);

            if (mapper is null)
            {
                throw new InvalidOperationException($"No mapper for the given type {type} has been registered.");
            }

            var mappingProcessor = mapper.CreateProcessor(mappingService);

            mappingProcessor.Map(content, destination);

            //mapper.Map(content, destination);
        }
    }
}