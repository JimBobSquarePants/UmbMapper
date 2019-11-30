using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using UmbMapper.Umbraco8.Sample.Models.Pages;

namespace UmbMapper.Umbraco8.Sample.RenderModels
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
            this.CurrentCulture = Umbraco.Web.Composing.Current.UmbracoContext.PublishedRequest.Culture; // UmbracoContext.Current.PublishedContentRequest.Culture;
        }

        /// <inheritdoc/>
        public T Content { get; }

        /// <inheritdoc/>
        public CultureInfo CurrentCulture { get; }
    }
}