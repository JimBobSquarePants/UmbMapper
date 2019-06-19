using System.Linq;

namespace UmbMapper.Helpers
{
    /// <summary>
    /// This class is here as the Umbraco Umbraco.Core.Xml.XmlHelper class
    /// was marked as internal for V8 and is needed here
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Determines whether the specified string appears to be XML.
        /// </summary>
        /// <param name="xml">The XML string.</param>
        /// <returns>
        ///     <c>true</c> if the specified string appears to be XML; otherwise, <c>false</c>.
        /// </returns>
        public static bool CouldItBeXml(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return false;

            xml = xml.Trim();
            return xml.StartsWith("<") && xml.EndsWith(">") && xml.Contains('/');
        }
    }
}
