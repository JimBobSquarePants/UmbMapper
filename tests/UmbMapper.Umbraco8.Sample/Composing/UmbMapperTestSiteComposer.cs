using Umbraco.Core;
using Umbraco.Core.Composing;

namespace UmbMapper.Umbraco8.Sample.Composing
{
    public class UmbMapperTestSiteComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            // Append our component to the collection of Components
            // It will be the last one to be run
            composition.Components().Append<UmbMapperRegistryComponent>();
        }
    }
}