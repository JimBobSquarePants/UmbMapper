using System.Collections.Generic;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mocks;
using Xunit;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class EnumMappingTests : BaseMappingTest, IClassFixture<UmbracoSupport>
    {
        public EnumMappingTests(UmbracoSupport support) : base (support)
        {
            // This is needed to access the culture info
            //this.support.SetupUmbracoContext();

            this.support.InitFactoryMappers(this.umbMapperInitialiser);
            
        }

        [Fact]
        public void MapperReturnsDefaultEnum()
        {
            const PlaceOrder placeOrder = PlaceOrder.Fourth;

            MockPublishedContent content = this.support.Content;
            content.Properties = new List<IPublishedProperty>
            {
                Mocks.UmbMapperMockFactory.CreateMockPublishedProperty(nameof(PublishedItem.PlaceOrder), null)
            };

            PublishedItem result = this.umbMapperService.MapTo<PublishedItem>(content);

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
                Mocks.UmbMapperMockFactory.CreateMockPublishedProperty(nameof(PublishedItem.PlaceOrder), (int)placeOrder)
            };

            PublishedItem result = this.umbMapperService.MapTo<PublishedItem>(content);

            Assert.Equal(placeOrder, result.PlaceOrder);
        }

        [Fact]
        public void MapperReturnsCorrectEnumFromString()
        {
            const PlaceOrder placeOrder = PlaceOrder.Fourth;

            MockPublishedContent content = this.support.Content;
            content.Properties = new List<IPublishedProperty>
            {
                Mocks.UmbMapperMockFactory.CreateMockPublishedProperty(nameof(PublishedItem.PlaceOrder), placeOrder.ToString())
            };

            PublishedItem result = this.umbMapperService.MapTo<PublishedItem>(content);

            Assert.Equal(placeOrder, result.PlaceOrder);
        }
    }
}