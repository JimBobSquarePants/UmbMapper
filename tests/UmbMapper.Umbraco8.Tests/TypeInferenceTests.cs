using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UmbMapper.Extensions;
using UmbMapper.Umbraco8.Tests.Mocks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;
using Xunit;

namespace UmbMapper.Umbraco8.Tests
{
    /// <summary>
    /// The type inference tests.
    /// </summary>
    public class TypeInferenceTests
    {
        /// <summary>
        /// Tests the <see cref="TypeInferenceExtensions.IsCollectionType"/> method.
        /// </summary>
        /// <param name="input">The input type</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(long), false)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(Enumerable), false)]
        [InlineData(typeof(Dictionary<string, string>), true)]
        public void TestIsCollectionType(Type input, bool expected)
        {
            Assert.Equal(expected, input.IsCollectionType());
        }

        /// <summary>
        /// Tests the <see cref="TypeInferenceExtensions.IsEnumerableType"/> method.
        /// </summary>
        /// <param name="input">The input type</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [InlineData(typeof(string), true)]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(long), false)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(Enumerable), false)]
        [InlineData(typeof(Dictionary<string, string>), true)]
        //TODO - does this need to be added? Probably not
        //[InlineData(typeof(RelatedLinks), true)]
        public void TestIsEnumerableType(Type input, bool expected)
        {
            Assert.Equal(expected, input.IsEnumerableType());
        }
        /// <summary>
        /// Tests the <see cref="TypeInferenceExtensions.IsCastableEnumerableType"/> method.
        /// </summary>
        /// <param name="input">The input type</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(long), false)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(Enumerable), false)]
        [InlineData(typeof(Dictionary<string, string>), false)]
        //TODO - does this need to be added? Probably not
        //[InlineData(typeof(RelatedLinks), true)]
        public void TestIsCastableEnumerableType(Type input, bool expected)
        {
            Assert.Equal(expected, input.IsCastableEnumerableType());
        }

        /// <summary>
        /// Tests the <see cref="TypeInferenceExtensions.IsConvertableEnumerableType"/> method.
        /// </summary>
        /// <param name="input">The input type</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(long), false)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(Enumerable), false)]
        [InlineData(typeof(Dictionary<string, string>), true)]
        //TODO - does this need to be added? Probably not
        //[InlineData(typeof(RelatedLinks), true)]
        public void TestIsConvertableEnumerableType(Type input, bool expected)
        {
            Assert.Equal(expected, input.IsConvertableEnumerableType());
        }

        /// <summary>
        /// Tests the <see cref="TypeInferenceExtensions.IsEnumerableOfKeyValueType"/> method.
        /// </summary>
        /// <param name="input">The input type</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(long), false)]
        [InlineData(typeof(List<string>), false)]
        [InlineData(typeof(Collection<string>), false)]
        [InlineData(typeof(HashSet<string>), false)]
        [InlineData(typeof(Enumerable), false)]
        [InlineData(typeof(Dictionary<string, string>), true)]
        [InlineData(typeof(List<KeyValuePair<string, string>>), true)]
        [InlineData(typeof(Collection<KeyValuePair<string, string>>), true)]
        [InlineData(typeof(HashSet<KeyValuePair<string, string>>), true)]
        public void TestIsEnumerableOfKeyValueType(Type input, bool expected)
        {
            Assert.Equal(expected, input.IsEnumerableOfKeyValueType());
        }

        /// <summary>
        /// Tests the <see cref="TypeInferenceExtensions.IsEnumerableOfType"/> method.
        /// </summary>
        /// <param name="input">The input type</param>
        /// <param name="argumentType">The argument type.</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [InlineData(typeof(IEnumerable<string>), typeof(string), true)]
        [InlineData(typeof(string[]), typeof(string), true)]
        [InlineData(typeof(IEnumerable<int>), typeof(int), true)]
        [InlineData(typeof(int[]), typeof(int), true)]
        [InlineData(typeof(string), typeof(string), false)]
        [InlineData(typeof(string), typeof(char), true)]
        [InlineData(typeof(Dictionary<string, string>), typeof(KeyValuePair<string, string>), true)]
        [InlineData(typeof(IEnumerable<MockPublishedContent>), typeof(IPublishedContent), true)]
        public void TestIsEnumerableOfType(Type input, Type argumentType, bool expected)
        {
            Assert.Equal(expected, input.IsEnumerableOfType(argumentType));
        }
    }
}
