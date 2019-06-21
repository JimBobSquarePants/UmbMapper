using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using Umbraco.Core;
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
            this.Children = children;
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public IEnumerable<IPublishedContent> Children { get; set; }
        public PublishedContentType ContentType { get; set; }
        public IEnumerable<IPublishedProperty> Properties { get; set; }
        public DateTime CreateDate { get; set; }

        public string UrlSegment { get; set; }

        public int SortOrder { get; set; }

        public int Level { get; set; }

        public string Path { get; set; }

        public int? TemplateId { get; set; }

        public int CreatorId { get; set; }

        public string CreatorName { get; set; }



        public int WriterId { get; set; }

        public string WriterName { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Url { get; set; }

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures { get; set; }

        public PublishedItemType ItemType { get; set; }

        public IPublishedContent Parent { get; set; }

        public Guid Key { get; set; }



        public PublishedCultureInfo GetCulture(string culture = null)
        {
            throw new NotImplementedException();
        }

        public IPublishedProperty GetProperty(string alias)
        {
            return this.GetProperty(alias, false);
        }

        public IPublishedProperty GetProperty(string alias, bool recurse)
        {
            // trying to mock what umbraco does internally
            //var mockContentProperty = MockPublishedPropertService.GetProperty(this, alias, recurse);
            return MockPublishedPropertService.GetProperty(this, alias, recurse);
            //if (mockContentProperty != null)
            //{
            //    return mockContentProperty;
            //}


            //IPublishedProperty prop = this.Properties.SingleOrDefault(p => p.PropertyType.Alias.InvariantEquals(alias));

            //if (prop == null && recurse && this.Parent != null)
            //{
            //    return null;
            //}

            //return prop;
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
