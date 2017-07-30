// <copyright file="FactoryPropertyMapperBase.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using UmbMapper.Extensions;
using UmbMapper.Invocations;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// The base class for all factory property mappers
    /// </summary>
    public abstract class FactoryPropertyMapperBase : PropertyMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryPropertyMapperBase"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        protected FactoryPropertyMapperBase(PropertyMapInfo info)
            : base(info)
        {
        }

        /// <summary>
        /// Resolves a type name based upon the current content item.
        /// </summary>
        /// <param name="content">The current published content.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public abstract string ResolveTypeName(IPublishedContent content);

        /// <inheritdoc/>
        public override object Map(IPublishedContent content, object value)
        {
            Type propType = this.PropertyType;
            bool propTypeIsEnumerable = propType.IsEnumerableType();
            Type baseType = propTypeIsEnumerable ? propType.GetEnumerableType() : propType;

            var types = (IEnumerable<Type>)ApplicationContext.Current.ApplicationCache.StaticCache.GetCacheItem("DittoFactoryAttribute_ResolveTypes_" + baseType.AssemblyQualifiedName, () =>
            {
                // Workaround for http://issues.umbraco.org/issue/U4-9011
                if (baseType.Assembly.IsAppCodeAssembly())
                {
                    // This logic is taken from the core type finder so it should be performing the same checks
                    return baseType.Assembly
                        .GetTypes()
                        .Where(t => baseType.IsAssignableFrom(t)
                                    && t.IsClass
                                    && !t.IsAbstract
                                    && !t.IsSealed
                                    && !t.IsNestedPrivate
                                    && t.GetCustomAttribute<HideFromTypeFinderAttribute>(true) == null)
                        .ToArray();
                }

                return PluginManagerInvocations.ResolveTypes(baseType);
            });

            // Check for IEnumerable<IPublishedContent> value
            var enumerableValue = value as IEnumerable<IPublishedContent>;
            if (enumerableValue != null)
            {
                IEnumerable<object> items = enumerableValue.Select(x =>
                {
                    string typeName = this.ResolveTypeName(x);
                    Type type = types.FirstOrDefault(y => y.Name.InvariantEquals(typeName));

                    return type != null ? x.MapTo(type) : null;
                });

                return EnumerableInvocations.Cast(baseType, items);
            }

            // Check for IPublishedContent value
            var ipublishedContentValue = value as IPublishedContent;
            if (ipublishedContentValue != null)
            {
                string typeName = this.ResolveTypeName(ipublishedContentValue);
                Type type = types.FirstOrDefault(y => y.Name.InvariantEquals(typeName));
                return type != null ? ipublishedContentValue.MapTo(type) : null;
            }

            // No other possible options
            return this.DefaultValue;
        }
    }
}