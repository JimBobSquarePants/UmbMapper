using System;
using System.Collections.Generic;
using UmbMapper.Extensions;
using UmbMapper.Proxy;
using UmbMapper.Tests.Mapping.Models;
using UmbMapper.Tests.Mocks;
using Umbraco.Core.Models;
using Xunit;

namespace UmbMapper.Tests.Mapping
{
    public class LazyMappingTests : IClassFixture<UmbracoSupport>
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
                new MockPublishedContentProperty(nameof(PublishedItem.PlaceOrder), PlaceOrder.Fourth)
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
