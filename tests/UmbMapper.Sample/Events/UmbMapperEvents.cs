// <copyright file="UmbMapperEvents.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.Sample.ComponentModel.Mappers;
using UmbMapper.Sample.Models.Components;
using Umbraco.Core;

namespace UmbMapper.Sample.Events
{
    /// <summary>
    /// Configures application events
    /// </summary>
    public class UmbMapperEvents : ApplicationEventHandler
    {
        /// <summary>
        /// All resolvers have been initialized but resolution is not frozen so they can be modified in this method
        /// </summary>
        /// <param name="umbracoApplication">
        /// The current <see cref="UmbracoApplicationBase"/>
        /// </param>
        /// <param name="applicationContext">
        /// The Umbraco <see cref="ApplicationContext"/> for the current application.
        /// </param>
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // Register our custom maps
            UmbMapperRegistry.AddMapper(new PublishedImageMap());
            UmbMapperRegistry.AddMapper(new HomeMap());
            UmbMapperRegistry.AddMapper(new BlogMap());
            UmbMapperRegistry.AddMapper(new PostMap());

            // The Slide document type is simple with no additional customization required.
            // This can be mapped by convention. The mapper will implicitly lazy map any virtual properties.
            UmbMapperRegistry.AddMapperFor<Slide>();
        }
    }
}