using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace UmbMapper.Umbraco8.Tests.Mocks
{
    public class MockPublishedPropertyType : PublishedPropertyType
    {
        public MockPublishedPropertyType(
            PublishedContentType contentType,
            PropertyType propertyType,
            PropertyValueConverterCollection propertyValueConverters,
            IPublishedModelFactory publishedModelFactory,
            IPublishedContentTypeFactory factory) :
            base(
                contentType,
                propertyType,
                 propertyValueConverters,
                 publishedModelFactory,
                 factory
            )

        {
            
        }
        public MockPublishedPropertyType(
            string propertyTypeAlias,
            int dataTypeId,
            bool isUserProperty,
            ContentVariation variations,
            PropertyValueConverterCollection propertyValueConverters,
            IPublishedModelFactory publishedModelFactory,
            IPublishedContentTypeFactory factory
            ) :
            base(
                propertyTypeAlias,
                dataTypeId,
                isUserProperty,
                variations,
                propertyValueConverters,
                publishedModelFactory,
                factory
            )
        {
            
        }
        public MockPublishedPropertyType(
            PublishedContentType contentType,
            string propertyTypeAlias,
            int dataTypeId,
            bool isUserProperty,
            ContentVariation variations,
            PropertyValueConverterCollection propertyValueConverters,
            IPublishedModelFactory publishedModelFactory,
            IPublishedContentTypeFactory factory) :
            base(
                contentType,
                propertyTypeAlias,
                dataTypeId,
                isUserProperty,
                variations,
                propertyValueConverters,
                publishedModelFactory,
            factory)
        {
            
        }

        
    }
}
