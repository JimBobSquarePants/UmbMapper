// <copyright file="DocTypeFactoryPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using Umbraco.Core.Models;

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
        /// <param name="config">The property configuration</param>
        public DocTypeFactoryPropertyMapper(PropertyMapperConfig config)
            : base(config)
        {
        }

        /// <inheritdoc/>
        public override string ResolveTypeName(IPublishedContent content) => content.DocumentTypeAlias;
    }
}