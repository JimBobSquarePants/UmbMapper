using UmbMapper.Extensions;
using UmbMapper.Proxy;
using UmbMapper.Tests.Mapping.Models;
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
    }
}
