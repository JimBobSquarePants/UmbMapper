using Umbraco.Core.Models;

namespace UmbMapper.Tests.Mocks
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

        public string PropertyTypeAlias { get; set; }

        public bool HasValue { get; set; }

        public object DataValue { get; set; }

        public object Value { get; set; }

        public object XPathValue { get; set; }
    }
}
