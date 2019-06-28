using UmbMapper.Umbraco8.Sample.Models.UmbracoDocTypes;
using Umbraco.Core.Composing;

namespace UmbMapper.Umbraco8.Sample.Composing
{
    public class UmbMapperRegistryComponent : IComponent
    {
        public void Initialize()
        {
            UmbMapperRegistry.AddMapperFor<SiteRoot>();
            UmbMapperRegistry.AddMapperFor<MetaDataComposition>();
        }

        public void Terminate()
        {
            UmbMapperRegistry.ClearMappers();
        }
    }
}