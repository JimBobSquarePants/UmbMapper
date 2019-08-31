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
        private readonly IUmbMapperInitialiser umbMapperInitialiser;
        public UmbMapperRegistryComponent(IUmbMapperInitialiser umbMapperInitialiser)
        {
            this.umbMapperInitialiser = umbMapperInitialiser;
        }
        public void Initialize()
        {
            // Register our custom maps
            this.umbMapperInitialiser.AddMapper(new PublishedImageMappingDefinition()); // UmbMapperRegistry.AddMapper(new PublishedImageMap());
            this.umbMapperInitialiser.AddMapper(new HomeMappingDefinition()); //UmbMapperRegistry.AddMapper(new HomeMap());
            this.umbMapperInitialiser.AddMapper(new BlogMappingDefinition()); //UmbMapperRegistry.AddMapper(new BlogMap());
            this.umbMapperInitialiser.AddMapper(new PostMappingDefinition()); //UmbMapperRegistry.AddMapper(new PostMap());

            // The Slide document type is simple with no additional customization required.
            // This can be mapped by convention. The mapper will implicitly lazy map any virtual properties.
            this.umbMapperInitialiser.AddMapperFor<Slide>(); //UmbMapperRegistry.AddMapperFor<Slide>();
        }

        public void Terminate()
        {
            //UmbMapperRegistry.ClearMappers();
        }
    }
}