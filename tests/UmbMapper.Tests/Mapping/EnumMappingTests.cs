using System.Collections.Generic;
using UmbMapper.Extensions;
using UmbMapper.Tests.Mapping.Models;
using UmbMapper.Tests.Mocks;
using Umbraco.Core.Models;
using Xunit;

namespace UmbMapper.Tests.Mapping
{
    public class EnumMappingTests : IClassFixture<UmbracoSupport>
    {
        private readonly UmbracoSupport support;

        public EnumMappingTests(UmbracoSupport support)
        {
            this.support = support;
        }

        [Fact]
        public void MapperReturnsDefaultEnum()
        {
            const PlaceOrder placeOrder = PlaceOrder.Fourth;

            MockPublishedContent content = this.support.Content;
            content.Properties = new List<IPublishedProperty>
            {
                new MockPublishedContentProperty(nameof(PublishedItem.PlaceOrder), null)
            };

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.NotEqual(placeOrder, result.PlaceOrder);
            Assert.Equal(default(PlaceOrder), result.PlaceOrder);
        }

        [Fact]
        public void MapperReturnsCorrectEnumFromInt()
        {
            const PlaceOrder placeOrder = PlaceOrder.Fourth;

            MockPublishedContent content = this.support.Content;
            content.Properties = new List<IPublishedProperty>
            {
                new MockPublishedContentProperty(nameof(PublishedItem.PlaceOrder), (int)placeOrder)
            };

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.Equal(placeOrder, result.PlaceOrder);
        }

        [Fact]
        public void MapperReturnsCorrectEnumFromString()
        {
            const PlaceOrder placeOrder = PlaceOrder.Fourth;

            MockPublishedContent content = this.support.Content;
            content.Properties = new List<IPublishedProperty>
            {
                new MockPublishedContentProperty(nameof(PublishedItem.PlaceOrder), placeOrder.ToString())
            };

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.Equal(placeOrder, result.PlaceOrder);
        }
    }
}