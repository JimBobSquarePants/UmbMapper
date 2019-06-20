using System.Collections.Generic;
using System.Linq;
using UmbMapper.PropertyMappers;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using Xunit;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class UmbMapperRegistryTests
    {
        [Fact]
        public void UmbMapperRegistryCanStoreMapper()
        {
            UmbMapperRegistry.AddMapper(new PublishedItemMap());
            Assert.Contains(UmbMapperRegistry.CurrentMappers(), m => m.MappedType == typeof(PublishedItem));
        }

        [Fact]
        public void UmbMapperCanMapAll()
        {
            var mapper = new PublishedItemMapAll();
            Assert.True(mapper.Mappings.Any());
        }

        [Fact]
        public void UmbMapperCanMapMany()
        {
            var mapper = new PublishedItemMapMany();
            IEnumerable<IPropertyMap> currentMaps = mapper.Mappings;
            int currentCount = currentMaps.Count();

            Assert.DoesNotContain(mapper.Mappings, m => m.PropertyMapper is UmbracoPickerPropertyMapper);

            IEnumerable<PropertyMap<PublishedItem>> maps = mapper.AddMappings(
                  p => p.PublishedInterfaceContent,
                  p => p.PublishedContent,
                  p => p.Image,
                  p => p.Child);

            Assert.True(maps.Count() == 4);

            // Original map list count is unchanged
            Assert.True(mapper.Mappings.Count() == currentCount);

            maps.ForEach(x => x.SetMapper<UmbracoPickerPropertyMapper>());

            Assert.Contains(mapper.Mappings, m => m.PropertyMapper is UmbracoPickerPropertyMapper);
            Assert.True(mapper.Mappings.Count(m => m.PropertyMapper is UmbracoPickerPropertyMapper) == 4);
        }
    }
}
