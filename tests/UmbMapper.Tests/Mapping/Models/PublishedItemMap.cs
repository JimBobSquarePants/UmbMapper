using UmbMapper.PropertyMappers;

namespace UmbMapper.Tests.Mapping.Models
{
    public class PublishedItemMap : MapperConfig<PublishedItem>
    {
        public PublishedItemMap()
        {
            this.AddMap(p => p.Id).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.Name).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.CreateDate).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.UpdateDate).SetAlias(p => p.UpdateDate, p => p.CreateDate).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.PlaceOrder).SetMapper<EnumPropertyMapper>();
            this.AddMap(p => p.PublishedInterfaceContent).SetMapper<UmbracoPickerPropertyMapper>();
            this.AddMap(p => p.PublishedContent).SetMapper<UmbracoPickerPropertyMapper>();
            this.AddMap(p => p.Image).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.Child).SetMapper<UmbracoPickerPropertyMapper>();
        }
    }
}
