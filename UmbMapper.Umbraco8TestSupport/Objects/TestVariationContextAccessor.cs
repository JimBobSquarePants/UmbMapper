using Umbraco.Core.Models.PublishedContent;


namespace UmbMapper.Umbraco8TestSupport.Objects
{
    /// <summary>
    /// Provides an implementation of <see cref="IVariationContextAccessor"/> for tests.
    /// </summary>
    public class TestVariationContextAccessor : IVariationContextAccessor
    {
        /// <inheritdoc />
        public VariationContext VariationContext { get; set; }
    }
}
