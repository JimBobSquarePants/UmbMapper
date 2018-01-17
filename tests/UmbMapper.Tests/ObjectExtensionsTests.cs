// <copyright file="FastPropertyAccessorExpressions.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using UmbMapper.Extensions;
using Umbraco.Core;
using Xunit;

namespace UmbMapper.Tests
{
    /// <summary>
    /// Tests replicate the tests found in Umbraco source repository
    /// </summary>
    public class ObjectExtensionsTests
    {
        [Fact]
        public void CanParseStringToUnit()
        {
            string stringUnit = "1234px";
            object objUnit = "1234px";
            Attempt<Unit> result = stringUnit.UmbMapperTryConvertTo<Unit>();
            Attempt<Unit> result2 = objUnit.UmbMapperTryConvertTo<Unit>();
            var unit = new Unit("1234px");
            Assert.True(result.Success);
            Assert.True(result2.Success);
            Assert.Equal(unit, result.Result);
            Assert.Equal(unit, result2.Result);
        }

        [Fact]
        public void CanConvertListToEnumerable()
        {
            var list = new List<string>() { "hello", "world", "awesome" };
            Attempt<IEnumerable<string>> result = list.UmbMapperTryConvertTo<IEnumerable<string>>();
            Assert.True(result.Success);
            Assert.Equal(3, result.Result.Count());
        }

        [Fact]
        public void CanConvertIntToNullableInt()
        {
            int i = 1;
            Attempt<int?> result = i.UmbMapperTryConvertTo<int?>();
            Assert.True(result.Success);
        }

        [Fact]
        public void CanConvertNullableIntToInt()
        {
            int? i = 1;
            Attempt<int> result = i.UmbMapperTryConvertTo<int>();
            Assert.True(result.Success);
        }

        [Theory]
        [InlineData("TRUE", true)]
        [InlineData("True", true)]
        [InlineData("true", true)]
        [InlineData("1", true)]
        [InlineData("FALSE", false)]
        [InlineData("False", false)]
        [InlineData("false", false)]
        [InlineData("0", false)]
        [InlineData("", false)]
        public virtual void CanConvertStringToBool(string value, bool expected)
        {
            Attempt<bool> result = value.UmbMapperTryConvertTo<bool>();
            Assert.True(result.Success);
            Assert.Equal(expected, result.Result);
        }

        [Theory]
        [InlineData("2012-11-10", true)]
        [InlineData("2012/11/10", true)]
        [InlineData("10/11/2012", true)] // assuming your culture uses DD/MM/YYYY
        [InlineData("11/10/2012", false)] // assuming your culture uses DD/MM/YYYY
        [InlineData("Sat 10, Nov 2012", true)]
        [InlineData("Saturday 10, Nov 2012", true)]
        [InlineData("Sat 10, November 2012", true)]
        [InlineData("Saturday 10, November 2012", true)]
        [InlineData("2012-11-10 13:14:15", true)]
        [InlineData("2012-11-10T13:14:15Z", true)]
        public virtual void CanConvertStringToDateTime(string date, bool expected)
        {
            var dateTime = new DateTime(2012, 11, 10, 13, 14, 15);
            Attempt<DateTime> result = date.UmbMapperTryConvertTo<DateTime>();

            Assert.True(result.Success);
            Assert.Equal(expected, DateTime.Equals(dateTime.Date, result.Result.Date));
        }

        [Fact]
        public virtual void CanConvertBlankStringToNullDateTime()
        {
            Attempt<DateTime?> result = string.Empty.UmbMapperTryConvertTo<DateTime?>();
            Assert.True(result.Success);
            Assert.Null(result.Result);
        }

        [Fact]
        public virtual void CanConvertBlankStringToNullBool()
        {
            Attempt<bool?> result = string.Empty.UmbMapperTryConvertTo<bool?>();
            Assert.True(result.Success);
            Assert.Null(result.Result);
        }

        [Fact]
        public virtual void CanConvertBlankStringToDateTime()
        {
            Attempt<DateTime> result = string.Empty.UmbMapperTryConvertTo<DateTime>();
            Assert.True(result.Success);
            Assert.Equal(DateTime.MinValue, result.Result);
        }

        [Fact]
        public virtual void CanConvertObjectToStringUsingToStringOverload()
        {
            Attempt<string> result = new MyTestObject().UmbMapperTryConvertTo<string>();

            Assert.True(result.Success);
            Assert.Equal("Hello world", result.Result);
        }

        [Fact]
        public virtual void CanConvertObjectToSameObject()
        {
            var obj = new MyTestObject();
            Attempt<object> result = obj.UmbMapperTryConvertTo<object>();

            Assert.Equal(obj, result.Result);
        }

        private class MyTestObject
        {
            public override string ToString()
            {
                return "Hello world";
            }
        }
    }
}