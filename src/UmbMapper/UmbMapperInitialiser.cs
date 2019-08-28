using System;
using System.Reflection;
using UmbMapper.Extensions;
using UmbMapper.Factories;
using UmbMapper.Models;

namespace UmbMapper
{
    public class UmbMapperInitialiser : IUmbMapperInitialiser
    {
        private readonly IUmbMapperRegistry umbMapperRegistry;
        private readonly IUmbMapperConfigFactory umbMapperConfigFactory;
        private readonly IPropertyMapFactory propertyMapFactory;
        public UmbMapperInitialiser(IUmbMapperRegistry umbMapperRegistry, IUmbMapperConfigFactory umbMapperConfigFactory, IPropertyMapFactory propertyMapFactory)
        {
            this.umbMapperRegistry = umbMapperRegistry;
            this.umbMapperConfigFactory = umbMapperConfigFactory;
            this.propertyMapFactory = propertyMapFactory;
        }

        public void AddMapper<T>(MappingDefinition<T> mappingDefinition)
            where T : class
        {
            if (this.umbMapperRegistry.Mappers.ContainsKey(mappingDefinition.MappedType))
            {
                return;
            }

            var mappingConfig = this.umbMapperConfigFactory.GenerateConfig<T>(mappingDefinition);

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

            this.umbMapperRegistry.AddMapper(mappingConfig);
        }

        public void AddMapperFor<T>()
            where T : class
        {
            if (this.umbMapperRegistry.Mappers.ContainsKey(typeof(T)))
            {
                return;
            }

            var mappingDefinition = new MappingDefinition<T>();
            //mappingDefinition.m

            throw new NotImplementedException();
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
