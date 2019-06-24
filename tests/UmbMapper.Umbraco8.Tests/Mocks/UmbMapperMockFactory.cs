using System.Collections.Generic;
using System.Linq;
using Moq;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Routing;

namespace UmbMapper.Umbraco8.Tests.Mocks
{
    public static class UmbMapperMockFactory
    {
        public static MockPublishedProperty CreateMockPublishedProperty(string alias, object value)
        {
            return new MockPublishedProperty(alias, value);
        }

        public static MockPublishedProperty CreateMockUmbracoContentPublishedProperty(string alias, object value)
        {
            return new MockPublishedProperty(alias, value);
        }

        public static MockPublishedPropertyType CreateMockPublishedPropertyType(string alias = "test")
        {
            var mockPublishedContentTypeFactory = new Mock<IPublishedContentTypeFactory>();

            var publishedPropType = new MockPublishedPropertyType(
                alias,
                1,
                true,
                ContentVariation.CultureAndSegment,
                new PropertyValueConverterCollection(Enumerable.Empty<IPropertyValueConverter>()),
                Mock.Of<IPublishedModelFactory>(),
                mockPublishedContentTypeFactory.Object);

            return publishedPropType;
        }

        public static MockPublishedPropertyType CreateMockUmbracoContentPublishedPropertyType()
        {
            var mockPublishedContentTypeFactory = new Mock<IPublishedContentTypeFactory>();

            var publishedPropType = new MockPublishedPropertyType(
                Umbraco.Core.Constants.PropertyEditors.Aliases.ContentPicker,
                1,
                true,
                ContentVariation.CultureAndSegment,
                new PropertyValueConverterCollection(new List<IPropertyValueConverter> { new MockContentPickerValueConverter() }),
                Mock.Of<IPublishedModelFactory>(),
                mockPublishedContentTypeFactory.Object);


            return publishedPropType;
        }

        public static IUmbracoSettingsSection GetUmbracoSettings()
        {
            // FIXME: Why not use the SettingsForTest.GenerateMock ... ?
            // FIXME: Shouldn't we use the default ones so they are the same instance for each test?

            var umbracoSettingsMock = new Mock<IUmbracoSettingsSection>();
            var webRoutingSectionMock = new Mock<IWebRoutingSection>();
            webRoutingSectionMock.Setup(x => x.UrlProviderMode).Returns(UrlProviderMode.Auto.ToString());
            umbracoSettingsMock.Setup(x => x.WebRouting).Returns(webRoutingSectionMock.Object);
            return umbracoSettingsMock.Object;
        }
    }
}
