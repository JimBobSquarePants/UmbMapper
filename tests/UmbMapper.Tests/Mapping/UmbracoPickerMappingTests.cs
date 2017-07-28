using UmbMapper.Extensions;
using UmbMapper.Tests.Mapping.Models;
using UmbMapper.Tests.Mocks;
using Umbraco.Core.Models;
using Xunit;

namespace UmbMapper.Tests.Mapping
{
    public class UmbracoPickerMappingTests : IClassFixture<UmbracoSupport>
    {
        private readonly UmbracoSupport support;

        public UmbracoPickerMappingTests(UmbracoSupport support)
        {
            this.support = support;
        }

        [Fact]
        public void UmbracoPickerProcessesIPublishedContent()
        {
            PublishedItem result = this.support.Content.MapTo<PublishedItem>();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IPublishedContent>(result.PublishedInterfaceContent);
        }

        [Fact]
        public void UmbracoPickerProcessesPublishedContent()
        {
            PublishedItem result = this.support.Content.MapTo<PublishedItem>();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<MockPublishedContent>(result.PublishedContent);
        }

        [Fact]
        public void UmbracoPickerProcessesMappedContent()
        {
            PublishedItem result = this.support.Content.MapTo<PublishedItem>();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<PublishedItem>(result.Child);
        }
    }
}
