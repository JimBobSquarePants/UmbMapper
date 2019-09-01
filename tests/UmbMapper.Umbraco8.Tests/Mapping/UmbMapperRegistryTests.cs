using System.Collections.Generic;
using System.Linq;
using Moq;
using UmbMapper.Factories;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mapping.Models.PropertyMapDefinitions;
using Umbraco.Web;
using Xunit;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class UmbMapperRegistryTests
    {
        private readonly IUmbMapperRegistry umbMapperRegistry;
        private readonly IUmbMapperService umbMapperService;
        private readonly IFactoryPropertyMapperFactory factoryPropertyMapperFactory;
        protected readonly IPropertyMapperFactory propertyMapperFactory;
        private readonly IPropertyMapFactory propertyMapFactory;
        private readonly IUmbMapperInitialiser umbMapperInitialiser;
        protected readonly IMappingProcessorFactory mappingProcessorFactory;

        public UmbMapperRegistryTests()
        {
            this.umbMapperRegistry = new UmbMapperRegistry();
            this.mappingProcessorFactory = new MappingProcessorFactory(Mock.Of<IUmbracoContextFactory>());
            this.umbMapperService = new UmbMapperService(this.umbMapperRegistry, this.mappingProcessorFactory);
            this.factoryPropertyMapperFactory = new FactoryPropertyMapperFactory(this.umbMapperRegistry);
            this.propertyMapperFactory = new PropertyMapperFactory(this.umbMapperRegistry, this.umbMapperService, Mock.Of<IUmbracoContextFactory>());
            var propertyMapFactory = new PropertyMapFactory(factoryPropertyMapperFactory, this.propertyMapperFactory);
            //var propertyMapFactory = new PropertyMapFactory(this.factoryPropertyMapperFactory, Mock.Of<IUmbracoContextFactory>());
            this.umbMapperInitialiser = new UmbMapperInitialiser(this.umbMapperRegistry, this.propertyMapFactory);
        }

        [Fact]
        public void UmbMapperRegistryCanStoreMapper()
        {
            var registry = new UmbMapperRegistry();
            var factoryPropertyMapperFactory = new FactoryPropertyMapperFactory(registry);
            var propertyMapperFactory = new PropertyMapperFactory(this.umbMapperRegistry, this.umbMapperService, Mock.Of<IUmbracoContextFactory>());
            var propertyMapFactory = new PropertyMapFactory(factoryPropertyMapperFactory, propertyMapperFactory);
            //var propertyMapFactory = new PropertyMapFactory(this.factoryPropertyMapperFactory, Mock.Of<IUmbracoContextFactory>());
            var initialiser = new UmbMapperInitialiser(registry, propertyMapFactory);

            initialiser.AddMapper<PublishedItem>(new PublishedItemMapDefinition());

            Assert.Contains(registry.CurrentMappers(), m => m.MappedType == typeof(PublishedItem));
        }

        [Fact]
        public void UmbMapperCanMapAll()
        {
            var registry = new UmbMapperRegistry();
            var factoryPropertyMapperFactory = new FactoryPropertyMapperFactory(registry);
            var propertyMapperFactory = new PropertyMapperFactory(this.umbMapperRegistry, this.umbMapperService, Mock.Of<IUmbracoContextFactory>());
            var propertyMapFactory = new PropertyMapFactory(factoryPropertyMapperFactory, propertyMapperFactory);
            //var propertyMapFactory = new PropertyMapFactory(this.factoryPropertyMapperFactory, Mock.Of<IUmbracoContextFactory>());
            var initialiser = new UmbMapperInitialiser(registry, propertyMapFactory);

            initialiser.AddMapperFor<PublishedItem>();

            //registry.AddMapper<PublishedItemMapMany, PublishedItem>();
            registry.Mappers.TryGetValue(typeof(PublishedItem), out IUmbMapperConfig umbMapperConfig);

            Assert.True(umbMapperConfig.Mappings.Any());
        }

        //[Fact]
        ////TODO - how do we test this now?
        //public void UmbMapperCanMapMany()
        //{
        //    var mapper = new PublishedItemMapMany();

        //    var registry = new UmbMapperRegistry(Mock.Of<IUmbracoContextFactory>());
        //    registry.AddMapper<PublishedItemMapMany, PublishedItem>(mapper);

        //    //var mapper = new PublishedItemMapMany();
        //    IEnumerable<IPropertyMap> currentMaps = mapper.Mappings;
        //    int currentCount = currentMaps.Count();

        //    IEnumerable<PropertyMap<PublishedItem>> maps = mapper.AddMappings(
        //          p => p.PublishedInterfaceContent,
        //          p => p.PublishedContent,
        //          p => p.Image,
        //          p => p.Child);

        //    Assert.True(maps.Count() == 4);

        //    // Original map list count is unchanged
        //    Assert.True(mapper.Mappings.Count() == currentCount);


        //}
    }
}
