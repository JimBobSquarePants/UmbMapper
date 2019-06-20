using UmbMapper.PropertyMappers;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class LazyPublishedItemMap : UmbMapperConfig<LazyPublishedItem>
    {
        public LazyPublishedItemMap()
        {
            this.AddMap(p => p.Id).AsLazy();
            this.AddMap(p => p.Name).AsLazy();
            this.AddMap(p => p.Slug).MapFromInstance((instance, content) => instance.Name.ToLowerInvariant());
            this.AddMap(p => p.CreateDate).AsLazy();
            this.AddMap(p => p.UpdateDate).SetAlias(p => p.UpdateDate, p => p.CreateDate).AsLazy();
            this.AddMap(p => p.PlaceOrder).AsLazy().SetMapper<EnumPropertyMapper>();
            this.AddMap(p => p.PublishedInterfaceContent).AsLazy().SetMapper<UmbracoPickerPropertyMapper>();
            this.AddMap(p => p.PublishedContent).AsLazy().SetMapper<UmbracoPickerPropertyMapper>();
            this.AddMap(p => p.Image).AsLazy();
            this.AddMap(p => p.Child).AsLazy().SetMapper<UmbracoPickerPropertyMapper>();
        }
    }
}
