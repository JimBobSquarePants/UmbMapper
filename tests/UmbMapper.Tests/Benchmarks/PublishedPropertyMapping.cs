using System;
using BenchmarkDotNet.Attributes;
using Our.Umbraco.Ditto;
using UmbMapper.Extensions;
using UmbMapper.Tests.Mapping;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Zone.UmbracoMapper;

namespace UmbMapper.Tests.Benchmarks
{
    public class PublishedPropertyMapping
    {
        private UmbracoSupport support;
        private IPublishedContent content;
        private IUmbracoMapper umbracoMapper;

        [GlobalSetup]
        public void Setup()
        {
            this.support = new UmbracoSupport();
            this.content = this.support.Content;

            this.umbracoMapper = new UmbracoMapper();
            UmbMapperRegistry.AddMapperFor<TestClass>();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.support?.Dispose();
        }

        [Benchmark(Baseline = true)]
        public int ManualMapping()
        {
            var testClass = new TestClass
            {
                Id = this.content.Id,
                Name = this.content.Name,
                Image = this.content.GetPropertyValue<ImageCropDataSet>("image"),
                CreateDate = this.content.CreateDate,
                UpdateDate = this.content.UpdateDate
            };

            return testClass.Id;
        }

        [Benchmark]
        public int MapUsingUmbMapper()
        {
            return this.content.MapTo<TestClass>().Id;
        }

        [Benchmark]
        public int MapUsingUmbracoMapper()
        {
            var testClass = new TestClass();
            this.umbracoMapper.Map(this.content, testClass);
            return testClass.Id;
        }

        [Benchmark]
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