using UmbMapper.Models;

namespace UmbMapper.Factories
{
    public interface IPropertyMapFactory
    {
        PropertyMap<T> Create<T>(PropertyMapDefinition<T> mapDefinition)
            where T : class;
    }
}
