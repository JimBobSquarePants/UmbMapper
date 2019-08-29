using System;
using UmbMapper.Models;
using UmbMapper.PropertyMappers;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using Xunit;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class UmbMapperConfigTests : BaseMappingTest, IClassFixture<UmbracoSupport>
    {
        public UmbMapperConfigTests(UmbracoSupport support) : base(support)
        {

        }

        [Fact]
        public void MapperConfigCapturesValues()
        {
            var config = new MappingDefinition<PublishedItem>();

            PropertyMapDefinition<PublishedItem> idMapper = config.AddMappingDefinition(p => p.Id);
            PropertyMapDefinition<PublishedItem> nameMapper = config.AddMappingDefinition(p => p.Name);
            PropertyMapDefinition<PublishedItem> createdMapper = config.AddMappingDefinition(p => p.CreateDate);

            Assert.NotNull(idMapper);
            Assert.NotNull(nameMapper);
            Assert.NotNull(createdMapper);

            Assert.True(typeof(int) == idMapper.PropertyInfo.PropertyType);
            Assert.True(typeof(string) == nameMapper.PropertyInfo.PropertyType);
            Assert.True(typeof(DateTime) == createdMapper.PropertyInfo.PropertyType);

            // Mapping Factory tests here too
        }

        [Fact]
        public void MapperThrowsWhenLazyIsNotVirtual()
        {
            var configDefinition = new MappingDefinition<PublishedItem>();
            var nonVirtualLazyMap = configDefinition.AddMappingDefinition(p => p.Id).AsLazy();

            PropertyMap<PublishedItem> map = null;

            Assert.Throws<InvalidOperationException>(
                () =>
                    map = this.propertyMapFactory.Create<PublishedItem>(nonVirtualLazyMap)
            );
        }

        [Fact]
        public void MapperAllowsLazyVirtual()
        {
            var configDefinition = new MappingDefinition<LazyPublishedItem>();
            var nonVirtualLazyMap = configDefinition.AddMappingDefinition(p => p.Id).AsLazy();

            PropertyMap<LazyPublishedItem> map = this.propertyMapFactory.Create<LazyPublishedItem>(nonVirtualLazyMap);

            Assert.NotNull(map);
        }

        [Fact]
        public void MapperConfigSetsPropertyMappers()
        {
            var config = new MappingDefinition<PublishedItem>();

            PropertyMapDefinition<PublishedItem> idMapper = config.AddMappingDefinition(p => p.Id).SetMapper<UmbracoPropertyMapper>();
            PropertyMapDefinition<PublishedItem> nameMapper = config.AddMappingDefinition(p => p.Name).SetMapper<UmbracoPropertyMapper>();
            PropertyMapDefinition<PublishedItem> createdMapper = config.AddMappingDefinition(p => p.CreateDate).SetMapper<UmbracoPropertyMapper>();

            Assert.NotNull(idMapper);
            Assert.NotNull(nameMapper);
            Assert.NotNull(createdMapper);

            Assert.True(typeof(int) == idMapper.PropertyInfo.PropertyType);
            Assert.True(typeof(string) == nameMapper.PropertyInfo.PropertyType);
            Assert.True(typeof(DateTime) == createdMapper.PropertyInfo.PropertyType);
        }

        //TODO - add tests so that stuff created as MappingDefinition and PropertyMapDefinition come through correctly using IPropertyMapFactory
    }
}
