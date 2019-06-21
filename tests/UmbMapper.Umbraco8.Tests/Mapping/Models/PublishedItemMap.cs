using UmbMapper.PropertyMappers;
using Umbraco.Core.PropertyEditors.ValueConverters;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class PublishedItemMap : UmbMapperConfig<PublishedItem>
    {
        public PublishedItemMap()
        {
            this.AddMap(p => p.Id).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.Name).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.CreateDate).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.UpdateDate).SetAlias(p => p.UpdateDate, p => p.CreateDate).SetMapper<UmbracoPropertyMapper>();
            this.AddMap(p => p.PlaceOrder).SetMapper<EnumPropertyMapper>();
            this.AddMap(p => p.Image); // as we can mock the composition with property value editors
            this.AddMap(p => p.Link);
            this.AddMap(p => p.Links);
            this.AddMap(p => p.NullLinks);
            this.AddMap(p => p.Polymorphic).SetMapper<DocTypeFactoryPropertyMapper>();
            this.AddMap(p => p.PublishedContent);
            this.AddMap(p => p.PublishedInterfaceContent); //.SetMapper<UmbracoPickerPropertyMapper>();
            this.AddMap(p => p.Child); //.SetMapper<UmbracoPickerPropertyMapper>();
            //this.AddMap(p => p.Image).SetMapper<UmbracoPropertyMapper>();
            //this.AddMap(p => p.Child).SetMapper<UmbracoPickerPropertyMapper>();


            //this.AddMap(p => p.PublishedInterfaceContent).SetMapper<UmbracoPickerPropertyMapper>();
            //this.AddMap(p => p.Image).SetMapper<UmbracoPropertyMapper>();
            //this.AddMap(p => p.Child).SetMapper<UmbracoPickerPropertyMapper>();
            //this.AddMap(p => p.RelatedLink);
            //this.AddMap(p => p.NullRelatedLinks);

        }
    }
}
