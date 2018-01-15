// <copyright file="UmbMapperPublishedContentModelFactory.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using UmbMapper.Extensions;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace UmbMapper.PublishedContentModelFactory
{
    /// <summary>
    /// A factory for the creation of strong typed models
    /// </summary>
    public class UmbMapperPublishedContentModelFactory : IPublishedContentModelFactory
    {
        private readonly IEnumerable<IUmbMapperConfig> registeredMappers;

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbMapperPublishedContentModelFactory"/> class.
        /// </summary>
        public UmbMapperPublishedContentModelFactory()
        {
            IEnumerable<IUmbMapperConfig> mappers = UmbMapperRegistry.CurrentMappers();
            if (mappers.Count() == 0)
            {
                throw new InvalidOperationException("No mappers have been registered. Ensure that registration occures before initialization of this factory.");
            }

            this.registeredMappers = mappers.Where(m => typeof(IPublishedContent).IsAssignableFrom(m.MappedType));
        }

        /// <summary>
        /// Resolves a type name based upon the current content item.
        /// </summary>
        /// <param name="content">The current published content.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public virtual string ResolveTypeName(IPublishedContent content) => content.DocumentTypeAlias;

        /// <inheritdoc/>
        public IPublishedContent CreateModel(IPublishedContent content)
        {
            UmbracoContext context = UmbracoContext.Current;
            if (context != null && context.IsFrontEndUmbracoRequest)
            {
                string typeName = this.ResolveTypeName(content);
                IUmbMapperConfig mapper = this.registeredMappers.FirstOrDefault(m => m.MappedType.Name.InvariantEquals(typeName));

                if (mapper != null)
                {
                    return (IPublishedContent)content.MapTo(mapper.MappedType);
                }
            }

            return content;
        }
    }
}