using System;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper.Umbraco8.Tests.Mocks
{
    public class MockPublishedProperty : IPublishedProperty
    {
        private readonly object _sourceValue;
        public MockPublishedProperty()
        {
            
        }

        public MockPublishedProperty(string alias, object value)
        {
            this.Alias = alias;
            this.Value = value;

            this._sourceValue = value;
        }

        public MockPublishedProperty(string alias, object value, PublishedPropertyType propertyType) : this (alias, value)
        {
            this.PropertyType = propertyType;
        }

        public string PropertyTypeAlias { get; set; }

        public bool HasValue { get; set; }

        public object DataValue { get; set; }

        public object Value { get; set; }

        public object XPathValue { get; set; }

        public PublishedPropertyType PropertyType { get; set; }

        public string Alias { get; set; }

        public object GetSourceValue(string culture = null, string segment = null)
        {
            throw new NotImplementedException();
        }

        public object GetValue(string culture = null, string segment = null)
        {
            int nodeId;
            if (int.TryParse(this.Value.ToString(), out nodeId))
            {
                try
                {
                    object convertedValue =
                        this.PropertyType.ConvertInterToObject(
                            new MockPublishedContent(),
                            Umbraco.Core.PropertyEditors.PropertyCacheLevel.Element,
                            nodeId,
                            true
                        );

                    if (convertedValue != null)
                    {
                        return convertedValue;
                    }
                }
                catch
                {
                    // We're only passing in a property value editor convertor 
                    // for published content types.  When one isn't found for the other
                    // types an exception will be thrown - in this case we can return 
                    // the current value
                }
            }

            return this.Value;
        }

        public object GetXPathValue(string culture = null, string segment = null)
        {
            throw new NotImplementedException();
        }

        bool IPublishedProperty.HasValue(string culture, string segment)
        {
            // Similar to Umbraco implementation
            // May need to be expanded to account for variants
            return this._sourceValue != null && 
                ((_sourceValue is string) == false || string.IsNullOrWhiteSpace((string)_sourceValue) == false);
        }
    }
}
