using System.Linq;
using UmbMapper.Extensions;
using UmbMapper.Models;

namespace UmbMapper.Factories
{
    public class PropertyMapFactory : IPropertyMapFactory
    {
        public PropertyMap<T> Create<T>(PropertyMapDefinition<T> mapDefinition)
            where T : class
        {
            PropertyMap<T> newMap = new PropertyMap<T>(mapDefinition.PropertyExpression.ToPropertyInfo());

            if (mapDefinition.Aliases.Any())
            {
                newMap.SetAlias(mapDefinition.Aliases);
            }

            if (mapDefinition.Culture != null)
            {
                newMap.SetCulture(mapDefinition.Culture);
            }

            //if (mapDefinition.MapperType != null)
            //{
            //    newMap.PropertyMapper = propertyMapperFactory.Create(something)??? mapDefinition.MapperType.GetInstance(newMap.Info);
            //}
            //if (mapDefinition.FactoryMapperType != null)
            //{
            //    newMap.PropertyMapper = propertyMapperFactory.Create(something)??? mapDefinition.MapperType.GetInstance(newMap.Info);
            //}
            if (mapDefinition.Lazy)
            {
                newMap.AsLazy();
            }

            if (mapDefinition.Recursive)
            {
                newMap.AsRecursive();
            }

            if (mapDefinition.DefaultValue != null)
            {
                newMap.DefaultValue(mapDefinition.DefaultValue);
            }


            return newMap;
        }
    }
}
