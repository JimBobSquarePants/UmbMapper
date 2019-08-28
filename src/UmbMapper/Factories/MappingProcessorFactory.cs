
using System;

namespace UmbMapper.Factories
{
    public class MappingProcessorFactory : IMappingProcessorFactory
    {
        public IMappingProcessor Create(IUmbMapperConfig config)
        {
            Type genericType = typeof(MappingProcessor<>);
            Type[] typeArgs = { config.MappedType };

            Type constructedGenericType = genericType.MakeGenericType(typeArgs);
            object[] constructorArgs = new object[] { config };

            return Activator.CreateInstance(constructedGenericType, constructorArgs) as IMappingProcessor;
        }
    }
}
