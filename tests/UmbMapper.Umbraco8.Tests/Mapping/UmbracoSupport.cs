using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Moq;
using Newtonsoft.Json;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mapping.Models.PropertyMapDefinitions;
using UmbMapper.Umbraco8.Tests.Mocks;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.Core.Services;
using Umbraco.Tests.TestHelpers;
using Umbraco.Tests.Testing.Objects.Accessors;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class UmbracoSupport : IDisposable
    {
        private bool disposed;
        private Link link;
        private ImageCropperValue dataSet;

        protected Composition Composition { get; private set; }
        IFactory Factory { get; set; }

        public UmbracoSupport()
        {
            this.Setup();
            this.InitPublishedProperties();
        }

        public MockPublishedContent Content => this.GetContent();

        private void TearDown()
        {
            //UmbMapperRegistry.ClearMappers();
            Current.Reset();
        }

        private void Setup()
        {
            /*
             * Ideally we want to mock a composition to do everything
            var compositionMock = new Mock<Composition>();

            this.Composition = compositionMock.Object;
            this.Composition.WithCollectionBuilder<PropertyValueConverterCollectionBuilder>();
            */
            Current.Factory = Mock.Of<IFactory>();
            //this.InitMappers();
        }

        public void InitMappers(IUmbMapperRegistry umbMapperRegistry)
        {
            //umbMapperRegistry.AddMapper<PublishedItemMap, PublishedItem>();  //umbMapperRegistry.AddMapper(new PublishedItemMap());
            ////umbMapperRegistry.AddMapper(new LazyPublishedItemMap());
            //umbMapperRegistry.AddMapperFor<AutoMappedItem>();
            //umbMapperRegistry.AddMapper<BackedPublishedItemMap, BackedPublishedItem>();  //umbMapperRegistry.AddMapper(new BackedPublishedItemMap());

            //umbMapperRegistry.AddMapper<InheritedPublishedItemMap, InheritedPublishedItem>(); // umbMapperRegistry.AddMapper(new InheritedPublishedItemMap());

            //umbMapperRegistry.AddMapper<CsvPublishedItemMap, CsvPublishedItem>();
            //umbMapperRegistry.AddMapperFor<PolymorphicItemOne>();
            //umbMapperRegistry.AddMapperFor<PolymorphicItemTwo>();
        }

        //public void InitFactoryMappers(IUmbMapperRegistry umbMapperRegistry)
        //{
        //    //umbMapperRegistry.AddMapper(new PublishedItemMapDefinition());
        //}

        public void InitFactoryMappers(IUmbMapperInitialiser umbMapperInitialiser)
        {
            umbMapperInitialiser.AddMapper(new PublishedItemMapDefinition());
            umbMapperInitialiser.AddMapper(new BackedPublishedItemMapDefinition());
            umbMapperInitialiser.AddMapper(new CsvPublishedItemMapDefinition());
            umbMapperInitialiser.AddMapper(new InheritedPublishedItemMapDefinition());
            umbMapperInitialiser.AddMapper(new LazyPublishedItemMap());
            umbMapperInitialiser.AddMapperFor<AutoMappedItem>();
            umbMapperInitialiser.AddMapperFor<PolymorphicItemOne>();
            umbMapperInitialiser.AddMapperFor<PolymorphicItemTwo>();
        }

        public void SetupUmbracoContext()
        {
            // Get the internal constructor
            //ConstructorInfo umbracoContextCtor = typeof(UmbracoContext)
            //    .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();

            //// build required parameters to invoke UmbracoContext
            //var _httpContextFactory = new FakeHttpContextFactory("~/Home");
            //var umbracoSettings = UmbMapperMockFactory.GetUmbracoSettings();
            //var globalSettings = Mock.Of<IGlobalSettings>();
            //var publishedSnapshotService = new Mock<IPublishedSnapshotService>();
            //publishedSnapshotService.Setup(x => x.CreatePublishedSnapshot(It.IsAny<string>())).Returns(Mock.Of<IPublishedSnapshot>());
            //var ctxMock = new Mock<UmbracoContext>();

            //// This has been copied from Umbraco source code Umbraco.Tests.Cache.PublishedCache.PublishContentCacheTests.Initialize
            //// This is where you could start setting up more Umbraco stuff to test against, e.g. content xml
            ////_xml = new XmlDocument();
            ////_xml.LoadXml(GetXml());
            ////var xmlStore = new XmlStore(() => _xml, null, null, null);
            ////var appCache = new DictionaryAppCache();
            ////var domainCache = new DomainCache(ServiceContext.DomainService, DefaultCultureAccessor);
            ////var publishedShapshot = new PublishedSnapshot(
            ////    new PublishedContentCache(xmlStore, domainCache, appCache, globalSettings, new SiteDomainHelper(), umbracoContextAccessor, ContentTypesCache, null, null),
            ////    new PublishedMediaCache(xmlStore, ServiceContext.MediaService, ServiceContext.UserService, appCache, ContentTypesCache, Factory.GetInstance<IEntityXmlSerializer>(), umbracoContextAccessor),
            ////    new PublishedMemberCache(null, appCache, Current.Services.MemberService, ContentTypesCache, umbracoContextAccessor),
            ////    domainCache);
            ////var publishedSnapshotService = new Mock<IPublishedSnapshotService>();
            ////publishedSnapshotService.Setup(x => x.CreatePublishedSnapshot(It.IsAny<string>())).Returns(publishedShapshot);
            /////// END

            //object umbracoContextObject =
            //    umbracoContextCtor.Invoke(
            //        new object[] {
            //            _httpContextFactory.HttpContext,
            //            publishedSnapshotService.Object,
            //            new WebSecurity(_httpContextFactory.HttpContext, Mock.Of<IUserService>(), globalSettings),
            //            umbracoSettings,
            //            Enumerable.Empty<IUrlProvider>(),
            //            globalSettings,
            //            new TestVariationContextAccessor()
            //        }
            //    );

            UmbracoContext umbracoContext = this.GetUmbracoContext();

            ConstructorInfo publishedRequestCtor = typeof(PublishedRequest)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();

            object publishedRequestObject =
                publishedRequestCtor.Invoke(
                    new object[] {
                        Mock.Of<IPublishedRouter>(),
                        umbracoContext,
                        null
                    });

            umbracoContext.PublishedRequest = publishedRequestObject as PublishedRequest;
            umbracoContext.PublishedRequest.Culture = new CultureInfo("en-US");

            var contextAccessor = new Mock<IUmbracoContextAccessor>();
            contextAccessor.Setup(x => x.UmbracoContext).Returns(umbracoContext);

            Umbraco.Web.Composing.Current.UmbracoContextAccessor = contextAccessor.Object;
        }

        // TODO - Update to create an UmbracoContext Factory that we can inject into our own factories, etc to get 
        // tests working 
        public UmbracoContext GetUmbracoContext()
        {
            ConstructorInfo umbracoContextCtor = typeof(UmbracoContext)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();

            // build required parameters to invoke UmbracoContext
            var _httpContextFactory = new FakeHttpContextFactory("~/Home");
            var umbracoSettings = UmbMapperMockFactory.GetUmbracoSettings();
            var globalSettings = Mock.Of<IGlobalSettings>();
            var publishedSnapshotService = new Mock<IPublishedSnapshotService>();
            publishedSnapshotService.Setup(x => x.CreatePublishedSnapshot(It.IsAny<string>())).Returns(Mock.Of<IPublishedSnapshot>());
            var ctxMock = new Mock<UmbracoContext>();

            // This has been copied from Umbraco source code Umbraco.Tests.Cache.PublishedCache.PublishContentCacheTests.Initialize
            // This is where you could start setting up more Umbraco stuff to test against, e.g. content xml
            //_xml = new XmlDocument();
            //_xml.LoadXml(GetXml());
            //var xmlStore = new XmlStore(() => _xml, null, null, null);
            //var appCache = new DictionaryAppCache();
            //var domainCache = new DomainCache(ServiceContext.DomainService, DefaultCultureAccessor);
            //var publishedShapshot = new PublishedSnapshot(
            //    new PublishedContentCache(xmlStore, domainCache, appCache, globalSettings, new SiteDomainHelper(), umbracoContextAccessor, ContentTypesCache, null, null),
            //    new PublishedMediaCache(xmlStore, ServiceContext.MediaService, ServiceContext.UserService, appCache, ContentTypesCache, Factory.GetInstance<IEntityXmlSerializer>(), umbracoContextAccessor),
            //    new PublishedMemberCache(null, appCache, Current.Services.MemberService, ContentTypesCache, umbracoContextAccessor),
            //    domainCache);
            //var publishedSnapshotService = new Mock<IPublishedSnapshotService>();
            //publishedSnapshotService.Setup(x => x.CreatePublishedSnapshot(It.IsAny<string>())).Returns(publishedShapshot);
            ///// END

            object umbracoContextObject =
                umbracoContextCtor.Invoke(
                    new object[] {
                        _httpContextFactory.HttpContext,
                        publishedSnapshotService.Object,
                        new WebSecurity(_httpContextFactory.HttpContext, Mock.Of<IUserService>(), globalSettings),
                        umbracoSettings,
                        Enumerable.Empty<IUrlProvider>(),
                        globalSettings,
                        new TestVariationContextAccessor()
                    }
                );

            UmbracoContext umbracoContext = umbracoContextObject as UmbracoContext;

            return umbracoContext;
        }
        //public IUmbracoContextFactory GetUmbracoContextFactory()
        //{
        //    var mockContextFactory = new Mock<IUmbracoContextFactory>();
        //    mockContextFactory.Setup(f => f.EnsureUmbracoContext()).Returns(new UmbracoContextReference());

        //    return mockContextFactory.Object;
        //}

        private void InitPublishedProperties()
        {
            this.link = new Link
            {
                Name = "Test Caption",
                Target = "",
                Type = LinkType.Content,
                Url = ""
            };
            // JSON test data taken from Umbraco unit-test:
            // https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Tests/PropertyEditors/ImageCropperTest.cs
            string json = "{\"focalPoint\": {\"left\": 0.96,\"top\": 0.80827067669172936},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\":\"thumb\",\"width\": 100,\"height\": 100,\"coordinates\": {\"x1\": 0.58729977382575338,\"y1\": 0.055768992440203169,\"x2\": 0,\"y2\": 0.32457553600198386}}]}";
            this.dataSet = JsonConvert.DeserializeObject<ImageCropperValue>(json);
        }

        public MockPublishedContent GetContent()
        {
            return new MockPublishedContent
            {
                Properties = new[]
                {
                    new MockPublishedProperty(nameof(PublishedItem.PublishedContent), 1000, Mocks.UmbMapperMockFactory.CreateMockUmbracoContentPublishedPropertyType()),
                    new MockPublishedProperty(nameof(PublishedItem.PublishedInterfaceContent), 1001, Mocks.UmbMapperMockFactory.CreateMockUmbracoContentPublishedPropertyType()),
                    new MockPublishedProperty(nameof(PublishedItem.Image), this.dataSet),
                    new MockPublishedProperty(nameof(PublishedItem.Child), 1003, Mocks.UmbMapperMockFactory.CreateMockUmbracoContentPublishedPropertyType()),

                    // We're deliberately switching these values to test enumerable conversion
                    new MockPublishedProperty(nameof(PublishedItem.Link), this.link),
                    new MockPublishedProperty(nameof(PublishedItem.Links), new List<Link> { this.link } ),
                    new MockPublishedProperty(nameof(PublishedItem.NullLinks), null),

                    // Polymorphic collections
                    new MockPublishedProperty(
                        nameof(PublishedItem.Polymorphic),
                        new MockPublishedContent[]{

                            new MockPublishedContent()
                            {
                                ContentType = new PublishedContentType(1, nameof(Models.PolymorphicItemOne), PublishedItemType.Content,Enumerable.Empty<string>(), Enumerable.Empty<PublishedPropertyType>(), ContentVariation.Nothing),
                                Properties = new[]{ new MockPublishedProperty(nameof(IPolyMorphic.PolyMorphicText),"Foo") }
                            },
                            new MockPublishedContent()
                            {
                                ContentType = new PublishedContentType(1, nameof(Models.PolymorphicItemTwo), PublishedItemType.Content,Enumerable.Empty<string>(), Enumerable.Empty<PublishedPropertyType>(), ContentVariation.Nothing),
                                Properties = new[]{ new MockPublishedProperty(nameof(IPolyMorphic.PolyMorphicText),"Bar") }
                            }
                        }
                    )
                }
            };
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.TearDown();
            this.disposed = true;
        }
    }
}
