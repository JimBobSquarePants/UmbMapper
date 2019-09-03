using UmbMapper.Extensions;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mocks;
using Umbraco.Core.Models.PublishedContent;
using Xunit;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class UmbracoPickerMappingTests : BaseMappingTest, IClassFixture<UmbracoSupport>
    {
        public UmbracoPickerMappingTests(UmbracoSupport support) : base(support)
        {
            //this.support.SetupUmbracoContext();

            this.support.InitFactoryMappers(this.umbMapperInitialiser);
        }

        [Fact]
        public void UmbracoPickerProcessesIPublishedContent()
        {
            PublishedItem result = this.umbMapperService.MapTo<PublishedItem>(this.support.Content);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IPublishedContent>(result.PublishedInterfaceContent);
        }

        [Fact]
        public void UmbracoPickerProcessesPublishedContent()
        {
            PublishedItem result = this.umbMapperService.MapTo<PublishedItem>(this.support.Content);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<MockPublishedContent>(result.PublishedContent);
        }

        [Fact]
        public void UmbracoPickerProcessesMappedContent()
        {
            PublishedItem result = this.umbMapperService.MapTo<PublishedItem>(this.support.Content);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<PublishedItem>(result.Child);
        }
    }
}
