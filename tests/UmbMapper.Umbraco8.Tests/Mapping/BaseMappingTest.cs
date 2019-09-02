using Moq;
using UmbMapper.Factories;
using UmbMapper.Umbraco8TestSupport.MockHelpers;
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

        protected readonly UmbracoContextHelper _ctxHelper;
        public BaseMappingTest(UmbracoSupport support)
        {
            this.support = support;
            _ctxHelper = new UmbracoContextHelper();

            //_ctxHelper.InitializeUmbracoContextMock();

            // This is needed to access the culture info
            this.support.SetupUmbracoContext();

            this.umbMapperRegistry = new UmbMapperRegistry();
            this.mappingProcessorFactory = new MappingProcessorFactory(Mock.Of<IUmbracoContextFactory>());
            this.umbMapperService = new UmbMapperService(this.umbMapperRegistry, this.mappingProcessorFactory);

            this.factoryPropertyMapperFactory = new FactoryPropertyMapperFactory(this.umbMapperRegistry, this.umbMapperService);
            this.propertyMapperFactory = new PropertyMapperFactory(this.umbMapperRegistry, this.umbMapperService, Mock.Of<IUmbracoContextFactory>());
            this.propertyMapFactory = new PropertyMapFactory(this.factoryPropertyMapperFactory, this.propertyMapperFactory);
            //this.propertyMapFactory = new PropertyMapFactory(this.factoryPropertyMapperFactory, Mock.Of<IUmbracoContextFactory>());
            this.umbMapperInitialiser = new UmbMapperInitialiser(this.umbMapperRegistry, this.propertyMapFactory);
            
        }

        
    }
}
