// <copyright file="FactoryPropertyMapperBase.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            bool propTypeIsEnumerable = this.IsEnumerableType;
            Type baseType = this.IsEnumerableType ? this.EnumerableParamType : propType;

            // TODO: Can this be moved to init to reduce running time?
            var types = (IEnumerable<Type>)ApplicationContext.Current.ApplicationCache.StaticCache.GetCacheItem("UmbMapperFactoryAttribute_ResolveTypes_" + baseType.AssemblyQualifiedName, () =>
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
            if (value is IEnumerable<IPublishedContent> enumerableValue)
            {
                // IEnumerable<object> items = enumerableValue.Select(x =>
                // {
                //    string typeName = this.ResolveTypeName(x);
                //    Type type = types.FirstOrDefault(y => y.Name.InvariantEquals(typeName));
                //
                //    return type != null ? x.MapTo(type) : null;
                // });
                IEnumerable<object> items = this.Select(enumerableValue, types);
                return EnumerableInvocations.Cast(baseType, items);
            }

            // Check for IPublishedContent value
            if (value is IPublishedContent ipublishedContentValue)
            {
                string typeName = this.ResolveTypeName(ipublishedContentValue);
                Type type = types.FirstOrDefault(y => y.Name.InvariantEquals(typeName));
                return type != null ? ipublishedContentValue.MapTo(type) : null;
            }

            // No other possible options
            return this.DefaultValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<object> Select(IEnumerable<IPublishedContent> content, IEnumerable<Type> types)
        {
            foreach (IPublishedContent item in content)
            {
                string typeName = this.ResolveTypeName(item);
                Type match = null;

                foreach (Type type in types)
                {
                    if (type.Name.InvariantEquals(typeName))
                    {
                        match = type;
                        break;
                    }
                }

                yield return match != null ? item.MapTo(match) : null;
            }
        }
    }
}