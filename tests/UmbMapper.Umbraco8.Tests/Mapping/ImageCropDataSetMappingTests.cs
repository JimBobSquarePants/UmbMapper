using Umbraco.Web;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Extensions;
using Xunit;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class ImageCropDataSetMappingTests : IClassFixture<UmbracoSupport>
    {
        private readonly UmbracoSupport support;

        public ImageCropDataSetMappingTests(UmbracoSupport support)
        {
            this.support = support;
        }

        [Fact]
        public void MapperReturnsImageCropDataSet()
        {
            PublishedItem result = this.support.Content.MapTo<PublishedItem>();

            Assert.NotNull(result);
            Assert.NotNull(result.Image);
            Assert.Equal(this.support.Content.Value(nameof(PublishedItem.Image)), result.Image);
        }
    }
}
