using UmbMapper.Models;

namespace UmbMapper.Factories
{
    public interface IUmbMapperConfigFactory
    {
        UmbMapperConfig<T> GenerateConfig<T>(MappingDefinition<T> mappingDefinition)
            where T : class;

        UmbMapperConfig<T> GenerateConfig<T>(UmbMapperConfig<T> mappingConfig, MappingDefinition<T> mappingDefinition)
            where T : class;
    }
}
