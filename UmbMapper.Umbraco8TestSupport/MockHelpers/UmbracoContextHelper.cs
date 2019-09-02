using System.Web.Security;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Dictionary;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Security;
using Current = Umbraco.Core.Composing.Current;

namespace UmbMapper.Umbraco8TestSupport.MockHelpers
{
    public class UmbracoContextHelper
    {
        public virtual void InitializeUmbracoContextMock()
        {
            ResetUmbracoContext();
            // See Umbraco.Tests.Testing.UmbracoTestBase
            var typeLoader = new TypeLoader(NoAppCache.Instance, IOHelper.MapPath("~/App_Data/TEMP"), new ProfilingLogger(Mock.Of<ILogger>(), Mock.Of<IProfiler>()));
            var runtimeState = Mock.Of<IRuntimeState>();
            var register = RegisterFactory.Create();
            var composition = new Composition(register, typeLoader, Mock.Of<IProfilingLogger>(), runtimeState);

            Compose(composition);
            Current.Factory = composition.CreateFactory();
            // Initialize()

        }

        public virtual void Teardown()
        {
            Current.Reset();
        }

        private void ResetUmbracoContext()
        {
            // Calling internal static method: Umbraco.Core.Composing.Current.Reset();
            var method = typeof(Current).GetMethod("Reset");
            method.Invoke(null, new object[] { });
        }

        protected virtual UmbracoContext GetUmbracoContext()
        {
            return UmbracoTestObjects.GetUmbracoContextMock();
        }

        protected virtual void Compose(Composition composition)
        {
            var umbracoContext = GetUmbracoContext();

            var membershipHelper = CreateMembershipHelper(umbracoContext);
            var umbracoHelper = CreateUmbracoHelper(membershipHelper);

            composition.RegisterUnique(Mock.Of<IUmbracoContextAccessor>());
            composition.RegisterUnique(Mock.Of<ISqlContext>());
            //composition.Register(UmbracoTestObjects.GetServiceContextMock());
            composition.Register(AppCaches.Disabled);
            composition.RegisterUnique(Mock.Of<IProfilingLogger>());
            composition.RegisterUnique(Mock.Of<IRuntimeState>());
            composition.Register(_ => Mock.Of<IMemberService>());
            composition.Register(_ => Mock.Of<IMemberTypeService>());
            composition.Register(_ => Mock.Of<IUserService>());
            composition.Register(_ => Mock.Of<IUmbracoDatabaseFactory>());
            composition.Register(_ => Mock.Of<ILogger>());
            composition.Register(_ => AppCaches.Disabled);
            composition.Register<ServiceContext>();
            composition.Register(membershipHelper);
            composition.Register(umbracoHelper);
        }

        protected virtual MembershipHelper CreateMembershipHelper(UmbracoContext umbracoContext)
        {
            var membershipHelper = new MembershipHelper(umbracoContext.HttpContext, Mock.Of<IPublishedMemberCache>(),
                Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>(), Mock.Of<IMemberService>(),
                Mock.Of<IMemberTypeService>(), Mock.Of<IUserService>(),
                Mock.Of<IPublicAccessService>(), Mock.Of<AppCaches>(), Mock.Of<ILogger>());

            return membershipHelper;
        }

        protected virtual UmbracoHelper CreateUmbracoHelper(MembershipHelper membershipHelper)
        {
            var umbracoHelper = new Mock<UmbracoHelper>(Mock.Of<IPublishedContent>(),
                Mock.Of<ITagQuery>(),
                Mock.Of<ICultureDictionaryFactory>(),
                Mock.Of<IUmbracoComponentRenderer>(),
                Mock.Of<IPublishedContentQuery>(),
                membershipHelper);



            return umbracoHelper.Object;
        }
    }
}
