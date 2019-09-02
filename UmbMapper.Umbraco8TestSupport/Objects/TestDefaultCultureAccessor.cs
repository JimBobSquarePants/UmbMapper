using Umbraco.Web.PublishedCache;

namespace UmbMapper.Umbraco8TestSupport.Objects
{
    public class TestDefaultCultureAccessor : IDefaultCultureAccessor
    {
        private string _defaultCulture = string.Empty;

        public string DefaultCulture
        {
            get => _defaultCulture;
            set => _defaultCulture = value ?? string.Empty;
        }
    }
}
