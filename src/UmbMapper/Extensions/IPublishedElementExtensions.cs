using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper.Extensions
{
    /// <summary>
    /// Extension methods for IPublishedElement
    /// </summary>
    public static class IPublishedElementExtensions
    {
        /// <summary>
        /// Determines if an IPublishedElement objectd is a valid IPublishedContent item
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsIPublishedContent(this IPublishedElement element)
        {
            return
                typeof(IPublishedContent).IsAssignableFrom(element.GetType());

        }
    }
}
