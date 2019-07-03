using UmbMapper.Umbraco8.Sample.Models.UmbracoDocTypes;
using Umbraco.Core.Composing;
using Umbraco.Core.PropertyEditors.ValueConverters;

namespace UmbMapper.Umbraco8.Sample.Composing
{
    public class UmbMapperRegistryComponent : IComponent
    {
        public void Initialize()
        {
            UmbMapperRegistry.AddMapperFor<SiteRoot>();
            UmbMapperRegistry.AddMapperFor<ImageCropperValue>();
            UmbMapperRegistry.AddMapperFor<MetaDataComposition>();
            UmbMapperRegistry.AddMapperFor<BasicHeaderComposition>();
            UmbMapperRegistry.AddMapperFor<BasicImage>();
        }

        public void Terminate()
        {
            UmbMapperRegistry.ClearMappers();
        }
    }
}