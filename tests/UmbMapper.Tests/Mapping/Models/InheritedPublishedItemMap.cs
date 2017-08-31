namespace UmbMapper.Tests.Mapping.Models
{
    public class InheritedPublishedItemMap : BasePublishedItemMap<InheritedPublishedItem>
    {
        public InheritedPublishedItemMap()
        {
            this.AddMap(p => p.Slug).MapFromInstance((instance, content) => instance.Name.ToLowerInvariant());
        }
    }
}
