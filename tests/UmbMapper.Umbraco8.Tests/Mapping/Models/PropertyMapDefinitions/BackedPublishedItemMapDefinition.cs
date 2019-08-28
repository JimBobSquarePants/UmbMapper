using UmbMapper.Models;
using UmbMapper.PropertyMappers;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models.PropertyMapDefinitions
{
    public class BackedPublishedItemMapDefinition : MappingDefinition<BackedPublishedItem>
    {
        public BackedPublishedItemMapDefinition()
        {
            this.AddMappingDefinition(p => p.Slug).MapFromInstance((instance, content) => instance.Name.ToLowerInvariant());
            this.AddMappingDefinition(p => p.PlaceOrder).AsLazy().SetMapper<EnumPropertyMapper>();
            this.AddMappingDefinition(p => p.PublishedInterfaceContent).AsLazy();
            this.AddMappingDefinition(p => p.PublishedContent).AsLazy();
            this.AddMappingDefinition(p => p.Image).AsLazy();
            this.AddMappingDefinition(p => p.Child).AsLazy();
        }
    }
}
