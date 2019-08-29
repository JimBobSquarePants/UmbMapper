using UmbMapper;
using UmbMapper.Extensions;
using UmbMapper.Models;
using UmbMapper.PropertyMappers;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class CsvPublishedItemMapDefinition : MappingDefinition<CsvPublishedItem>
    {
        public CsvPublishedItemMapDefinition()
        {
            // Test both lazy and no-lazy mapping.
            this.MapAll().ForEach(m => m.SetMapper<CsvPropertyMapper>());
            this.AddMappingDefinition(x => x.StringItems).AsLazy();
        }
    }
}

