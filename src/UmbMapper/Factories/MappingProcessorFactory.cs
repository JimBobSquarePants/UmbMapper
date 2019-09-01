
using System;
using Umbraco.Web;

namespace UmbMapper.Factories
{
    public class MappingProcessorFactory : IMappingProcessorFactory
    {
        private readonly IUmbracoContextFactory umbracoContextFactory;
        public MappingProcessorFactory(IUmbracoContextFactory umbracoContextFactory)
        {
            this.umbracoContextFactory = umbracoContextFactory;
        }
        public IMappingProcessor Create(IUmbMapperConfig config, IUmbMapperService umbMapperService)
        {
            Type genericType = typeof(MappingProcessor<>);
            Type[] typeArgs = { config.MappedType };

            Type constructedGenericType = genericType.MakeGenericType(typeArgs);
            object[] constructorArgs = new object[] { config, umbMapperService, this.umbracoContextFactory };

            return Activator.CreateInstance(constructedGenericType, constructorArgs) as IMappingProcessor;
        }
    }
}
