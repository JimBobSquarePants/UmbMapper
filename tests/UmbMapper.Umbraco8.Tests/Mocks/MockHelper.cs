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
            var propType = CreateMockPublishedPropertyType(alias);

            return new MockPublishedProperty(alias, value, propType);
        }

        public static MockPublishedPropertyType CreateMockPublishedPropertyType(string alias)
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
    }
}
