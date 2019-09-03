using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using Moq;
using UmbMapper.Umbraco8TestSupport.Objects;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Dictionary;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;
using Current = Umbraco.Core.Composing.Current;

namespace UmbMapper.Umbraco8TestSupport.MockHelpers
{
    public class UmbracoContextHelper
    {
        private UmbracoContext umbracoContext;
        protected virtual UmbracoContext UmbracoContext
            => this.umbracoContext;


        private IUmbracoContextAccessor umbracoContextAccessor;
        private IPublishedSnapshotService publishedSnapshotService;
        private IVariationContextAccessor variationContextAccessor;
        private IDefaultCultureAccessor defaultCultureAccessor;
        private IUmbracoSettingsSection umbracoSettingsSection;
        private IGlobalSettings globalSettings;
        private UrlProviderCollection urlProviders;
        private HttpContextBase httpContext;
        private IUserService userService;

        public virtual void Initialise()
        {
            this.InitialiseUmbracoContextDependencies();
            this.InitialiseUmbracoContext();
            this.InitialiseUmbracoContextAccessor();
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
        //public virtual void InitializeUmbracoContextMock()
        //{
        //    ResetUmbracoContext();
        //    // See Umbraco.Tests.Testing.UmbracoTestBase
        //    var typeLoader = new TypeLoader(NoAppCache.Instance, IOHelper.MapPath("~/App_Data/TEMP"), new ProfilingLogger(Mock.Of<ILogger>(), Mock.Of<IProfiler>()));
        //    var runtimeState = Mock.Of<IRuntimeState>();
        //    var register = RegisterFactory.Create();
        //    var composition = new Composition(register, typeLoader, Mock.Of<IProfilingLogger>(), runtimeState);

        //    Compose(composition);
        //    Current.Factory = composition.CreateFactory();
        //    // Initialize()

        //}

        //public virtual void Teardown()
        //{
        //    Current.Reset();
        //}

        //private void ResetUmbracoContext()
        //{
        //    // Calling internal static method: Umbraco.Core.Composing.Current.Reset();
        //    var method = typeof(Current).GetMethod("Reset");
        //    method.Invoke(null, new object[] { });
        //}

        //protected virtual UmbracoContext GetUmbracoContext()
        //{
        //    return UmbracoTestObjects.GetUmbracoContextMock();
        //}

        //protected virtual void Compose(Composition composition)
        //{
        //    var umbracoContext = GetUmbracoContext();

        //    ConstructorInfo publishedRequestCtor = typeof(PublishedRequest)
        //        .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();

        //    object publishedRequestObject =
        //        publishedRequestCtor.Invoke(
        //            new object[] {
        //                Mock.Of<IPublishedRouter>(),
        //                umbracoContext,
        //                null
        //            });

        //    umbracoContext.PublishedRequest = publishedRequestObject as PublishedRequest;
        //    umbracoContext.PublishedRequest.Culture = new CultureInfo("en-US");

        //    var contextAccessor = new Mock<IUmbracoContextAccessor>();
        //    contextAccessor.Setup(x => x.UmbracoContext).Returns(umbracoContext);

        //    Umbraco.Web.Composing.Current.UmbracoContextAccessor = contextAccessor.Object;



        //    var membershipHelper = CreateMembershipHelper(umbracoContext);
        //    var umbracoHelper = CreateUmbracoHelper(membershipHelper);

        //    composition.RegisterUnique(Mock.Of<IUmbracoContextAccessor>());
        //    composition.RegisterUnique(Mock.Of<ISqlContext>());
        //    //composition.Register(UmbracoTestObjects.GetServiceContextMock());
        //    composition.Register(AppCaches.Disabled);
        //    composition.RegisterUnique(Mock.Of<IProfilingLogger>());
        //    composition.RegisterUnique(Mock.Of<IRuntimeState>());
            
        //    composition.Register(_ => Mock.Of<IMemberService>());
        //    composition.Register(_ => Mock.Of<IMemberTypeService>());
        //    composition.Register(_ => Mock.Of<IUserService>());
        //    composition.Register(_ => Mock.Of<IUmbracoDatabaseFactory>());
        //    composition.Register(_ => Mock.Of<ILogger>());
        //    composition.Register(_ => AppCaches.Disabled);
        //    composition.Register<ServiceContext>();
        //    composition.Register(membershipHelper);
        //    composition.Register(umbracoHelper);

        //    composition.RegisterUnique<IUmbracoContextFactory, UmbracoContextFactory>();
        //}

        //protected virtual MembershipHelper CreateMembershipHelper(UmbracoContext umbracoContext)
        //{
        //    var membershipHelper = new MembershipHelper(umbracoContext.HttpContext, Mock.Of<IPublishedMemberCache>(),
        //        Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>(), Mock.Of<IMemberService>(),
        //        Mock.Of<IMemberTypeService>(), Mock.Of<IUserService>(),
        //        Mock.Of<IPublicAccessService>(), Mock.Of<AppCaches>(), Mock.Of<ILogger>());

        //    return membershipHelper;
        //}

        //protected virtual UmbracoHelper CreateUmbracoHelper(MembershipHelper membershipHelper)
        //{
        //    var umbracoHelper = new Mock<UmbracoHelper>(Mock.Of<IPublishedContent>(),
        //        Mock.Of<ITagQuery>(),
        //        Mock.Of<ICultureDictionaryFactory>(),
        //        Mock.Of<IUmbracoComponentRenderer>(),
        //        Mock.Of<IPublishedContentQuery>(),
        //        membershipHelper);



        //    return umbracoHelper.Object;
        //}
    }
}
