using UmbMapper.Models;

namespace UmbMapper
{
    public interface IUmbMapperInitialiser
    {
        void AddMapper<T>(MappingDefinition<T> mappingDefinition)
            where T : class;

        void AddMapperFor<T>()
            where T : class;
    }
}
