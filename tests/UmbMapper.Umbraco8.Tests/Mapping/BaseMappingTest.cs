using Moq;
using UmbMapper.Factories;
using Umbraco.Web;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public abstract class BaseMappingTest
    {
        protected readonly UmbracoSupport support;
        protected readonly IUmbMapperRegistry umbMapperRegistry;
        protected readonly IUmbMapperService umbMapperService;
        protected readonly IFactoryPropertyMapperFactory factoryPropertyMapperFactory;
        protected readonly IPropertyMapperFactory propertyMapperFactory;
        protected readonly IPropertyMapFactory propertyMapFactory;
        protected readonly IUmbMapperInitialiser umbMapperInitialiser;
        protected readonly IMappingProcessorFactory mappingProcessorFactory;

        public BaseMappingTest(UmbracoSupport support)
        {
            this.support = support;

            // This is needed to access the culture info
            this.support.SetupUmbracoContext();

            this.umbMapperRegistry = new UmbMapperRegistry();
            this.umbMapperService = new UmbMapperService(this.umbMapperRegistry, new MappingProcessorFactory());

            this.factoryPropertyMapperFactory = new FactoryPropertyMapperFactory(this.umbMapperRegistry, this.umbMapperService);
            this.propertyMapperFactory = new PropertyMapperFactory(this.umbMapperRegistry, this.umbMapperService, Mock.Of<IUmbracoContextFactory>());
            this.propertyMapFactory = new PropertyMapFactory(this.factoryPropertyMapperFactory, this.propertyMapperFactory);
            //this.propertyMapFactory = new PropertyMapFactory(this.factoryPropertyMapperFactory, Mock.Of<IUmbracoContextFactory>());
            this.umbMapperInitialiser = new UmbMapperInitialiser(this.umbMapperRegistry, this.propertyMapFactory);
            this.mappingProcessorFactory = new MappingProcessorFactory();
        }

        
    }
}
