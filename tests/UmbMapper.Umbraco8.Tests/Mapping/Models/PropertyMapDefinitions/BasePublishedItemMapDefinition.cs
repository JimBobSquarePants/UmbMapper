using UmbMapper.Models;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class BasePublishedItemMapDefinition<T> : MappingDefinition<T>
         where T : BasePublishedItem
    {
        public BasePublishedItemMapDefinition()
        {
            this.MapAll();
            this.AddMappingDefinition(m => m.Name).AsRecursive();
        }

        //public override void Init()
        //{
        //    this.MapAll();
        //    this.AddMap(m => m.Name).AsRecursive();

        //    base.Init();
        //}
    }
}
