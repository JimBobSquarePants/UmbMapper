// <copyright file="UmbracoPickerPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UmbMapper.Extensions;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Maps any properties that use pickers in the backoffice
    /// </summary>
    public class UmbracoPickerPropertyMapper : PropertyMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPickerPropertyMapper"/> class.
        /// </summary>
        /// <param name="config">The property configuration</param>
        public UmbracoPickerPropertyMapper(PropertyMapperConfig config)
            : base(config)
        {
        }

        /// <inheritdoc/>
        public override object Map(IPublishedContent content, object value)
        {
            if (value == null)
            {
                return Enumerable.Empty<object>();
            }

            // Single IPublishedContent
            var published = value as IPublishedContent;
            if (published != null)
            {
                return published;
            }

            // ReSharper disable once PossibleNullReferenceException
            Type type = value.GetType();

            // Multiple IPublishedContent
            if (type.IsEnumerableOfType(typeof(IPublishedContent)))
            {
                return (IEnumerable<IPublishedContent>)value;
            }

            int[] nodeIds = Array.Empty<int>();

            // First try enumerable strings, ints.
            if (type.IsGenericType || type.IsArray)
            {
                if (type.IsEnumerableOfType(typeof(string)))
                {
                    int n;
                    nodeIds = ((IEnumerable<string>)value)
                        .Select(x => int.TryParse(x, NumberStyles.Any, this.Culture, out n) ? n : -1)
                        .ToArray();
                }

                if (type.IsEnumerableOfType(typeof(int)))
                {
                    nodeIds = ((IEnumerable<int>)value).ToArray();
                }
            }

            // Now csv strings.
            if (!nodeIds.Any())
            {
                string s = value as string ?? value.ToString();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    int n;
                    nodeIds = XmlHelper.CouldItBeXml(s)
                        ? s.GetXmlIds()
                        : s.ToDelimitedList()
                            .Select(x => int.TryParse(x, NumberStyles.Any, this.Culture, out n) ? n : -1)
                            .Where(x => x > 0)
                            .ToArray();
                }
            }

            if (nodeIds.Any())
            {
                UmbracoObjectTypes objectType = UmbracoObjectTypes.Unknown;
                var multiPicker = new List<IPublishedContent>();

                // Oh so ugly if you let Resharper do this.
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (int nodeId in nodeIds)
                {
                    IPublishedContent item = this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Document, this.UmbracoContext.ContentCache.GetById)
                               ?? this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Media, this.UmbracoContext.MediaCache.GetById)
                               ?? this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Member, this.Members.GetById);

                    if (item != null)
                    {
                        multiPicker.Add(item);
                    }
                }

                return multiPicker;
            }

            return this.DefaultValue;
        }

        /// <summary>
        /// Attempt to get an <see cref="IPublishedContent"/> instance based on id and object type.
        /// </summary>
        /// <param name="nodeId">The content node ID</param>
        /// <param name="actual">The type of content being requested</param>
        /// <param name="expected">The type of content expected/supported by <paramref name="typedMethod"/></param>
        /// <param name="typedMethod">A function to fetch content of type <paramref name="expected"/></param>
        /// <returns>
        /// The requested content, or null if either it does not exist or <paramref name="actual"/> does not match <paramref name="expected"/>
        /// </returns>
        private IPublishedContent GetPublishedContent(int nodeId, ref UmbracoObjectTypes actual, UmbracoObjectTypes expected, Func<int, IPublishedContent> typedMethod)
        {
            // Is the given type supported by the typed method.
            if (actual != UmbracoObjectTypes.Unknown && actual != expected)
            {
                return null;
            }

            // Attempt to get the content
            IPublishedContent content = typedMethod(nodeId);
            if (content != null)
            {
                // If we find the content, assign the expected type to the actual type so we don't have to keep looking for other types of content.
                actual = expected;
            }

            return content;
        }
    }
}