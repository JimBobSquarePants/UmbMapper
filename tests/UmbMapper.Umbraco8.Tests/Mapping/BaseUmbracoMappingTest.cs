using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class BaseUmbracoMappingTest : IDisposable
    {
        public BaseUmbracoMappingTest()
        {
            Current.Factory = new Mock<IFactory>().Object;// Mock.Of<IFactory>();
            this.InitMappers();

            //var _httpContextFactory = new FakeHttpContextFactory("~/Home");

            //var umbracoSettings = Umbraco.Web.Composing.Current.Factory.GetInstance<IUmbracoSettingsSection>();
            //var globalSettings = Umbraco.Web.Composing.Current.Factory.GetInstance<IGlobalSettings>();
            //var umbracoContextAccessor = Umbraco.Web.Composing.Current.Factory.GetInstance<IUmbracoContextAccessor>();

            //var _umbracoContext = new UmbracoContext();

            ////Umbraco.Web.Composing.Current.UmbracoContext
        }

        private void InitMappers()
        {
            UmbMapperRegistry.AddMapper(new PublishedItemMap());
            UmbMapperRegistry.AddMapper(new LazyPublishedItemMap());
            UmbMapperRegistry.AddMapperFor<AutoMappedItem>();
            UmbMapperRegistry.AddMapper(new BackedPublishedItemMap());
            UmbMapperRegistry.AddMapper(new InheritedPublishedItemMap());
            UmbMapperRegistry.AddMapper(new CsvPublishedItemMap());
            UmbMapperRegistry.AddMapperFor<PolymorphicItemOne>();
            UmbMapperRegistry.AddMapperFor<PolymorphicItemTwo>();
        }
        public void Dispose()
        {
            UmbMapperRegistry.ClearMappers();
            Current.Reset();
        }
    }
}
