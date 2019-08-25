using System.Collections.Generic;
using System.Linq;
using Moq;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using Umbraco.Web;
using Xunit;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class UmbMapperRegistryTests
    {
        [Fact]
        public void UmbMapperRegistryCanStoreMapper()
        {
            var registry = new UmbMapperRegistry(Mock.Of<IUmbracoContextFactory>());

            registry.AddMapper(new PublishedItemMap());
            Assert.Contains(registry.CurrentMappers(), m => m.MappedType == typeof(PublishedItem));
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

            IEnumerable<PropertyMap<PublishedItem>> maps = mapper.AddMappings(
                  p => p.PublishedInterfaceContent,
                  p => p.PublishedContent,
                  p => p.Image,
                  p => p.Child);

            Assert.True(maps.Count() == 4);

            // Original map list count is unchanged
            Assert.True(mapper.Mappings.Count() == currentCount);


        }
    }
}
