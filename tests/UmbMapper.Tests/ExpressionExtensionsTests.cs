using System;
using UmbMapper.Extensions;
using Xunit;

namespace UmbMapper.Tests
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
            Assert.Throws(typeof(ArgumentException), () =>
            {
                ExpressionExtensions.ToPropertyInfo<Func<string, object>>(s => s.Clone());
            });
        }
    }
}