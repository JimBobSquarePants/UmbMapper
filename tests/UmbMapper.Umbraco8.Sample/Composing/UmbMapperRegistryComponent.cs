using UmbMapper.Umbraco8.Sample.ComponentModel.Mappers;
using UmbMapper.Umbraco8.Sample.Models.Components;
using UmbMapper.Umbraco8.Sample.Models.Pages;
using UmbMapper.Umbraco8.Sample.Models.UmbracoDocTypes;
using Umbraco.Core.Composing;
using Umbraco.Core.PropertyEditors.ValueConverters;

namespace UmbMapper.Umbraco8.Sample.Composing
{
    public class UmbMapperRegistryComponent : IComponent
    {
        public void Initialize()
        {
            //UmbMapperRegistry.AddMapperFor<SiteRoot>();
            //UmbMapperRegistry.AddMapperFor<ImageCropperValue>();
            //UmbMapperRegistry.AddMapperFor<MetaDataComposition>();
            //UmbMapperRegistry.AddMapperFor<BasicHeaderComposition>();
            //UmbMapperRegistry.AddMapperFor<BasicImage>();

            // Register our custom maps
            UmbMapperRegistry.AddMapper(new PublishedImageMap());
            UmbMapperRegistry.AddMapper(new HomeMap());
            UmbMapperRegistry.AddMapper(new BlogMap());
            UmbMapperRegistry.AddMapper(new PostMap());

            // The Slide document type is simple with no additional customization required.
            // This can be mapped by convention. The mapper will implicitly lazy map any virtual properties.
            UmbMapperRegistry.AddMapperFor<Slide>();
        }

        public void Terminate()
        {
            UmbMapperRegistry.ClearMappers();
        }
    }
}