using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace UmbMapper.Umbraco8.Tests.Mocks
{
    public class MockPublishedPropertyType : PublishedPropertyType
    {
        // not sure if this is the best implementation
        private string _alias;

        public new string Alias => _alias;

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
            //this._alias = propertyType.Alias;
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
            //this._alias = propertyTypeAlias;
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
            //this._alias = propertyTypeAlias;
        }
    }
}
