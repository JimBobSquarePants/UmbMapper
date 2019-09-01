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
            using (UmbracoContextReference ctx = this.umbracoContextFactory.EnsureUmbracoContext())
            {
                return ctx.UmbracoContext?.PublishedRequest?.Culture;
            }
        }
    }
}
