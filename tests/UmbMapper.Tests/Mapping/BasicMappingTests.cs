using System;
using UmbMapper.Extensions;
using UmbMapper.Tests.Mapping.Models;
using UmbMapper.Tests.Mocks;
using Umbraco.Web;
using Umbraco.Web.Models;
using Xunit;

namespace UmbMapper.Tests.Mapping
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
        public void MapperCanMapRelatedLinks()
        {
            MockPublishedContent content = this.support.Content;

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.NotNull(result.RelatedLink);
            Assert.NotNull(result.RelatedLinks);
            Assert.True(result.RelatedLinks.GetType().IsEnumerableOfType(typeof(RelatedLink)));
        }

        [Fact]
        public void MapperCanMapAutoMappedProperties()
        {
            MockPublishedContent content = this.support.Content;
            var created = new DateTime(2017, 1, 1);
            content.Id = 98765;
            content.Name = "AutoMapped";
            content.CreateDate = created;
            content.UpdateDate = created;

            AutoMappedItem result = content.MapTo<AutoMappedItem>();

            Assert.NotNull(result);
            Assert.Equal(content.Id, result.Id);
            Assert.Equal(content.Name, result.Name);
            Assert.Equal(content.CreateDate, result.CreateDate);
            Assert.Equal(content.UpdateDate, result.UpdateDate);
        }

        [Fact]
        public void MapperCanMapPublishedModelType()
        {
            MockPublishedContent content = this.support.Content;
            var created = new DateTime(2017, 1, 1);
            content.Id = 98765;
            content.Name = "BackMapped";
            content.CreateDate = created;
            content.UpdateDate = created;

            BackedPublishedItem result = content.MapTo<BackedPublishedItem>();

            Assert.NotNull(result);
            Assert.Equal(content.Id, result.Id);
            Assert.Equal(content.Name, result.Name);
            Assert.Equal(content.CreateDate, result.CreateDate);
            Assert.Equal(content.UpdateDate, result.UpdateDate);

            Assert.NotNull(result.Slug);
            Assert.True(result.Slug == result.Name.ToLowerInvariant());
            Assert.NotNull(result.Image);
            Assert.Equal(content.GetPropertyValue(nameof(BackedPublishedItem.Image)), result.Image);
        }

        [Fact]
        public void MapperCanRemoveMap()
        {
            var map = new LazyPublishedItemMap();
            int mapCount = map.Mappings.Count;

            bool result = map.Ignore(x => x.CreateDate);

            Assert.True(result);
            Assert.Equal(mapCount - 1, map.Mappings.Count);
        }
    }
}