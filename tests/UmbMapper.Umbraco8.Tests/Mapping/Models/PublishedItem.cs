using System;
using System.Collections.Generic;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mocks;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors.ValueConverters;
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

        public ImageCropperValue Image { get; set; }

        public PublishedItem Child { get; set; }

        public Link Link { get; set; }

        public IEnumerable<Link> Links { get; set; }

        public IEnumerable<Link> NullLinks { get; set; }

        public IEnumerable<IPolyMorphic> Polymorphic { get; set; }
    }
}
