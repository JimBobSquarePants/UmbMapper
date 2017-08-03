using System;
using System.Collections;
using System.Collections.Generic;
using Umbraco.Web.Models;
using Xunit;

namespace UmbMapper.Tests
{
    public class RelatedLinksTests
    {
        [Fact]
        public void CanConvertRelatedLinksToRelatedLink()
        {
            var links = new RelatedLinks(new List<RelatedLink> { new RelatedLink() }, "Test");

            // Property is not enumerable, but value is, so grab first item
            IEnumerator enumerator = ((IEnumerable)links).GetEnumerator();
            object link = enumerator.MoveNext() ? enumerator.Current : null;

            Assert.NotNull(link);
        }

        [Fact]
        public void CanConvertRelatedLinkToRelatedLinks()
        {
            var link = new RelatedLink();

            var array = Array.CreateInstance(link.GetType(), 1);
            array.SetValue(link, 0);

            Assert.NotNull(array);
            IEnumerator enumerator = array.GetEnumerator();
            enumerator.MoveNext();
            Assert.True(enumerator.Current == link);
        }
    }
}
