using UmbMapper.Extensions;
using UmbMapper.Tests.Mapping.Models;
using Umbraco.Web;
using Xunit;

namespace UmbMapper.Tests.Mapping
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
            Assert.Equal(this.support.Content.GetPropertyValue(nameof(PublishedItem.Image)), result.Image);
        }
    }
}
