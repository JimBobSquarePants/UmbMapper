// COMMENTED OUT - UmbMapper.PropertyMappers.UmbracoPickerPropertyMapper states that:
// This mapper is only required when using Umbraco prior to version 7.6
//using UmbMapper.Extensions;
//using UmbMapper.Umbraco8.Tests.Mapping.Models;
//using UmbMapper.Umbraco8.Tests.Mocks;
//using Umbraco.Core.Models.PublishedContent;
//using Xunit;

//namespace UmbMapper.Umbraco8.Tests.Mapping
//{
//    public class UmbracoPickerMappingTests : BaseUmbracoMappingTest IClassFixture<UmbracoSupport>
//    {
//        private readonly UmbracoSupport support;

//        public UmbracoPickerMappingTests(UmbracoSupport support)
//        {
//            this.support = support;
//        }

//        [Fact]
//        public void UmbracoPickerProcessesIPublishedContent()
//        {
//            PublishedItem result = this.support.Content.MapTo<PublishedItem>();

//            Assert.NotNull(result);
//            Assert.IsAssignableFrom<IPublishedContent>(result.PublishedInterfaceContent);
//        }

//        [Fact]
//        public void UmbracoPickerProcessesPublishedContent()
//        {
//            PublishedItem result = this.support.Content.MapTo<PublishedItem>();

//            Assert.NotNull(result);
//            Assert.IsAssignableFrom<MockPublishedContent>(result.PublishedContent);
//        }

//        [Fact]
//        public void UmbracoPickerProcessesMappedContent()
//        {
//            PublishedItem result = this.support.Content.MapTo<PublishedItem>();

//            Assert.NotNull(result);
//            Assert.IsAssignableFrom<PublishedItem>(result.Child);
//        }
//    }
//}
