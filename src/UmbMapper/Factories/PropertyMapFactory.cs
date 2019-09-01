using System.Linq;
using UmbMapper.Extensions;
using UmbMapper.Models;
using UmbMapper.PropertyMappers;
using Umbraco.Web;

namespace UmbMapper.Factories
{
    public class PropertyMapFactory : IPropertyMapFactory
    {
        private readonly IFactoryPropertyMapperFactory factoryPropertyMapperFactory;
        private readonly IPropertyMapperFactory propertyMapperFactory;
        //private readonly IUmbracoContextFactory umbracoContextFactory;

        //public PropertyMapFactory(IFactoryPropertyMapperFactory factoryPropertyMapperFactory, IUmbracoContextFactory umbracoContextFactory)
        public PropertyMapFactory(IFactoryPropertyMapperFactory factoryPropertyMapperFactory, IPropertyMapperFactory propertyMapperFactory)

        {
            this.factoryPropertyMapperFactory = factoryPropertyMapperFactory;
            this.propertyMapperFactory = propertyMapperFactory;
        }

        public PropertyMap<T> Create<T>(PropertyMapDefinition<T> mapDefinition)
            where T : class
        {
            PropertyMap<T> newMap = new PropertyMap<T>(mapDefinition.PropertyInfo);

            if (mapDefinition.Aliases != null && mapDefinition.Aliases.Any())
            {
                newMap.SetAlias(mapDefinition.Aliases);
            }

            if (mapDefinition.Culture != null)
            {
                newMap.SetCulture(mapDefinition.Culture);
            }

            if (mapDefinition.MapperType != null)
            {
                newMap.PropertyMapper = this.propertyMapperFactory.CreateMapper(newMap.Info, mapDefinition.MapperType);// mapDefinition.MapperType.GetInstance(newMap.Info) as IPropertyMapper;
            }

            if (mapDefinition.FactoryMapperType != null)
            {
                newMap.PropertyMapper = this.propertyMapperFactory.CreateFactoryMapper(newMap.Info, mapDefinition.FactoryMapperType);// this.factoryPropertyMapperFactory.Create(newMap.Info, mapDefinition.FactoryMapperType);
            }

            if (mapDefinition.Lazy)
            {
                newMap.SetLazy(true);
            }

            if (mapDefinition.Recursive)
            {
                newMap.SetRecursive(true);
            }

            if (mapDefinition.MapFromInstancePredicate != null)
            {
                newMap.SetMapFromInstance(mapDefinition.MapFromInstancePredicate);
            }

            if (mapDefinition.DefaultValue != null)
            {
                newMap.SetDefaultValue(mapDefinition.DefaultValue);
            }

            return newMap;
        }
    }
}
