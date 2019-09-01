using System;
using UmbMapper.PropertyMappers;

namespace UmbMapper.Factories
{
    public interface IPropertyMapperFactory
    {
        IPropertyMapper CreateMapper(PropertyMapInfo info, Type type);
        FactoryPropertyMapperBase CreateFactoryMapper(PropertyMapInfo info, Type type);
    }
}
