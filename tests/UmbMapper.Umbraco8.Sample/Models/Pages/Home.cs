using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbMapper.Umbraco8.Sample.Models.Pages
{
    public class Home : PublishedPage
    {
        /// <summary>
        /// Gets or sets the main body copy
        /// </summary>
        public virtual IHtmlString BodyText { get; set; }

        /// <summary>
        /// Gets or sets the image gallery
        /// </summary>
        public virtual IEnumerable<Slide> Gallery { get; set; }

        /// <summary>
        /// Gets or sets the post names.
        /// In <see cref="HomeMap"/> you'll see that we map this using the <see cref="Posts"/> property.
        /// I'm doing this so I can demonstrate mapping both individual properties and complete classes.
        /// </summary>
        public virtual IEnumerable<string> PostNames { get; set; }

        /// <summary>
        /// Gets or sets the posts
        /// </summary>
        public virtual IEnumerable<Post> Posts { get; set; }
    }
}