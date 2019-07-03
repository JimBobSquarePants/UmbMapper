//using UmbMapper.Umbraco8.Sample.Mapping;
using UmbMapper.Umbraco8.Sample.Models.UmbracoDocTypes;
using Umbraco.Core.Composing;
using Umbraco.Core.PropertyEditors.ValueConverters;

namespace UmbMapper.Umbraco8.Sample.Composing
{
    public class UmbMapperRegistryComponent : IComponent
    {
        public void Initialize()
        {
            // Custom Mappers
            //UmbMapperRegistry.AddMapper(new BasicImageItemMap());

            UmbMapperRegistry.AddMapperFor<SiteRoot>();
            //UmbMapperRegistry.AddMapperFor<ImageCropperValue>();
            UmbMapperRegistry.AddMapperFor<MetaDataComposition>();
            UmbMapperRegistry.AddMapperFor<BasicImage>();
            UmbMapperRegistry.AddMapperFor<BasicHeaderComposition>();
        }

        public void Terminate()
        {
            UmbMapperRegistry.ClearMappers();
        }
    }
}