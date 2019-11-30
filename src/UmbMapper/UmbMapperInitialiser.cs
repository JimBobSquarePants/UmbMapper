using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UmbMapper.Extensions;
using UmbMapper.Factories;
using UmbMapper.Models;

namespace UmbMapper
{
    public class UmbMapperInitialiser : IUmbMapperInitialiser
    {
        private readonly IUmbMapperRegistry umbMapperRegistry;
        private readonly IPropertyMapFactory propertyMapFactory;

        public UmbMapperInitialiser(IUmbMapperRegistry umbMapperRegistry, IPropertyMapFactory propertyMapFactory)
        {
            this.umbMapperRegistry = umbMapperRegistry;
            this.propertyMapFactory = propertyMapFactory;
        }

        public void AddMapper<T>(MappingDefinition<T> mappingDefinition)
            where T : class
        {
            this.MapWithProperties<T>(mappingDefinition.MappingDefinitions);
        }

        public void AddMapperFor<T>()
            where T : class
        {
            this.MapWithProperties<T>(
                typeof(T).GetProperties(UmbMapperConstants.MappableFlags)
                    .Where(p => p.CanWrite)
                    .Select(x => new PropertyMapDefinition<T>(x).AsAutoLazy()));
        }

        private void MapWithProperties<T>(IEnumerable<PropertyMapDefinition<T>> definitions)
            where T : class
        {
            if (this.umbMapperRegistry.Mappers.ContainsKey(typeof(T)))
            {
                return;
            }

            UmbMapperConfig<T> mappingConfig = new UmbMapperConfig<T>();

            foreach (var definition in definitions)
            {
                if (!this.GetOrCreateMap(mappingConfig, definition, out PropertyMap<T> map))
                {
                    mappingConfig.Maps.Add(map);
                }
            }

            this.umbMapperRegistry.AddMapper(mappingConfig);
        }

        private bool GetOrCreateMap<T>(UmbMapperConfig<T> mappingConfig, PropertyMapDefinition<T> propertyMapDefinition, out PropertyMap<T> map)
            where T : class
        {
            PropertyInfo property = propertyMapDefinition.PropertyInfo;

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
