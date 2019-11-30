using UmbMapper.Models;
using UmbMapper.PropertyMappers;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models.PropertyMapDefinitions
{
    public class PublishedItemMapManyDefinition : MappingDefinition<PublishedItem>
    {
        public PublishedItemMapManyDefinition()
        {
            this.MapAll();

            this.AddMappingDefinition(p => p.UpdateDate).SetAlias(p => p.UpdateDate, p => p.CreateDate);
            this.AddMappingDefinition(p => p.PlaceOrder).SetMapper<EnumPropertyMapper>();

            this.AddMappingDefinitions(
                p => p.PublishedInterfaceContent,
                p => p.PublishedContent,
                p => p.Image,
                p => p.Child);

        }
    }
}
