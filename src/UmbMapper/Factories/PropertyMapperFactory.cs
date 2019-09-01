using System;
using UmbMapper.Extensions;
using UmbMapper.PropertyMappers;
using Umbraco.Web;

namespace UmbMapper.Factories
{
    public class PropertyMapperFactory : IPropertyMapperFactory
    {
        private readonly IUmbMapperRegistry umbMapperRegistry;
        private readonly IUmbMapperService umbMapperService;
        private readonly IUmbracoContextFactory umbracoContextFactory;

        public PropertyMapperFactory(IUmbMapperRegistry umbMapperRegistry, IUmbMapperService umbMapperService, IUmbracoContextFactory umbracoContextFactory)
        {
            this.umbMapperRegistry = umbMapperRegistry;
            this.umbMapperService = umbMapperService;
            this.umbracoContextFactory = umbracoContextFactory;
        }

        public IPropertyMapper CreateMapper(PropertyMapInfo info, Type type)
        {
            return Create(info, type);
        }

        public FactoryPropertyMapperBase CreateFactoryMapper(PropertyMapInfo info, Type type)
        {
            return Create(info, type) as FactoryPropertyMapperBase;
        }

        private IPropertyMapper Create(PropertyMapInfo info, Type type)
        {
            return
                type.GetInstance(info, this.umbMapperRegistry, this.umbMapperService, this.umbracoContextFactory)
                    as IPropertyMapper;
        }
    }
}
