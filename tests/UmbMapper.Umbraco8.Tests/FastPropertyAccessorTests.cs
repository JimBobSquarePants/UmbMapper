using System;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using Xunit;

namespace UmbMapper.Umbraco8.Tests
{
    public class FastPropertyAccessorTests
    {
        private readonly PublishedItem item;
        private readonly FastPropertyAccessor accessor;

        public FastPropertyAccessorTests()
        {
            this.item = new PublishedItem
            {
                Id = 1234,
                Name = "Foo",
                CreateDate = DateTime.MaxValue
            };

            this.accessor = new FastPropertyAccessor(typeof(PublishedItem));
        }

        [Fact]
        public void FastPropertyAccessorCanGetValue()
        {
            Assert.True(1234 == (int)this.accessor.GetValue(nameof(PublishedItem.Id).ToUpperInvariant(), this.item));
            Assert.True("Foo" == (string)this.accessor.GetValue(nameof(PublishedItem.Name).ToUpperInvariant(), this.item));
            Assert.True(DateTime.MaxValue == (DateTime)this.accessor.GetValue(nameof(PublishedItem.CreateDate).ToUpperInvariant(), this.item));
        }

        [Fact]
        public void FastPropertyAccessorCanSetValue()
        {
            this.accessor.SetValue(nameof(PublishedItem.Id), this.item, 4321);
            this.accessor.SetValue(nameof(PublishedItem.Name), this.item, "Bar");
            this.accessor.SetValue(nameof(PublishedItem.CreateDate), this.item, DateTime.MinValue);

            Assert.True(4321 == (int)this.accessor.GetValue(nameof(PublishedItem.Id).ToUpperInvariant(), this.item));
            Assert.True("Bar" == (string)this.accessor.GetValue(nameof(PublishedItem.Name).ToUpperInvariant(), this.item));
            Assert.True(DateTime.MinValue == (DateTime)this.accessor.GetValue(nameof(PublishedItem.CreateDate).ToUpperInvariant(), this.item));
        }
    }
}