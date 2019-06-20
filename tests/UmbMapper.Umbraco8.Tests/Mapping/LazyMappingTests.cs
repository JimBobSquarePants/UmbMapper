using System;
using System.Collections.Generic;
using UmbMapper.Extensions;
using UmbMapper.Proxy;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mocks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Xunit;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class LazyMappingTests : BaseUmbracoMappingTest, IClassFixture<UmbracoSupport>
    {
        private readonly UmbracoSupport support;

        public LazyMappingTests(UmbracoSupport support)
        {
            this.support = support;
        }

        [Fact]
        public void MapLazyProperties()
        {
            LazyPublishedItem result = this.support.Content.MapTo<LazyPublishedItem>();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IProxy>(result);
        }

        [Fact]
        public void MapLazyFunctions()
        {
            LazyPublishedItem result = this.support.Content.MapTo<LazyPublishedItem>();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IProxy>(result);
            Assert.NotNull(result.Slug);
            Assert.True(result.Slug == result.Name.ToLowerInvariant());
        }

        [Fact]
        public void MapperCanMapToExistingInstance()
        {
            const int id = 999;
            const string name = "Foo";
            var created = new DateTime(2017, 1, 1);
            PlaceOrder placeOrder = PlaceOrder.Second;

            MockPublishedContent content = this.support.Content;
            content.Id = id;
            content.Name = name;
            content.CreateDate = created;
            content.Properties = new List<IPublishedProperty>
            {
                new MockPublishedProperty(nameof(PublishedItem.PlaceOrder), PlaceOrder.Fourth, MockHelper.CreateMockPublishedPropertyType( nameof(PublishedItem.PlaceOrder)))
            };


            LazyPublishedItem result = UmbMapperRegistry.CreateEmpty<LazyPublishedItem>();

            // Set a value before mapping.
            result.PlaceOrder = placeOrder;

            content.MapTo(result);

            Assert.Equal(id, result.Id);
            Assert.Equal(name, result.Name);
            Assert.Equal(created, result.CreateDate);

            // We expect it to be overwritten
            Assert.NotEqual(placeOrder, result.PlaceOrder);
        }
    }
}
