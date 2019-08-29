using UmbMapper.Models;
using UmbMapper.PropertyMappers;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class LazyPublishedItemMapDefinition : MappingDefinition<LazyPublishedItem>
    {
        public LazyPublishedItemMapDefinition()
        {
            this.AddMappingDefinition(p => p.Id).AsLazy();
            this.AddMappingDefinition(p => p.Name).AsLazy();
            this.AddMappingDefinition(p => p.Slug).MapFromInstance((instance, content) => instance.Name.ToLowerInvariant());
            this.AddMappingDefinition(p => p.CreateDate).AsLazy();
            this.AddMappingDefinition(p => p.UpdateDate).SetAlias(p => p.UpdateDate, p => p.CreateDate).AsLazy();
            this.AddMappingDefinition(p => p.PlaceOrder).AsLazy().SetMapper<EnumPropertyMapper>();
            this.AddMappingDefinition(p => p.PublishedInterfaceContent).AsLazy(); //.SetMapper<UmbracoPickerPropertyMapper>();
            this.AddMappingDefinition(p => p.PublishedContent).AsLazy(); //.SetMapper<UmbracoPickerPropertyMapper>();
            this.AddMappingDefinition(p => p.Image).AsLazy();
            this.AddMappingDefinition(p => p.Child).AsLazy(); //.SetMapper<UmbracoPickerPropertyMapper>();
        }
    }
}
