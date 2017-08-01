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
            MapperConfigRegistry.AddMapper(new PublishedItemMap());
            Assert.True(MapperConfigRegistry.CurrentMappers().Any(m => m.MappedType == typeof(PublishedItem)));
        }
    }
}
