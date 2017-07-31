// <copyright file="UrlHelpers.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Web;
using UmbMapper.Sample.Extensions;
using Umbraco.Web;

namespace UmbMapper.Sample.Helpers
{
    /// <summary>
    /// Provides helper methods for parsing urls.
    /// </summary>
    public static class UrlHelpers
    {
        /// <summary>
        /// Gets the absolute url for the Umbraco content item.
        /// </summary>
        /// <param name="id">The id of the content to look for</param>
        /// <returns><see cref="string"/>.</returns>
        public static string AbsoluteUmbracoContentUrl(int id)
        {
            string url = new UmbracoHelper(UmbracoContext.Current).UrlAbsolute(id);

            // Certain virtual pages such as Articulate blog pages only return a relative url.
            if (!url.IsAbsoluteUrl())
            {
                string root = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                url = new Uri(new Uri(root, UriKind.Absolute), url).ToString();
            }

            return url;
        }

        /// <summary>
        /// Gets the absolute url for the Umbraco media item.
        /// </summary>
        /// <param name="src">The id of the content to look for</param>
        /// <returns><see cref="string"/>.</returns>
        public static string AbsoluteUmbracoMediaUrl(string src)
        {
            string url = src;
            if (!url.IsAbsoluteUrl())
            {
                string root = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                url = new Uri(new Uri(root, UriKind.Absolute), src).ToString();
            }

            return url;
        }
    }
}