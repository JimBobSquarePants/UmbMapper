using UmbMapper.PropertyMappers;

namespace UmbMapper.Tests.Mapping.Models
{
    public class LazyPublishedItemMap : MapperConfig<LazyPublishedItem>
    {
        public LazyPublishedItemMap()
        {
            this.AddMap(p => p.Id).AsLazy();
            this.AddMap(p => p.Name).AsLazy();
            this.AddMap(p => p.CreateDate).AsLazy();
            this.AddMap(p => p.UpdateDate).SetAlias(p => p.UpdateDate, p => p.CreateDate).AsLazy();
            this.AddMap(p => p.PlaceOrder).SetMapper<EnumPropertyMapper>().AsLazy();
            this.AddMap(p => p.PublishedInterfaceContent).SetMapper<UmbracoPickerPropertyMapper>().AsLazy();
            this.AddMap(p => p.PublishedContent).SetMapper<UmbracoPickerPropertyMapper>().AsLazy();
            this.AddMap(p => p.Image).AsLazy();
            this.AddMap(p => p.Child).SetMapper<UmbracoPickerPropertyMapper>().AsLazy();
        }
    }
}
