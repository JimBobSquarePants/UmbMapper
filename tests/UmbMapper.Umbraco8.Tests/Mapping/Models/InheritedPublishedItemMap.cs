namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class InheritedPublishedItemMap : BasePublishedItemMap<InheritedPublishedItem>
    {
        public InheritedPublishedItemMap()
        {
            //this.AddMap(x => x.Image).AsLazy();
            //this.AddMap(p => p.Slug).MapFromInstance((instance, content) => instance.Name.ToLowerInvariant());
        }

        public override void Init()
        {
            this.AddMap(x => x.Image).AsLazy();
            this.AddMap(p => p.Slug).MapFromInstance((instance, content) => instance.Name.ToLowerInvariant());

            base.Init();
        }
    }
}
