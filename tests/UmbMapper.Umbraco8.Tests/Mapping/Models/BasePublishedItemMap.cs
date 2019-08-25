namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class BasePublishedItemMap<T> : UmbMapperConfig<T>
         where T : BasePublishedItem
    {
        public BasePublishedItemMap()
        {
            this.MapAll();
            this.AddMap(m => m.Name).AsRecursive();
        }
    }
}
