using System;
using UmbMapper.Extensions;
using UmbMapper.PropertyMappers;

namespace UmbMapper.Factories
{
    public class FactoryPropertyMapperFactory : IFactoryPropertyMapperFactory
    {
        private readonly IUmbMapperRegistry umbMapperRegistry;

        public FactoryPropertyMapperFactory(IUmbMapperRegistry umbMapperRegistry)
        {
            this.umbMapperRegistry = umbMapperRegistry;
        }

        public FactoryPropertyMapperBase Create(PropertyMapInfo info, Type type)
        {
            return type.GetInstance<PropertyMapInfo, IUmbMapperRegistry>(info, this.umbMapperRegistry) as FactoryPropertyMapperBase;
        }
    }
}
