namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class InheritedPublishedItemMapDefinition : BasePublishedItemMapDefinition<InheritedPublishedItem>
    {
        public InheritedPublishedItemMapDefinition()
        {
            this.AddMappingDefinition(x => x.Image).AsLazy();
            this.AddMappingDefinition(p => p.Slug).MapFromInstance((instance, content) => instance.Name.ToLowerInvariant());
        }
    }
}

