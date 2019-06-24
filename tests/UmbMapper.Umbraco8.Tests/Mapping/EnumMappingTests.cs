using System.Collections.Generic;
using System.Linq;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mocks;
using UmbMapper.Extensions;
using Xunit;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using System.Reflection;
using Moq;
using Umbraco.Tests.TestHelpers;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Web.PublishedCache;
using Umbraco.Core.Configuration;
using Umbraco.Web.Security;
using Umbraco.Core.Services;
using Umbraco.Web.Routing;
using Umbraco.Tests.Testing.Objects.Accessors;
using System.Globalization;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class EnumMappingTests : BaseUmbracoMappingTest, IClassFixture<UmbracoSupport>
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
                Mocks.UmbMapperMockFactory.CreateMockPublishedProperty(nameof(PublishedItem.PlaceOrder), null)
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
                Mocks.UmbMapperMockFactory.CreateMockPublishedProperty(nameof(PublishedItem.PlaceOrder), (int)placeOrder)
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
                Mocks.UmbMapperMockFactory.CreateMockPublishedProperty(nameof(PublishedItem.PlaceOrder), placeOrder.ToString())
            };

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.Equal(placeOrder, result.PlaceOrder);
        }

        /// <summary>
        /// Trying to create an umbraco context
        /// Umbraco.Tests.TestHelpers.TestObjects.GetUmbracoContextMock()
        /// </summary>
        [Fact]
        public void UmbracoContextNotNull()
        {
            // Get the internal constructor
            var constructor = typeof(UmbracoContext)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();

            var ctorParams = constructor.GetParameters();

            // build required parameters
            var _httpContextFactory = new FakeHttpContextFactory("~/Home");
            var umbracoSettings = UmbMapperMockFactory.GetUmbracoSettings();// Mock.Of<IUmbracoSettingsSection>();
            var globalSettings = Mock.Of<IGlobalSettings>();
            var publishedSnapshotService = new Mock<IPublishedSnapshotService>();
            publishedSnapshotService.Setup(x => x.CreatePublishedSnapshot(It.IsAny<string>())).Returns(Mock.Of<IPublishedSnapshot>());
            var ctxMock = new Mock<UmbracoContext>();

            var instance =
                constructor.Invoke(
                    new object[] {
                        _httpContextFactory.HttpContext,
                        publishedSnapshotService.Object,
                        new WebSecurity(_httpContextFactory.HttpContext, Mock.Of<IUserService>(), globalSettings),
                        umbracoSettings,
                        Enumerable.Empty<IUrlProvider>(),
                        globalSettings,
                        new TestVariationContextAccessor()
                    }
                );

            var umbracoContext = instance as UmbracoContext;

            var publishedRequestCtor = typeof(PublishedRequest)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();

            var publishedRequestInstance =
                publishedRequestCtor.Invoke(
                    new object[] {
                        Mock.Of<IPublishedRouter>(),
                        umbracoContext,
                        null
                    });

            umbracoContext.PublishedRequest = publishedRequestInstance as PublishedRequest;
            umbracoContext.PublishedRequest.Culture = new CultureInfo("en-US");

            var contextAccessor = new Mock<IUmbracoContextAccessor>();
            contextAccessor.Setup(x => x.UmbracoContext).Returns(umbracoContext);

            Umbraco.Web.Composing.Current.UmbracoContextAccessor = contextAccessor.Object;

            var testCulture = Umbraco.Web.Composing.Current.UmbracoContext.PublishedRequest.Culture;

            var culture = umbracoContext.PublishedRequest?.Culture;

            Assert.NotNull(culture);
        }
    }
}