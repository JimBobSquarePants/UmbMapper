using System;
using System.Collections.Generic;
using UmbMapper.Tests.Mocks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;

namespace UmbMapper.Tests.Mapping.Models
{
    public class PublishedItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public PlaceOrder PlaceOrder { get; set; }

        public IPublishedContent PublishedInterfaceContent { get; set; }

        public MockPublishedContent PublishedContent { get; set; }

        public Umbraco.Core.PropertyEditors.ValueConverters.ImageCropperValue Image { get; set; }

        public PublishedItem Child { get; set; }

        //public RelatedLink RelatedLink { get; set; }
        public Link Link { get; set; }

        //public RelatedLinks RelatedLinks { get; set; }
        public IEnumerable<Link> Links { get; set; }

        //public RelatedLinks NullRelatedLinks { get; set; }
        public IEnumerable<Link> NullLinks { get; set; }

        public IEnumerable<IPolyMorphic> Polymorphic { get; set; }
    }
}
