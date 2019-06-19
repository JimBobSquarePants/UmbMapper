using UmbMapper.Umbraco8.Tests.Mapping.Models;
using Umbraco.Tests.PublishedContent;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class UmbMapperPublishedContentTests : PublishedContentTests
    {
        public UmbMapperPublishedContentTests()
        {
            this.InitMappers();
        }

        private void InitMappers()
        {
            UmbMapperRegistry.AddMapper(new PublishedItemMap());
            //UmbMapperRegistry.AddMapper(new LazyPublishedItemMap());
            //UmbMapperRegistry.AddMapperFor<AutoMappedItem>();
            //UmbMapperRegistry.AddMapper(new BackedPublishedItemMap());
            //UmbMapperRegistry.AddMapper(new InheritedPublishedItemMap());
            //UmbMapperRegistry.AddMapper(new CsvPublishedItemMap());
            //UmbMapperRegistry.AddMapperFor<PolymorphicItemOne>();
            //UmbMapperRegistry.AddMapperFor<PolymorphicItemTwo>();
        }
    }
}
