using System.Linq;
using UmbMapper.Extensions;
using UmbMapper.Models;
using UmbMapper.PropertyMappers;

namespace UmbMapper.Factories
{
    public class PropertyMapFactory : IPropertyMapFactory
    {
        private readonly IFactoryPropertyMapperFactory factoryPropertyMapperFactory;

        public PropertyMapFactory(IFactoryPropertyMapperFactory factoryPropertyMapperFactory)
        {
            this.factoryPropertyMapperFactory = factoryPropertyMapperFactory;
        }

        public PropertyMap<T> Create<T>(PropertyMapDefinition<T> mapDefinition)
            where T : class
        {
            PropertyMap<T> newMap = new PropertyMap<T>(mapDefinition.PropertyExpression.ToPropertyInfo());

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
                newMap.PropertyMapper = mapDefinition.MapperType.GetInstance(newMap.Info) as IPropertyMapper;
            }

            if (mapDefinition.FactoryMapperType != null)
            {
                newMap.PropertyMapper = this.factoryPropertyMapperFactory.Create(newMap.Info, mapDefinition.FactoryMapperType);
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
