using System;
using System.Collections.Generic;
//using UmbMapper.Tests.Mocks;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mocks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.Web.Models;

namespace UmbMapper.Tests.Mapping.Models
{
    public class PublishedItem //: IPublishedContent//: PublishedContentWrapped
    {
        //public PublishedItem(IPublishedContent content) : base(content)
        //{

        //}
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

        //public string UrlSegment => throw new NotImplementedException();

        //public int SortOrder => throw new NotImplementedException();

        //public int Level => throw new NotImplementedException();

        //public string Path => throw new NotImplementedException();

        //public int? TemplateId => throw new NotImplementedException();

        //public int CreatorId => throw new NotImplementedException();

        //public string CreatorName => throw new NotImplementedException();

        //public int WriterId => throw new NotImplementedException();

        //public string WriterName => throw new NotImplementedException();

        //public string Url => throw new NotImplementedException();

        //public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures => throw new NotImplementedException();

        //public PublishedItemType ItemType => throw new NotImplementedException();

        //public IPublishedContent Parent => throw new NotImplementedException();

        //public IEnumerable<IPublishedContent> Children => throw new NotImplementedException();

        //public PublishedContentType ContentType => throw new NotImplementedException();

        //public Guid Key => throw new NotImplementedException();

        //public IEnumerable<IPublishedProperty> Properties => throw new NotImplementedException();

        //public PublishedCultureInfo GetCulture(string culture = null)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPublishedProperty GetProperty(string alias)
        //{
        //    throw new NotImplementedException();
        //}

        //public string GetUrl(string culture = null)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool IsDraft(string culture = null)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool IsPublished(string culture = null)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
