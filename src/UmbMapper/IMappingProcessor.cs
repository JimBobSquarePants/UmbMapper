using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper
{
    public interface IMappingProcessor
    {
        object Map(IUmbMapperConfig mappingConfig, IPublishedContent content);
        void Map(IUmbMapperConfig mappingConfig, IPublishedContent content, object destination);
        object CreateEmpty(IUmbMapperConfig mappingConfig);
        object CreateEmpty(IUmbMapperConfig mappingConfig, IPublishedContent content);
    }
}
