using System;
using UmbMapper.PropertyMappers;
using UmbMapper.Tests.Mapping.Models;
using Xunit;

namespace UmbMapper.Tests.Mapping
{
    public class MapperConfigTests
    {
        [Fact]
        public void MapperConfigCapturesValues()
        {
            var config = new MapperConfig<PublishedItem>();

            PropertyMap<PublishedItem> idMapper = config.AddMap(p => p.Id);
            PropertyMap<PublishedItem> nameMapper = config.AddMap(p => p.Name);
            PropertyMap<PublishedItem> createdMapper = config.AddMap(p => p.CreateDate);

            Assert.NotNull(idMapper);
            Assert.NotNull(nameMapper);
            Assert.NotNull(createdMapper);

            Assert.True(typeof(int) == idMapper.PropertyType);
            Assert.True(typeof(string) == nameMapper.PropertyType);
            Assert.True(typeof(DateTime) == createdMapper.PropertyType);
        }

        [Fact]
        public void MapperConfigSetsPropertyMappers()
        {
            var config = new MapperConfig<PublishedItem>();

            PropertyMap<PublishedItem> idMapper = config.AddMap(p => p.Id).SetMapper<UmbracoPropertyMapper>();
            PropertyMap<PublishedItem> nameMapper = config.AddMap(p => p.Name).SetMapper<UmbracoPropertyMapper>();
            PropertyMap<PublishedItem> createdMapper = config.AddMap(p => p.CreateDate).SetMapper<UmbracoPropertyMapper>();

            Assert.NotNull(idMapper.PropertyMapper);
            Assert.NotNull(nameMapper.PropertyMapper);
            Assert.NotNull(createdMapper.PropertyMapper);

            Assert.True(typeof(int) == idMapper.PropertyMapper.PropertyType);
            Assert.True(typeof(string) == nameMapper.PropertyMapper.PropertyType);
            Assert.True(typeof(DateTime) == createdMapper.PropertyMapper.PropertyType);
        }
    }
}