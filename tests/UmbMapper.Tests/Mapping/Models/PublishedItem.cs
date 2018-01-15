using System;
using System.Collections.Generic;
using UmbMapper.Tests.Mocks;
using Umbraco.Core.Models;
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

        public ImageCropDataSet Image { get; set; }

        public PublishedItem Child { get; set; }

        public RelatedLink RelatedLink { get; set; }

        public RelatedLinks RelatedLinks { get; set; }

        public RelatedLinks NullRelatedLinks { get; set; }

        public IEnumerable<IPolyMorphic> Polymorphic { get; set; }
    }
}
