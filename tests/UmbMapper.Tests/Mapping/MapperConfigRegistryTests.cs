using System.Collections.Generic;
using System.Linq;
using UmbMapper.PropertyMappers;
using UmbMapper.Tests.Mapping.Models;
using Xunit;

namespace UmbMapper.Tests.Mapping
{
    public class MapperConfigRegistryTests
    {
        [Fact]
        public void MapperConfigRegistryCanStoreMapper()
        {
            UmbMapper.AddMapper(new PublishedItemMap());
            Assert.True(UmbMapper.CurrentMappers().Any(m => m.MappedType == typeof(PublishedItem)));
        }

        [Fact]
        public void MapperConfigCanMapAll()
        {
            var mapper = new PublishedItemMapAll();
            Assert.True(mapper.Mappings.Any());
        }

        [Fact]
        public void MapperConfigCanMapMany()
        {
            var mapper = new PublishedItemMapMany();
            IReadOnlyCollection<PropertyMap<PublishedItem>> currentMaps = mapper.Mappings;
            int currentCount = currentMaps.Count;

            Assert.False(mapper.Mappings.Any(m => m.PropertyMapper is UmbracoPickerPropertyMapper));

            IEnumerable<PropertyMap<PublishedItem>> maps = mapper.AddMappings(
                  p => p.PublishedInterfaceContent,
                  p => p.PublishedContent,
                  p => p.Image,
                  p => p.Child);

            Assert.True(maps.Count() == 4);

            Assert.True(mapper.Mappings.Count == currentCount + 4);

            maps.ForEach(x => x.SetMapper<UmbracoPickerPropertyMapper>());

            Assert.True(mapper.Mappings.Any(m => m.PropertyMapper is UmbracoPickerPropertyMapper));
        }
    }
}