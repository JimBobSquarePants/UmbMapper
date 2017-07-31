namespace UmbMapper.Sample.Models.Media
{
    /// <summary>
    /// Defines a contract specifiying the properties of a file in the media section.
    /// </summary>
    public interface IPublishedFile
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Gets or sets the size of the media file in bytes.
        /// </summary>
        int Bytes { get; set; }

        /// <summary>
        /// Gets or sets the file extension.
        /// </summary>
        string Extension { get; set; }
    }
}