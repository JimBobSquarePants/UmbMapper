using System.Linq;
using UmbMapper.Tests.Mapping.Models;
using Xunit;

namespace UmbMapper.Tests.Mapping
{
    public class MapperConfigRegistryTests
    {
        [Fact]
        public void MapperConfigRegistryCanStoreMapper()
        {
            MapperConfigRegistry.Mappers.Add(new PublishedItemMap());
            Assert.True(MapperConfigRegistry.Mappers.Any(m => m.MappedType == typeof(PublishedItem)));
        }
    }
}
