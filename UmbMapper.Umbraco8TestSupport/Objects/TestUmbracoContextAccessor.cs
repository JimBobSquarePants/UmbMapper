using Umbraco.Web;

namespace UmbMapper.Umbraco8TestSupport.Objects
{
    public class TestUmbracoContextAccessor : IUmbracoContextAccessor
    {
        public UmbracoContext UmbracoContext { get; set; }

        public TestUmbracoContextAccessor()
        {
        }

        public TestUmbracoContextAccessor(UmbracoContext umbracoContext)
        {
            UmbracoContext = umbracoContext;
        }
    }
}
