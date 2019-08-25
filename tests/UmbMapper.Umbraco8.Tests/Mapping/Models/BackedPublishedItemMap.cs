//using System;
//using UmbMapper.PropertyMappers;

//namespace UmbMapper.Umbraco8.Tests.Mapping.Models
//{
//    public class BackedPublishedItemMap : UmbMapperConfig<BackedPublishedItem>
//    {
//        public BackedPublishedItemMap()
//        {
//            this.AddMap(p => p.Slug).MapFromInstance((instance, content) => instance.Name.ToLowerInvariant());
//            this.AddMap(p => p.PlaceOrder).AsLazy().SetMapper<EnumPropertyMapper>();
//            this.AddMap(p => p.PublishedInterfaceContent).AsLazy();//.SetMapper<UmbracoPickerPropertyMapper>();
//            this.AddMap(p => p.PublishedContent).AsLazy();//.SetMapper<UmbracoPickerPropertyMapper>();
//            this.AddMap(p => p.Image).AsLazy();
//            this.AddMap(p => p.Child).AsLazy();//.SetMapper<UmbracoPickerPropertyMapper>();
//        }
//    }
//}
