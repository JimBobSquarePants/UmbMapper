// <copyright file="FastPropertyAccessorExpressions.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using UmbMapper.Extensions;
using Umbraco.Core;

namespace UmbMapper.Tests.Benchmarks
{
    [MemoryDiagnoser]
    public class TryConvertTo
    {
        private static readonly List<string> List = new List<string>() { "hello", "world", "awesome" };
        private static readonly string Date = "Saturday 10, November 2012";

        [Benchmark(Description = "Umbraco: List<string> to IEnumerable<string>", Baseline = true)]
        public IEnumerable<string> UmbracoTryConvertToEnumerable()
        {
            return List.TryConvertTo<IEnumerable<string>>().Result;
        }

        [Benchmark(Description = "UmbMapper: List<string> to IEnumerable<string>")]
        public IEnumerable<string> UmbMapperTryConvertToEnumerable()
        {
            return List.UmbMapperTryConvertTo<IEnumerable<string>>().Result;
        }

        [Benchmark(Description = "Umbraco: Int to Double")]
        public double UmbracoTryConvertToDouble()
        {
            return 1.TryConvertTo<double>().Result;
        }

        [Benchmark(Description = "UmbMapper: Int to Double")]
        public double UmbMapperTryConvertToToDouble()
        {
            return 1.UmbMapperTryConvertTo<double>().Result;
        }

        [Benchmark(Description = "Umbraco: Float to Decimal")]
        public decimal UmbracoTryConvertToDecimal()
        {
            return 1F.TryConvertTo<decimal>().Result;
        }

        [Benchmark(Description = "UmbMapper: Float to Decimal")]
        public decimal UmbMapperTryConvertToToDecimal()
        {
            return 1F.UmbMapperTryConvertTo<decimal>().Result;
        }

        [Benchmark(Description = "Umbraco: String to Boolean")]
        public bool UmbracoTryConvertToBoolean()
        {
            return "1".TryConvertTo<bool>().Result;
        }

        [Benchmark(Description = "UmbMapper: String to Boolean")]
        public bool UmbMapperTryConvertToBoolean()
        {
            return "1".UmbMapperTryConvertTo<bool>().Result;
        }

        [Benchmark(Description = "Umbraco: String to DateTime")]
        public DateTime UmbracoTryConvertToDateTime()
        {
            return Date.TryConvertTo<DateTime>().Result;
        }

        [Benchmark(Description = "UmbMapper: String to DateTime")]
        public DateTime UmbMapperTryConvertToToDateTime()
        {
            return Date.UmbMapperTryConvertTo<DateTime>().Result;
        }
    }
}
