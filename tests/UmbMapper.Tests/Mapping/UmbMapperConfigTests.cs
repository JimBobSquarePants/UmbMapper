using System;
using UmbMapper.PropertyMappers;
using UmbMapper.Tests.Mapping.Models;
using Xunit;

namespace UmbMapper.Tests.Mapping
{
    public class UmbMapperConfigTests
    {
        [Fact]
        public void MapperConfigCapturesValues()
        {
            var config = new UmbMapperConfig<PublishedItem>();

            PropertyMap<PublishedItem> idMapper = config.AddMap(p => p.Id);
            PropertyMap<PublishedItem> nameMapper = config.AddMap(p => p.Name);
            PropertyMap<PublishedItem> createdMapper = config.AddMap(p => p.CreateDate);

            Assert.NotNull(idMapper);
            Assert.NotNull(nameMapper);
            Assert.NotNull(createdMapper);

            Assert.True(typeof(int) == idMapper.Info.PropertyType);
            Assert.True(typeof(string) == nameMapper.Info.PropertyType);
            Assert.True(typeof(DateTime) == createdMapper.Info.PropertyType);
        }

        [Fact]
        public void MapperThrowsWhenLazyIsNotVirtual()
        {
            var config = new UmbMapperConfig<PublishedItem>();

            Assert.Throws<InvalidOperationException>(() =>
            {
                config.AddMap(p => p.Id).AsLazy();
            });
        }

        [Fact]
        public void MapperAllowsLazyVirtual()
        {
            var config = new UmbMapperConfig<LazyPublishedItem>();
            config.AddMap(p => p.Id).AsLazy();
        }

        [Fact]
        public void MapperConfigSetsPropertyMappers()
        {
            var config = new UmbMapperConfig<PublishedItem>();

            UmbracoPropertyMapper idMapper = config.AddMap(p => p.Id).SetMapper<UmbracoPropertyMapper>();
            UmbracoPropertyMapper nameMapper = config.AddMap(p => p.Name).SetMapper<UmbracoPropertyMapper>();
            UmbracoPropertyMapper createdMapper = config.AddMap(p => p.CreateDate).SetMapper<UmbracoPropertyMapper>();

            Assert.NotNull(idMapper);
            Assert.NotNull(nameMapper);
            Assert.NotNull(createdMapper);

            Assert.True(typeof(int) == idMapper.Info.PropertyType);
            Assert.True(typeof(string) == nameMapper.Info.PropertyType);
            Assert.True(typeof(DateTime) == createdMapper.Info.PropertyType);
        }
    }
}