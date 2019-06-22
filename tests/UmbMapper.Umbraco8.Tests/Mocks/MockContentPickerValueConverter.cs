using System;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PublishedCache;

namespace UmbMapper.Umbraco8.Tests.Mocks
{
    public class MockContentPickerValueConverter : PropertyValueConverterBase
    {
        IPublishedContentCache _cache;

        public MockContentPickerValueConverter()
        {
            var cacheMock = new Mock<IPublishedContentCache>();
            cacheMock.Setup(c => c.GetById(It.IsAny<int>())).Returns((int nodeId) => new MockPublishedContent { Id = nodeId });
            cacheMock.Setup(c => c.GetById(It.IsAny<Guid>())).Returns(new MockPublishedContent());

            _cache = cacheMock.Object;
        }

        public override bool IsConverter(PublishedPropertyType propertyType)
            => propertyType.Alias.Equals(Umbraco.Core.Constants.PropertyEditors.Aliases.ContentPicker);

        public override Type GetPropertyValueType(PublishedPropertyType propertyType)
            => typeof(IPublishedContent);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null) return null;

            var attemptConvertInt = source.TryConvertTo<int>();
            if (attemptConvertInt.Success)
                return attemptConvertInt.Result;
            var attemptConvertUdi = source.TryConvertTo<Udi>();
            if (attemptConvertUdi.Success)
                return attemptConvertUdi.Result;
            return null;
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter == null)
                return null;

            if (propertyType.Alias != null)
            {
                IPublishedContent content;
                if (inter is int id)
                {
                    content = _cache.GetById(id);
                    if (content != null)
                        return content;
                }
                else
                {
                    var udi = inter as GuidUdi;
                    if (udi == null)
                        return null;
                    content = _cache.GetById(udi.Guid);
                    if (content != null && content.ItemType == PublishedItemType.Content)
                        return content;
                }
            }

            return inter;
        }
    }
}
