using UmbMapper.PropertyMappers;

namespace UmbMapper.Tests.Mapping.Models
{
    public class BackedPublishedItemMap : UmbMapperConfig<BackedPublishedItem>
    {
        public BackedPublishedItemMap()
        {
            this.AddMap(p => p.Slug).MapFromInstance(item => item.Name.ToLowerInvariant());
            this.AddMap(p => p.PlaceOrder).SetMapper<EnumPropertyMapper>().AsLazy();
            this.AddMap(p => p.PublishedInterfaceContent).SetMapper<UmbracoPickerPropertyMapper>().AsLazy();
            this.AddMap(p => p.PublishedContent).SetMapper<UmbracoPickerPropertyMapper>().AsLazy();
            this.AddMap(p => p.Image).AsLazy();
            this.AddMap(p => p.Child).SetMapper<UmbracoPickerPropertyMapper>().AsLazy();
        }
    }
}
