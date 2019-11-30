using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UmbMapper.Umbraco8.Sample.Models.Media;

namespace UmbMapper.Umbraco8.Sample.Models.Pages
{
    public interface IMeta
    {
        /// <summary>
        /// Gets or sets the website title that will be displayed to search engines and in the browser tab.
        /// </summary>
        string BrowserWebsiteTitle { get; set; }

        /// <summary>
        /// Gets or sets the page title that will be displayed to search engines and in the browser tab.
        /// </summary>
        string BrowserPageTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the title order should be switched. Defaults to page | website.
        /// </summary>
        bool? SwitchTitleOrder { get; set; }

        /// <summary>
        /// Gets or sets the page description search engine results. Optimum 150 - 160 characters.
        /// </summary>
        string BrowserDescription { get; set; }

        /// <summary>
        /// Gets or sets the title of the document as it should appear when shared in social media, <example>Zoombraco</example>.
        /// </summary>
        string OpenGraphTitle { get; set; }

        /// <summary>
        /// Gets or sets the URL to the image that represents your document when shared in social media.
        /// <remarks>
        /// The Open Graph protocol expects the full qualified URL to the image. Use <see cref="M:Image.UrlAbsolute"/>
        /// to return the correct value.
        /// </remarks>
        /// </summary>
        PublishedImage OpenGraphImage { get; set; }
    }
}