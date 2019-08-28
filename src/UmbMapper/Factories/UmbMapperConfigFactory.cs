using System;
using System.Linq.Expressions;
using System.Reflection;
using UmbMapper.Extensions;
using UmbMapper.Models;

namespace UmbMapper.Factories
{
    public class UmbMapperConfigFactory : IUmbMapperConfigFactory
    {
        private readonly IPropertyMapFactory propertyMapFactory;
        public UmbMapperConfigFactory(IPropertyMapFactory propertyMapFactory)
        {
            this.propertyMapFactory = propertyMapFactory;
        }

        public UmbMapperConfig<T> GenerateConfig<T>(MappingDefinition<T> mappingDefinition)
            where T : class
        {
            return this.GenerateConfig(new UmbMapperConfig<T>(), mappingDefinition);
        }

        public UmbMapperConfig<T> GenerateConfig<T>(UmbMapperConfig<T> mappingConfig, MappingDefinition<T> mappingDefinition)
            where T : class
        {
            foreach (var propertyMapDefinition in mappingDefinition.MappingDefinitions)
            {
                bool mapExists =
                    this.GetOrCreateMap<T>(
                        mappingConfig,
                        propertyMapDefinition,
                        out PropertyMap<T> map);

                if (!mapExists)
                {
                    mappingConfig.Maps.Add(map);
                }
            }

            return mappingConfig;
        }

        private bool GetOrCreateMap<T>(UmbMapperConfig<T> mappingConfig, PropertyMapDefinition<T> propertyMapDefinition, out PropertyMap<T> map)
            where T : class
        {
            PropertyInfo property = propertyMapDefinition.PropertyExpression.ToPropertyInfo();

            bool exists = true;
            map = mappingConfig.Maps.Find(x => x.Info.Property.Name == property.Name);

            if (map is null)
            {
                exists = false;
                map = this.propertyMapFactory.Create<T>(propertyMapDefinition);
            }

            return exists;
        }
    }
}
