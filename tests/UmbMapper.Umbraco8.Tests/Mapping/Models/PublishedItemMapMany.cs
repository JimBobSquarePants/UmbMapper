using UmbMapper.PropertyMappers;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class PublishedItemMapMany : UmbMapperConfig<PublishedItem>
    {
        public PublishedItemMapMany()
        {
            //this.MapAll();

            //this.AddMap(p => p.UpdateDate).SetAlias(p => p.UpdateDate, p => p.CreateDate);
            //this.AddMap(p => p.PlaceOrder).SetMapper<EnumPropertyMapper>();

            //this.AddMappings(
            //    p => p.PublishedInterfaceContent,
            //    p => p.PublishedContent,
            //    p => p.Image,
            //    p => p.Child);
        }

        public override void Init()
        {
            this.MapAll();

            this.AddMap(p => p.UpdateDate).SetAlias(p => p.UpdateDate, p => p.CreateDate);
            this.AddMap(p => p.PlaceOrder).SetMapper<EnumPropertyMapper>();

            this.AddMappings(
                p => p.PublishedInterfaceContent,
                p => p.PublishedContent,
                p => p.Image,
                p => p.Child);

            base.Init();
        }
    }
}
