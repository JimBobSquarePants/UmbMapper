using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmbMapper.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mocks;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors.ValueConverters;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class BackedPublishedItem : PublishedContentModel
    {
        public BackedPublishedItem(IPublishedContent content)
            : base(content)
        {
        }

        public virtual string Slug { get; set; }

        public virtual PlaceOrder PlaceOrder { get; set; }

        public virtual IPublishedContent PublishedInterfaceContent { get; set; }

        public virtual MockPublishedContent PublishedContent { get; set; }

        public virtual ImageCropperValue Image { get; set; }

        public virtual PublishedItem Child { get; set; }
    }
}
