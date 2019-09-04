using System;
using System.Linq;
using System.Reflection;
using System.Web;
using Moq;
using UmbMapper.Umbraco8TestSupport.Objects;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;
using Current = Umbraco.Core.Composing.Current;

namespace UmbMapper.Umbraco8TestSupport.MockHelpers
{
    public class UmbracoContextHelper : IDisposable
    {
        private UmbracoContext umbracoContext;
        private IUmbracoContextAccessor umbracoContextAccessor;
        private IPublishedSnapshotService publishedSnapshotService;
        private IVariationContextAccessor variationContextAccessor;
        private IDefaultCultureAccessor defaultCultureAccessor;
        private IUmbracoSettingsSection umbracoSettingsSection;
        private IGlobalSettings globalSettings;
        private IUmbracoContextFactory umbracoContextFactory;
        private UrlProviderCollection urlProviders;
        private HttpContextBase httpContext;
        private IUserService userService;

        protected virtual UmbracoContext UmbracoContext
            => this.umbracoContext;

        public virtual IUmbracoContextFactory UmbracoContextFactory
            => this.umbracoContextFactory;

        public virtual void Initialise()
        {
            Current.Reset();

            // Initialise low level dependencies / mocks
            this.InitialiseUmbracoContextDependencies();

            // Initialsie context, needs low level dependencies
            this.InitialiseUmbracoContext();

            // Initialise context accessor, needs a context
            this.InitialiseUmbracoContextAccessor();

            // Initialise ctx factory to be injected into services to access things like culture info
            this.InitialiseUmbracoContextFactory();

            // Is this needed if we're injecting context factory into which ever services need it?
            Current.Factory = Mock.Of<IFactory>();
        }

        public virtual void InitialiseUmbracoContextDependencies()
        {
            this.variationContextAccessor = new TestVariationContextAccessor();
            this.defaultCultureAccessor = new TestDefaultCultureAccessor();
            this.umbracoSettingsSection = this.GetUmbracoSettings();
            this.globalSettings = Mock.Of<IGlobalSettings>();
            this.urlProviders = new UrlProviderCollection(Enumerable.Empty<IUrlProvider>());
            this.userService = Mock.Of<IUserService>();
            this.InitialisePublishedSnapshotService();
            this.httpContext = new Factories.FakeHttpContextFactory("~/Home").HttpContext;


        }

        public virtual void InitialiseUmbracoContext()
        {
            this.umbracoContext = this.GetUmbracoContext();
        }

        public virtual void InitialisePublishedSnapshotService()
        {
            var publishedSnapshotService = new Mock<IPublishedSnapshotService>();
            publishedSnapshotService.Setup(x => x.CreatePublishedSnapshot(It.IsAny<string>())).Returns(Mock.Of<IPublishedSnapshot>());
            this.publishedSnapshotService = publishedSnapshotService.Object;
        }

        public virtual void InitialiseUmbracoContextAccessor()
        {
            var contextAccessor = new Mock<IUmbracoContextAccessor>();
            contextAccessor.Setup(x => x.UmbracoContext).Returns(this.umbracoContext);

            this.umbracoContextAccessor = contextAccessor.Object;

            Umbraco.Web.Composing.Current.UmbracoContextAccessor = this.umbracoContextAccessor;
        }

        public virtual void InitialiseUmbracoContextFactory()
        {
            this.umbracoContextFactory = this.GetUmbracoContextFactory();
        }

        //public virtual void InitialiseUmbrcaco
        public virtual UmbracoContext GetUmbracoContext()
        {
            return this.GetUmbracoContext(
                this.httpContext,
                        this.publishedSnapshotService,
                        new WebSecurity(this.httpContext, this.userService, this.globalSettings),
                        this.umbracoSettingsSection,
                        this.urlProviders,
                        this.globalSettings,
                        this.variationContextAccessor);
        }

        public virtual UmbracoContext GetUmbracoContext(HttpContextBase httpContext, IPublishedSnapshotService publishedSnapshotService, WebSecurity webSecurity, IUmbracoSettingsSection umbracoSettingsSection, UrlProviderCollection urlProviders, IGlobalSettings globalSettings, IVariationContextAccessor variationContextAccessor)
        {
            ConstructorInfo umbracoContextCtor = typeof(UmbracoContext)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();

            object umbracoContextObject =
                umbracoContextCtor.Invoke(
                    new object[] {
                        httpContext,
                        publishedSnapshotService,
                        webSecurity,
                        umbracoSettingsSection,
                        urlProviders.ToList(),
                        globalSettings,
                        variationContextAccessor
                    }
                );

            return umbracoContextObject as UmbracoContext;
        }

        public virtual IUmbracoContextFactory GetUmbracoContextFactory()
        {
            return
                new UmbracoContextFactory
                (
                    this.umbracoContextAccessor,
                    this.publishedSnapshotService,
                    this.variationContextAccessor,
                    this.defaultCultureAccessor,
                    this.umbracoSettingsSection,
                    this.globalSettings,
                    this.urlProviders,
                    this.userService
                );
        }

        public virtual IUmbracoSettingsSection GetUmbracoSettings()
        {
            // Copied from Umbraco source
            // FIXME: Why not use the SettingsForTest.GenerateMock ... ?
            // FIXME: Shouldn't we use the default ones so they are the same instance for each test?

            var umbracoSettingsMock = new Mock<IUmbracoSettingsSection>();
            var webRoutingSectionMock = new Mock<IWebRoutingSection>();
            webRoutingSectionMock.Setup(x => x.UrlProviderMode).Returns(UrlProviderMode.Auto.ToString());
            umbracoSettingsMock.Setup(x => x.WebRouting).Returns(webRoutingSectionMock.Object);
            return umbracoSettingsMock.Object;
            
        }

        public void Dispose()
        {
            Current.Reset();
        }
    }
}
