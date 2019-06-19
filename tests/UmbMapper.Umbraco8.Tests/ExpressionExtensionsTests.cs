using System;
using UmbMapper.Extensions;
using Xunit;

namespace UmbMapper.Umbraco8.Tests
{
    public class ExpressionExtensionsTests
    {
        [Fact]
        public void CanResolveProperty()
        {
            var result = ExpressionExtensions.ToPropertyInfo<Func<string, object>>(s => s.Length);

            Assert.NotNull(result);
        }   

        [Fact]
        public void ThrowsWhenPassedNonProperty()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ExpressionExtensions.ToPropertyInfo<Func<string, object>>(s => s.Clone());
            });
        }
    }
}