// <copyright file="RenderPage.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Globalization;
using UmbMapper.Sample.Models.Pages;
using Umbraco.Web;

namespace UmbMapper.Sample.RenderModels
{
    /// <summary>
    /// The render page base model for rendering pages.
    /// </summary>
    /// <typeparam name="T">The type of object to create the render model for.</typeparam>
    public class RenderPage<T> : IRenderPage<T>
        where T : PublishedPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPage{T}"/> class.
        /// </summary>
        /// <param name="content">The <see cref="System.Type"/> to create the view model from.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> providing information about the specific culture.</param>
        public RenderPage(T content, CultureInfo culture)
        {
            this.Content = content;
            this.CurrentCulture = culture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPage{T}"/> class.
        /// </summary>
        /// <param name="content">
        /// The <see cref="System.Type"/> to create the view model from.</param>
        public RenderPage(T content)
        {
            this.Content = content;
            this.CurrentCulture = UmbracoContext.Current.PublishedContentRequest.Culture;
        }

        /// <inheritdoc/>
        public T Content { get; }

        /// <inheritdoc/>
        public CultureInfo CurrentCulture { get; }
    }
}