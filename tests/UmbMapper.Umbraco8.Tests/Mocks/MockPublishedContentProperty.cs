using System;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper.Umbraco8.Tests.Mocks
{
    public class MockPublishedContentProperty : IPublishedProperty
    {
        public MockPublishedContentProperty()
        {
        }

        public MockPublishedContentProperty(string alias, object value)
        {
            this.PropertyTypeAlias = alias;
            this.Value = value;
        }

        public MockPublishedContentProperty(string alias, object value, PublishedPropertyType propertyType)
        {
            this.PropertyTypeAlias = alias;
            this.Value = value;
            this.PropertyType = propertyType;
        }

        public string PropertyTypeAlias { get; set; }

        public bool HasValue { get; set; }

        public object DataValue { get; set; }

        public object Value { get; set; }

        public object XPathValue { get; set; }

        public PublishedPropertyType PropertyType { get; set; }

        public string Alias => this.PropertyTypeAlias;

        public object GetSourceValue(string culture = null, string segment = null)
        {
            throw new NotImplementedException();
        }

        public object GetValue(string culture = null, string segment = null)
        {
            return this.Value;
        }

        public object GetXPathValue(string culture = null, string segment = null)
        {
            throw new NotImplementedException();
        }

        bool IPublishedProperty.HasValue(string culture, string segment)
        {
            throw new NotImplementedException();
        }
    }
}
