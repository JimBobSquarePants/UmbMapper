namespace UmbMapper.Tests.Mapping.Models
{
    public class BasePublishedItemMap<T> : UmbMapperConfig<T>
         where T : BasePublishedItem
    {
        public BasePublishedItemMap()
        {
            this.MapAll();
        }
    }
}
