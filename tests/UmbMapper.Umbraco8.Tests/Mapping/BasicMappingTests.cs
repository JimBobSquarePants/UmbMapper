using System;
using UmbMapper.Tests.Mapping.Models;
using UmbMapper.Extensions;
using UmbMapper.Umbraco8.Tests.Mocks;
using Xunit;
using Umbraco.Web.Models;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class BasicMappingTests : IClassFixture<UmbracoSupport>
    {
        private readonly UmbracoSupport support;

        public BasicMappingTests(UmbracoSupport support)
        {
            this.support = support;
        }

        [Fact]
        public void MapperCanMapBaseProperties()
        {
            const int id = 999;
            const string name = "Foo";
            var created = new DateTime(2017, 1, 1);

            MockPublishedContent content = this.support.Content;
            content.Id = id;
            content.Name = name;
            content.CreateDate = created;

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.Equal(id, result.Id);
            Assert.Equal(name, result.Name);
            Assert.Equal(created, result.CreateDate);
        }

        [Fact]
        public void MapperReturnsDefaultProperties()
        {
            const int id = default(int);
            const string name = default(string);
            var created = default(DateTime);
            var updated = default(DateTime);

            MockPublishedContent content = this.support.Content;
            content.Id = id;
            content.Name = name;
            content.CreateDate = created;

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.Equal(id, result.Id);
            Assert.Equal(name, result.Name);
            Assert.Equal(created, result.CreateDate);
            Assert.Equal(updated, result.UpdateDate);
        }

        [Fact]
        public void MapperCanMapBaseAlternativeProperties()
        {
            var created = new DateTime(2017, 1, 1);

            MockPublishedContent content = this.support.Content;
            content.CreateDate = created;

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.Equal(created, result.CreateDate);
            Assert.Equal(created, result.UpdateDate);
        }

        [Fact]
        public void MapperCanMapLinks()
        {
            MockPublishedContent content = this.support.Content;

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.NotNull(result.Link);
            Assert.NotNull(result.Links);
            Assert.True(result.Links.GetType().IsEnumerableOfType(typeof(Link)));
        }
    }
}
