using System;
using UmbMapper.PropertyMappers;

namespace UmbMapper.Factories
{
    public interface IFactoryPropertyMapperFactory
    {
        FactoryPropertyMapperBase Create(PropertyMapInfo info, Type type);
    }
}
