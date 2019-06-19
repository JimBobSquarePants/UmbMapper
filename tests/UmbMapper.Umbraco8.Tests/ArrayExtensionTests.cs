// <copyright file="ArrayExtensionTests.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.Extensions;
using Xunit;

namespace UmbMapper.Umbraco8.Tests
{
    public class ArrayExtensionTests
    {
        [Fact]
        public void ArrayCanRemoveItemAtIndex()
        {
            int[] input = new int[] { 1, 2, 3, 4, 5 };
            int[] expected = new int[] { 1, 2, 4, 5 };
            int[] actual = input.RemoveAt(2);

            Assert.Equal(expected, actual);
        }
    }
}
