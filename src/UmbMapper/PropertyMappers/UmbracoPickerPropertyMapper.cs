// <copyright file="UmbracoPickerPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UmbMapper.Extensions;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Maps any properties that use pickers in the backoffice.
    /// This mapper is only required when using Umbraco prior to version 7.6
    /// </summary>
    public class UmbracoPickerPropertyMapper : PropertyMapperBase
    {
        private static readonly int[] Empty = new int[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPickerPropertyMapper"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        public UmbracoPickerPropertyMapper(PropertyMapInfo info)
            : base(info)
        {
        }

        /// <inheritdoc/>
        public override object Map(IPublishedContent content, object value)
        {
            if (value is null)
            {
                return Enumerable.Empty<object>();
            }

            // Single IPublishedContent
            if (value is IPublishedContent published)
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

            PropertyMapInfo info = this.Info;
            CultureInfo culture = this.GetRequestCulture();
            int[] nodeIds = Empty;

            // First try enumerable strings, ints.
            if (type.IsGenericType || type.IsArray)
            {
                if (type.IsEnumerableOfType(typeof(string)))
                {
                    nodeIds = SelectNodeIds((IEnumerable<string>)value, culture);
                }

                if (type.IsEnumerableOfType(typeof(int)))
                {
                    nodeIds = type.IsArray ? (int[])value : ((IEnumerable<int>)value).ToArray();
                }
            }

            // Now csv strings.
            if (nodeIds.Length == 0)
            {
                string s = value as string ?? value.ToString();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    nodeIds = XmlHelper.CouldItBeXml(s)
                        ? s.GetXmlIds()
                        : SelectNodeIds(s.ToDelimitedList(), culture);
                }
            }

            if (nodeIds.Length > 0)
            {
                UmbracoObjectTypes objectType = UmbracoObjectTypes.Unknown;
                var multiPicker = new List<IPublishedContent>();

                for (int i = 0; i < nodeIds.Length; i++)
                {
                    int nodeId = nodeIds[i];
                    if (nodeId > -1)
                    {
                        IPublishedContent item = this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Document, this.UmbracoContext.ContentCache.GetById)
                        ?? this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Media, this.UmbracoContext.MediaCache.GetById)
                        ?? this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Member, this.Members.GetById);

                        if (item != null)
                        {
                            multiPicker.Add(item);
                        }
                    }
                }

                return multiPicker;
            }

            return info.DefaultValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int[] SelectNodeIds(IEnumerable<string> values, CultureInfo culture)
        {
            int length = values.Count();
            int[] result = new int[length];
            int i = 0;

            foreach (string item in values)
            {
                result[i++] = int.TryParse(item, NumberStyles.Any, culture, out int n) ? n : -1;
            }

            return result;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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