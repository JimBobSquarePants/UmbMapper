using Umbraco.Core.PropertyEditors.ValueConverters;

namespace UmbMapper.Umbraco8.Sample.Models.UmbracoDocTypes
{
    public class BasicImage
    {
        public ImageCropperValue UmbracoFile { get; set; }
        public string Url { get; set; }
    }
}