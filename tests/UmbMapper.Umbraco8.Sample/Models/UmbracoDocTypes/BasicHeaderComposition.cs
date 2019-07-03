using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.Web.Models;

namespace UmbMapper.Umbraco8.Sample.Models.UmbracoDocTypes
{
    public class BasicHeaderComposition
    {
        //public BasicImage Image { get; set; }
        public BasicImage Image { get; set; }
        public string Strapline { get; set; }
        public Link Link { get; set; }
    }
}