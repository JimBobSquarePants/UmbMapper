using System;
using BenchmarkDotNet.Attributes;
using Our.Umbraco.Ditto;
using UmbMapper.Extensions;
using UmbMapper.Tests.Mapping;
using Umbraco.Core.Models;
using Umbraco.Web.Models;

namespace UmbMapper.Tests.Benchmarks
{
    public class PublishedPropertyMapping
    {
        private UmbracoSupport support;
        private IPublishedContent content;

        [GlobalSetup]
        public void Setup()
        {
            this.support = new UmbracoSupport();
            this.content = this.support.Content;
            UmbMapperRegistry.AddMapperFor<TestClass>();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.support?.Dispose();
        }

        [Benchmark]
        public int MapUsingUmbMapper()
        {
            return this.content.MapTo<TestClass>().Id;
        }

        [Benchmark(Baseline = true)]
        public int MapUsingDitto()
        {
            return this.content.As<TestClass>().Id;
        }

        public class TestClass
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public ImageCropDataSet Image { get; set; }

            public DateTime CreateDate { get; set; }

            public DateTime UpdateDate { get; set; }
        }
    }
}