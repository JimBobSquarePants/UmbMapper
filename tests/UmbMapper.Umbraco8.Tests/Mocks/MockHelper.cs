using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace UmbMapper.Umbraco8.Tests.Mocks
{
    public static class MockHelper
    {
        public static MockPublishedProperty CreateMockPublishedProperty(string alias, object value)
        {
            //var propType = CreateMockPublishedPropertyType(alias);

            return new MockPublishedProperty(alias, value);
        }

        public static MockPublishedProperty CreateMockUmbracoContentPublishedProperty(string alias, object value)
        {
            //var propType = CreateMockPublishedPropertyType(alias);

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
    }
}
