using System.Globalization;
using UmbMapper.PropertyMappers;
using Umbraco.Web;

namespace UmbMapper.Models
{
    public class MappingContext
    {
        private readonly IUmbracoContextFactory umbracoContextFactory;
        public MappingContext(IUmbracoContextFactory umbracoContextFactory)
        {
            this.umbracoContextFactory = umbracoContextFactory;
        }

        public IUmbracoContextFactory UmbracoContextFactory
            => this.umbracoContextFactory;

        public CultureInfo GetRequestCultureInfo()
        {
            // Create MappingContextFactory to creat an instance of this
            // which can then be passed around as and when it's needed
            //TODO - only here until IUmbracoContextFactory can be mocked
            //return Umbraco.Web.Composing.Current.UmbracoContext?.PublishedRequest?.Culture;
            using (UmbracoContextReference ctx = this.umbracoContextFactory.EnsureUmbracoContext())
            {
                return ctx.UmbracoContext?.PublishedRequest?.Culture;
            }
        }
    }
}
