using System;
using UmbMapper.Umbraco8.Tests.Mocks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.Web.Models;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class LazyPublishedItem
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Slug { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual DateTime UpdateDate { get; set; }

        public virtual PlaceOrder PlaceOrder { get; set; }

        public virtual IPublishedContent PublishedInterfaceContent { get; set; }

        public virtual MockPublishedContent PublishedContent { get; set; }

        public virtual ImageCropperValue Image { get; set; }

        public virtual PublishedItem Child { get; set; }
    }
}
