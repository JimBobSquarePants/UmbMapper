// <copyright file="UmbMapperEvents.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.Sample.ComponentModel.Mappers;
using Umbraco.Core;

namespace UmbMapper.Sample.Events
{
    /// <summary>
    /// Configures application events
    /// </summary>
    public class UmbMapperEvents : ApplicationEventHandler
    {
        /// <summary>
        /// Boot-up is completed, this allows you to perform any other boot-up logic required for the application.
        /// Resolution is frozen so now they can be used to resolve instances.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The current <see cref="UmbracoApplicationBase"/>
        /// </param>
        /// <param name="applicationContext">
        /// The Umbraco <see cref="ApplicationContext"/> for the current application.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            MapperConfigRegistry.AddMapper(new PublishedImageMap());
            MapperConfigRegistry.AddMapper(new SlideMap());
            MapperConfigRegistry.AddMapper(new HomeMap());
        }
    }
}