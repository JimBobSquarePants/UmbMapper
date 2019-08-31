using UmbMapper.Factories;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace UmbMapper.Umbraco8.Sample.Composing
{
    public class UmbMapperTestSiteComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            this.RegisterServices(composition);
            this.RegisterFactories(composition);

            // Append our component to the collection of Components
            // It will be the last one to be run
            composition.Components().Append<UmbMapperRegistryComponent>();
        }

        private void RegisterServices(Composition composition)
        {
            composition.Register<IUmbMapperInitialiser, UmbMapperInitialiser>();
            composition.Register<IUmbMapperService, UmbMapperService>();
            composition.Register<IUmbMapperRegistry, UmbMapperRegistry>(Lifetime.Singleton);
            
        }

        private void RegisterFactories(Composition composition)
        {
            composition.Register<IMappingProcessorFactory, MappingProcessorFactory>();
            composition.Register<IFactoryPropertyMapperFactory, FactoryPropertyMapperFactory>();
            composition.Register<IPropertyMapFactory, PropertyMapFactory>();
        }
    }
}