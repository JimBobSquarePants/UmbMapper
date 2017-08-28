using UmbMapper.Tests.Mocks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;

namespace UmbMapper.Tests.Mapping.Models
{
    public class BackedPublishedItem : PublishedContentModel
    {
        public BackedPublishedItem(IPublishedContent content)
            : base(content)
        {
        }

        public virtual string Slug { get; set; }

        public virtual PlaceOrder PlaceOrder { get; set; }

        public virtual IPublishedContent PublishedInterfaceContent { get; set; }

        public virtual MockPublishedContent PublishedContent { get; set; }

        public virtual ImageCropDataSet Image { get; set; }

        public virtual PublishedItem Child { get; set; }
    }
}
