// <copyright file="IRenderPage.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Globalization;
using UmbMapper.Sample.Models.Pages;

namespace UmbMapper.Sample.RenderModels
{
    /// <summary>
    /// Encapsulates properties required rendering pages with metadata.
    /// </summary>
    /// <typeparam name="T">The type of object to create the render model for.</typeparam>
    public interface IRenderPage<out T>
        where T : PublishedPage
    {
        /// <summary>
        /// Gets the content.
        /// </summary>
        T Content { get; }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        CultureInfo CurrentCulture { get; }
    }
}