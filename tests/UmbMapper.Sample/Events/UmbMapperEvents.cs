// <copyright file="UmbMapperEvents.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.PublishedContentModelFactory;
using UmbMapper.Sample.ComponentModel.Mappers;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;

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
            UmbMapperRegistry.AddMapper(new PublishedImageMap());
            UmbMapperRegistry.AddMapper(new SlideMap());
            UmbMapperRegistry.AddMapper(new HomeMap());

            var factory = new UmbMapperPublishedContentModelFactory();
            PublishedContentModelFactoryResolver.Current.SetFactory(factory);
        }
    }
}