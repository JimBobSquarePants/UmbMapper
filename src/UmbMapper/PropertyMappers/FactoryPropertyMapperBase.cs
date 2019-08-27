// <copyright file="FactoryPropertyMapperBase.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using UmbMapper.Extensions;
using UmbMapper.Invocations;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// The base class for all factory property mappers
    /// </summary>
    public abstract class FactoryPropertyMapperBase : PropertyMapperBase
    {
        private readonly IUmbMapperRegistry umbMapperRegistry;
        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryPropertyMapperBase"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        protected FactoryPropertyMapperBase(PropertyMapInfo info, IUmbMapperRegistry umbMapperRegistry)
            : base(info)
        {
            this.umbMapperRegistry = umbMapperRegistry;
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
            PropertyMapInfo info = this.Info;
            Type propType = info.PropertyType;
            bool propTypeIsEnumerable = info.IsEnumerableType;
            Type baseType = info.IsEnumerableType ? info.EnumerableParamType : propType;
            // TODO
            // Done to get project compiling and initial tests running
            //var mapperRegistry = DependencyResolver.Current.GetService<IUmbMapperRegistry>();
            //IEnumerable<Type> types = mapperRegistry.CurrentMappedTypes();
            
            IEnumerable<Type> types = this.umbMapperRegistry.CurrentMappedTypes();

            // Check for IEnumerable<IPublishedContent> value
            if (value is IEnumerable<IPublishedContent> enumerableContentValue)
            {
                IEnumerable<object> items = this.Select(enumerableContentValue, types);
                return EnumerableInvocations.Cast(baseType, items);
            }

            // Check for IPublishedContent value
            if (value is IPublishedContent contentValue)
            {
                string typeName = this.ResolveTypeName(contentValue);
                Type type = FirstOrDefault(types, typeName);
                return type != null ? contentValue.MapTo(type) : null;
            }

            // No other possible options
            return info.DefaultValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Type FirstOrDefault(IEnumerable<Type> types, string typeName)
        {
            foreach (Type type in types)
            {
                if (type.Name.InvariantEquals(typeName))
                {
                    return type;
                }
            }

            return null;
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

        //private IUmbMapperRegistry GetMapperRegistry()
        //{
        //    var args = new MapperRegistryRequiredArgs();
        //    EventHandler<MapperRegistryRequiredArgs> handler = OnRegistryRequired;
        //    if (OnRegistryRequired != null)
        //    {
        //        OnRegistryRequired(this, args);
        //    }
        //    return args.Registry;
        //}

        //public event EventHandler<MapperRegistryRequiredArgs> OnRegistryRequired;
    }

    //public class MapperRegistryRequiredArgs : EventArgs
    //{
    //    public IUmbMapperRegistry Registry { get; set; }
    //}
}