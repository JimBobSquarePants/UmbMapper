using UmbMapper.PropertyMappers;

namespace UmbMapper.Tests.Mapping.Models
{
    public class PublishedItemMapMany : UmbMapperConfig<PublishedItem>
    {
        public PublishedItemMapMany()
        {
            this.AddMap(p => p.Id).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.Name).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.CreateDate).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.UpdateDate).SetAlias(p => p.UpdateDate, p => p.CreateDate).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.PlaceOrder).SetMapper<EnumPropertyMapper>();

            //this.AddMappings(
            //    p => p.PublishedInterfaceContent,
            //    p => p.PublishedContent,
            //    p => p.Image,
            //    p => p.Child).ForEach(x => x.SetMapper<UmbracoPickerPropertyMapper>());
        }
    }
}
