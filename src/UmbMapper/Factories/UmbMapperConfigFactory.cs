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
                        propertyMapDefinition.PropertyExpression,
                        out PropertyMap<T> map);

                if (!mapExists)
                {
                    mappingConfig.Maps.Add(map);
                }
            }

            return mappingConfig;
        }

        private bool GetOrCreateMap<T>(UmbMapperConfig<T> mappingConfig, Expression<Func<T, object>> expression, out PropertyMap<T> map)
            where T : class
        {
            PropertyInfo property = expression.ToPropertyInfo();

            bool exists = true;
            map = mappingConfig.Maps.Find(x => x.Info.Property.Name == property.Name);

            if (map is null)
            {
                exists = false;
                map = new PropertyMap<T>(property);
                map = this.propertyMapFactory.Create<T>(new PropertyMapDefinition<T>(expression));
            }

            return exists;
        }
    }
}
