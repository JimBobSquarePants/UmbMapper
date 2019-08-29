using System;
using UmbMapper.Extensions;
using UmbMapper.PropertyMappers;

namespace UmbMapper.Factories
{
    public class FactoryPropertyMapperFactory : IFactoryPropertyMapperFactory
    {
        private readonly IUmbMapperRegistry umbMapperRegistry;
        private readonly IUmbMapperService umbMapperService;

        public FactoryPropertyMapperFactory(IUmbMapperRegistry umbMapperRegistry)
        {
            this.umbMapperRegistry = umbMapperRegistry;
        }

        public FactoryPropertyMapperFactory(IUmbMapperRegistry umbMapperRegistry, IUmbMapperService umbMapperService)
            : this (umbMapperRegistry)
        {
            this.umbMapperService = umbMapperService;
        }

        public FactoryPropertyMapperBase Create(PropertyMapInfo info, Type type)
        {
            return
                type.GetInstance(info, this.umbMapperRegistry, this.umbMapperService)
                as FactoryPropertyMapperBase;
        }
    }
}
