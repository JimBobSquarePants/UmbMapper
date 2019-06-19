using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper.Umbraco8.Tests.Mocks
{
    public class MockPublishedContent : IPublishedContent
    {
        public MockPublishedContent()
        {
            this.Id = 1234;
            this.Name = "Name";
            this.Children = Enumerable.Empty<IPublishedContent>();
            this.Properties = new Collection<IPublishedProperty>();
        }

        public MockPublishedContent(
            int id,
            string name,
            IEnumerable<IPublishedContent> children,
            ICollection<IPublishedProperty> properties)
        {
            this.Properties = properties;
            this.Id = id;
            this.Name = name;
            //this.ContentType = new AutoPublishedContentType(22, "myType", new PublishedPropertyType[] { }); this.GetType().Name;
            this.Children = children;
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public IEnumerable<IPublishedContent> Children { get; set; }
        public PublishedContentType ContentType { get; set; }
        public IEnumerable<IPublishedProperty> Properties { get; set; }
        public DateTime CreateDate { get; set; }

        public string UrlSegment => throw new NotImplementedException();

        public int SortOrder => throw new NotImplementedException();

        public int Level => throw new NotImplementedException();

        public string Path => throw new NotImplementedException();

        public int? TemplateId => throw new NotImplementedException();

        public int CreatorId => throw new NotImplementedException();

        public string CreatorName => throw new NotImplementedException();

        

        public int WriterId => throw new NotImplementedException();

        public string WriterName => throw new NotImplementedException();

        public DateTime UpdateDate => throw new NotImplementedException();

        public string Url => throw new NotImplementedException();

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures => throw new NotImplementedException();

        public PublishedItemType ItemType => throw new NotImplementedException();

        public IPublishedContent Parent => throw new NotImplementedException();

        public Guid Key => throw new NotImplementedException();



        public PublishedCultureInfo GetCulture(string culture = null)
        {
            throw new NotImplementedException();
        }

        public IPublishedProperty GetProperty(string alias)
        {
            throw new NotImplementedException();
        }

        public string GetUrl(string culture = null)
        {
            throw new NotImplementedException();
        }

        public bool IsDraft(string culture = null)
        {
            throw new NotImplementedException();
        }

        public bool IsPublished(string culture = null)
        {
            throw new NotImplementedException();
        }
    }
}
