using System.Globalization;
using UmbMapper.Umbraco8.Sample.Models.Pages;

namespace UmbMapper.Umbraco8.Sample.RenderModels
{
    /// <summary>
    /// Encapsulates properties required rendering pages with metadata.
    /// </summary>
    /// <typeparam name="T">The type of object to create the render model for.</typeparam>
    public interface IRenderPage<out T>
        where T : PublishedPage
    {
        /// <summary>
        /// Gets the content.
        /// </summary>
        T Content { get; }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        CultureInfo CurrentCulture { get; }
    }
}