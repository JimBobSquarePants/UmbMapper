namespace UmbMapper.Factories
{
    public interface IMappingProcessorFactory
    {
        MappingProcessor<T> Create<T>(IUmbMapperConfig mappingConfig) where T : class;
        MappingProcessor<T> Create<T>(UmbMapperConfig<T> mappingConfig) where T : class;
    }
}
