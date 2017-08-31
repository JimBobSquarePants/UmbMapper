using Umbraco.Web.Models;

namespace UmbMapper.Tests.Mapping.Models
{
    public class InheritedPublishedItem : BasePublishedItem
    {
        public virtual string Slug { get; set; }

        public virtual ImageCropDataSet Image { get; set; }
    }
}
