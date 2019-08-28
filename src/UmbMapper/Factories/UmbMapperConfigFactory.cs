using System;
using System.Reflection;
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
            return GenerateConfig(new UmbMapperConfig<T>(), mappingDefinition);

            
        }

        public UmbMapperConfig<T> GenerateConfig<T>(UmbMapperConfig<T> mappingConfig, MappingDefinition<T> mappingDefinition)
            where T : class
        {

            //var map = this.GetOrCreateMap<T>(mapping)


                return mappingConfig;
        }





        private bool GetOrCreateMap<T>(UmbMapperConfig<T> mapping, PropertyInfo property, out PropertyMap<T> map)
            where T : class
        {
            bool exists = true;
            map = mapping.Maps.Find(x => x.Info.Property.Name == property.Name);

            if (map is null)
            {
                exists = false;
                map = new PropertyMap<T>(property);
                //map = this.propertyMapFactory.Create<T>(new PropertyMapDefinition<T>()); //new PropertyMap<T>(property);
            }
            return exists;
        }
    }
}
