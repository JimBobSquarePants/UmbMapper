using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moq;
using Newtonsoft.Json;
using UmbMapper.Tests.Mapping.Models;
using UmbMapper.Tests.Mocks;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.ObjectResolution;
using Umbraco.Core.Profiling;
using Umbraco.Core.Xml;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace UmbMapper.Tests.Mapping
{
    public class UmbracoSupport : IDisposable
    {
        private bool disposed;
        private RelatedLink link;
        private ImageCropDataSet dataSet;

        public UmbracoSupport()
        {
            this.Setup();
            this.InitPublishedProperties();
            this.InitMappers();
        }

        public MockPublishedContent Content => GetContent();

        private void TearDown()
        {
            if (PublishedCachesResolver.HasCurrent)
            {
                UmbracoContext.Current = null;
                ApplicationContext.Current = null;
                UmbMapperRegistry.ClearMappers();
                PublishedCachesResolver.Reset();

                if (Resolution.IsFrozen)
                {
                    Resolution.Reset();
                }
            }
        }

        private void Setup()
        {
            ILogger loggerMock = Mock.Of<ILogger>();
            IProfiler profilerMock = Mock.Of<IProfiler>();
            HttpContextBase contextBaseMock = Mock.Of<HttpContextBase>();
            WebSecurity webSecurityMock = new Mock<WebSecurity>(null, null).Object;
            IUmbracoSettingsSection umbracoSettingsSectionMock = Mock.Of<IUmbracoSettingsSection>();

            ApplicationContext.Current = new ApplicationContext(CacheHelper.CreateDisabledCacheHelper(), new ProfilingLogger(loggerMock, profilerMock));
            UmbracoContext.Current = UmbracoContext.EnsureContext(contextBaseMock, ApplicationContext.Current, webSecurityMock, umbracoSettingsSectionMock, Enumerable.Empty<IUrlProvider>(), true);
        }

        private void InitPublishedProperties()
        {
            if (!PublishedCachesResolver.HasCurrent)
            {
                var mockPublishedContentCache = new Mock<IPublishedContentCache>();

                mockPublishedContentCache
                    .Setup(x => x.GetById(It.IsAny<UmbracoContext>(), It.IsAny<bool>(), It.IsAny<int>()))
                    .Returns<UmbracoContext, bool, int>((ctx, preview, id) => new MockPublishedContent { Id = id });

                mockPublishedContentCache
                    .Setup(x => x.GetByXPath(It.IsAny<UmbracoContext>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<XPathVariable[]>()))
                    .Returns<UmbracoContext, bool, string, XPathVariable[]>(
                        (ctx, preview, xpath, vars) =>
                        {
                            switch (xpath)
                            {
                                case "/root":
                                case "id(1111)":
                                    return new MockPublishedContent { Id = 1111 }.AsEnumerableOfOne();

                                case "id(2222)":
                                    return new MockPublishedContent { Id = 2222 }.AsEnumerableOfOne();

                                case "id(3333)":
                                    return new MockPublishedContent { Id = 3333 }.AsEnumerableOfOne();

                                default:
                                    return Enumerable.Empty<IPublishedContent>();
                            }
                        });

                PublishedCachesResolver.Current =
                    new PublishedCachesResolver(new PublishedCaches(mockPublishedContentCache.Object, new Mock<IPublishedMediaCache>().Object));

                // JSON test data taken from Umbraco unit-test:
                // https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Tests/PropertyEditors/ImageCropperTest.cs
                string json = "{\"focalPoint\": {\"left\": 0.96,\"top\": 0.80827067669172936},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\":\"thumb\",\"width\": 100,\"height\": 100,\"coordinates\": {\"x1\": 0.58729977382575338,\"y1\": 0.055768992440203169,\"x2\": 0,\"y2\": 0.32457553600198386}}]}";
                this.dataSet = JsonConvert.DeserializeObject<ImageCropDataSet>(json);
                this.link = new RelatedLink
                {
                    Caption = "Test Caption",
                    Content = null,
                    Id = 98765,
                    IsDeleted = true,
                    IsInternal = true,
                    Link = "test link",
                    NewWindow = true
                };

                if (!Resolution.IsFrozen)
                {
                    Resolution.Freeze();
                }
            }
        }

        public MockPublishedContent GetContent()
        {
            return new MockPublishedContent
            {
                Properties = new[]
                {
                    new MockPublishedContentProperty(nameof(PublishedItem.PublishedContent), 1000),
                    new MockPublishedContentProperty(nameof(PublishedItem.PublishedInterfaceContent), 1001),
                    new MockPublishedContentProperty(nameof(PublishedItem.Image), this.dataSet),
                    new MockPublishedContentProperty(nameof(PublishedItem.Child), 3333),

                    // We're deliberately switching these values to test enumerable conversion
                    new MockPublishedContentProperty(nameof(PublishedItem.RelatedLink), new RelatedLinks(new List<RelatedLink>{ this.link },nameof(PublishedItem.RelatedLink))),
                    new MockPublishedContentProperty(nameof(PublishedItem.RelatedLinks), this.link),
                    new MockPublishedContentProperty(nameof(PublishedItem.NullRelatedLinks), null),

                    // Polymorphic collections
                    new MockPublishedContentProperty(nameof(PublishedItem.Polymorphic),
                    new MockPublishedContent[]{

                        new MockPublishedContent()
                        {
                            DocumentTypeAlias = nameof(PolymorphicItemOne),
                            Properties = new[]{ new MockPublishedContentProperty(nameof(IPolyMorphic.PolyMorphicText),"Foo")}
                        },
                        new MockPublishedContent()
                        {
                            DocumentTypeAlias = nameof(PolymorphicItemTwo),
                            Properties = new[]{ new MockPublishedContentProperty(nameof(IPolyMorphic.PolyMorphicText),"Bar")}
                        }
                    })
                }
            };
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
            if (this.disposed)
            {
                return;
            }

            this.TearDown();
            this.disposed = true;
        }
    }
}