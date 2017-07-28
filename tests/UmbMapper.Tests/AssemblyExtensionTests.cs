using System;
using App_Code;
using UmbMapper.Extensions;
using Xunit;

namespace UmbMapper.Tests
{
    public class AssemblyExtensionTests
    {
        /// <summary>
        /// TODO: We can't test for truthy values here as App_Code is a special Asp.Net folder
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="expected">The expected result</param>
        [Theory]
        [InlineData(typeof(AssemblyExtensionTests), false)]
        public void CanCorrectlyCheckIsAppCodeAssembly(Type type, bool expected)
        {
            Assert.Equal(expected, type.Assembly.IsAppCodeAssembly());
        }
    }
}