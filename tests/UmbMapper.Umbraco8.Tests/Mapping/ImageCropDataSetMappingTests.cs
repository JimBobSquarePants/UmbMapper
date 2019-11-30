using Umbraco.Web;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Extensions;
using Xunit;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class ImageCropDataSetMappingTests : BaseMappingTest, IClassFixture<UmbracoSupport>
    {
        public ImageCropDataSetMappingTests(UmbracoSupport support) : base(support)
        {
            this.support.InitFactoryMappers(this.umbMapperInitialiser);
        }

        [Fact]
        public void MapperReturnsImageCropDataSet()
        {
            var content = this.support.Content;
            PublishedItem result = this.umbMapperService.MapTo<PublishedItem>(content);

            Assert.NotNull(result);
            Assert.NotNull(result.Image);
            Assert.Equal(this.support.Content.Value(nameof(PublishedItem.Image)), result.Image);
        }
    }
}
