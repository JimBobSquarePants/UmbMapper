﻿// <copyright file="DocTypeFactoryPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Resolves the type based on its doctype alias
    /// </summary>
    public class DocTypeFactoryPropertyMapper : FactoryPropertyMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocTypeFactoryPropertyMapper"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        public DocTypeFactoryPropertyMapper(PropertyMapInfo info, IUmbMapperRegistry umbMapperRegistry, IUmbMapperService umbMapperService)
            : base(info, umbMapperRegistry, umbMapperService)
        {
        }

        public DocTypeFactoryPropertyMapper(PropertyMapInfo info, IUmbMapperRegistry umbMapperRegistry, IUmbMapperService umbMapperService, IUmbracoContextFactory umbracoContextFactory)
            : base(info, umbMapperRegistry, umbMapperService, umbracoContextFactory)
        {
        }

        /// <inheritdoc/>
        public override string ResolveTypeName(IPublishedElement content) => content.ContentType.Alias;
    }
}