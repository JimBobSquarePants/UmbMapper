using Umbraco.Core.PropertyEditors.ValueConverters;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class InheritedPublishedItem : BasePublishedItem
    {
        public virtual string Slug { get; set; }

        public virtual ImageCropperValue Image { get; set; }
    }
}
