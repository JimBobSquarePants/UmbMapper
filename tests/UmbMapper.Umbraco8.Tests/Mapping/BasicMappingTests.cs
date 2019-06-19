using System;
using UmbMapper.Tests.Mapping.Models;
using UmbMapper.Extensions;
using UmbMapper.Umbraco8.Tests.Mocks;
using Umbraco.Tests.PublishedContent;
using Xunit;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class BasicMappingTests : IClassFixture<UmbMapperPublishedContentTests>
    {
        public BasicMappingTests(PublishedContentTests support)
        {

        }

        [Fact]
        public void MapperCanMapBaseProperties()
        {
            const int id = 999;
            const string name = "Foo";
            var created = new DateTime(2017, 1, 1);

            MockPublishedContent content = new MockPublishedContent();
            content.Id = id;
            content.Name = name;
            content.CreateDate = created;

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.Equal(id, result.Id);
            Assert.Equal(name, result.Name);
            Assert.Equal(created, result.CreateDate);
        }
    }
}
