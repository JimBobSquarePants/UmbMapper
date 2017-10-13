using UmbMapper.PropertyMappers;

namespace UmbMapper.Tests.Mapping.Models
{
    public class CsvPublishedItemMap : UmbMapperConfig<CsvPublishedItem>
    {
        public CsvPublishedItemMap()
        {
            this.MapAll().ForEach(m => m.SetMapper<CsvPropertyMapper>());
        }
    }
}
