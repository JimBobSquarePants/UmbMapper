namespace UmbMapper.Factories
{
    public interface IMappingProcessorFactory
    {
        IMappingProcessor Create(IUmbMapperConfig config);
    }
}
