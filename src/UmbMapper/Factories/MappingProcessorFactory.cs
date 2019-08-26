
namespace UmbMapper.Factories
{
    public class MappingProcessorFactory : IMappingProcessorFactory
    {
        private readonly IUmbMapperService umbMapperService;
        public MappingProcessorFactory(IUmbMapperService umbMapperService)
        {
            this.umbMapperService = umbMapperService;
        }
        public MappingProcessor<T> Create<T>(IUmbMapperConfig mappingConfig)
            where T : class
        {
            return new MappingProcessor<T>(mappingConfig, this.umbMapperService);
        }

        public MappingProcessor<T> Create<T>(UmbMapperConfig<T> mappingConfig)
            where T : class
        {
            return new MappingProcessor<T>(mappingConfig, this.umbMapperService);
        }
    }
}
