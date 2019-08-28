using System;
using UmbMapper.Extensions;
using UmbMapper.Umbraco8.Tests.Mocks;
using Xunit;
using Umbraco.Web;
using Umbraco.Web.Models;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using System.Linq;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;
using Moq;
using UmbMapper.Factories;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class BasicMappingTests : IClassFixture<UmbracoSupport>
    {
        private readonly UmbracoSupport support;
        private readonly IUmbMapperRegistry umbMapperRegistry;
        private readonly IUmbMapperService umbMapperService;

        public BasicMappingTests(UmbracoSupport support)
        {
            this.support = support;
            this.support.SetupUmbracoContext();

            this.umbMapperRegistry = new UmbMapperRegistry(Mock.Of<IUmbracoContextFactory>());
            this.support.InitMappers(this.umbMapperRegistry);

            this.umbMapperService = new UmbMapperService(this.umbMapperRegistry, new MappingProcessorFactory());
        }

        [Fact]
        public void MapperCanMapBaseProperties()
        {
            const int id = 999;
            const string name = "Foo";
            var created = new DateTime(2017, 1, 1);

            MockPublishedContent content = this.support.Content;
            content.Id = id;
            content.Name = name;
            content.CreateDate = created;

            PublishedItem result = this.umbMapperService.MapTo<PublishedItem>(content);

            Assert.Equal(id, result.Id);
            Assert.Equal(name, result.Name);
            Assert.Equal(created, result.CreateDate);
        }

        [Fact]
        public void MapperReturnsDefaultProperties()
        {
            const int id = default(int);
            const string name = default(string);
            var created = default(DateTime);
            var updated = default(DateTime);

            MockPublishedContent content = this.support.Content;
            content.Id = id;
            content.Name = name;
            content.CreateDate = created;

            PublishedItem result = this.umbMapperService.MapTo<PublishedItem>(content);

            Assert.Equal(id, result.Id);
            Assert.Equal(name, result.Name);
            Assert.Equal(created, result.CreateDate);
            Assert.Equal(updated, result.UpdateDate);
        }

        [Fact]
        public void MapperCanMapBaseAlternativeProperties()
        {
            var created = new DateTime(2017, 1, 1);

            MockPublishedContent content = this.support.Content;
            content.CreateDate = created;

            PublishedItem result = this.umbMapperService.MapTo<PublishedItem>(content);

            Assert.Equal(created, result.CreateDate);
            Assert.Equal(created, result.UpdateDate);
        }

        [Fact]
        public void MapperCanMapLinks()
        {
            MockPublishedContent content = this.support.Content;

            PublishedItem result = this.umbMapperService.MapTo<PublishedItem>(content);

            Assert.NotNull(result.Link);
            Assert.NotNull(result.Links);
            Assert.True(result.Links.GetType().IsEnumerableOfType(typeof(Link)));
        }

        [Fact]
        public void MapperCanMapAutoMappedProperties()
        {
            MockPublishedContent content = this.support.Content;
            var created = new DateTime(2017, 1, 1);
            content.Id = 98765;
            content.Name = "AutoMapped";
            content.CreateDate = created;
            content.UpdateDate = created;

            AutoMappedItem result = this.umbMapperService.MapTo<AutoMappedItem>(content);

            Assert.NotNull(result);
            Assert.Equal(content.Id, result.Id);
            Assert.Equal(content.Name, result.Name);
            Assert.Equal(content.CreateDate, result.CreateDate);
            Assert.Equal(content.UpdateDate, result.UpdateDate);
        }

        [Fact]
        public void MapperCanMapPublishedModelType()
        {
            MockPublishedContent content = this.support.Content;
            var created = new DateTime(2017, 1, 1);
            content.Id = 98765;
            content.Name = "BackMapped";
            content.CreateDate = created;
            content.UpdateDate = created;

            BackedPublishedItem result = this.umbMapperService.MapTo<BackedPublishedItem>(content);

            Assert.NotNull(result);
            Assert.Equal(content.Id, result.Id);
            Assert.Equal(content.Name, result.Name);
            Assert.Equal(content.CreateDate, result.CreateDate);
            Assert.Equal(content.UpdateDate, result.UpdateDate);

            Assert.NotNull(result.Slug);
            Assert.True(result.Slug == result.Name.ToLowerInvariant());
            Assert.NotNull(result.Image);
            Assert.Equal(content.GetProperty(nameof(BackedPublishedItem.Image))?.GetValue(), result.Image);
        }

        [Fact]
        public void MapperCanMapCsvValues()
        {
            const string input = "1,0,1.234";
            const string singleInput = "1.234";
            const string emptyInput = "   ";
            IEnumerable<string> stringExpected = new[] { "1", "0", "1.234" };
            IEnumerable<sbyte> sbyteExpected = new sbyte[] { 1, 0, 1 };
            IEnumerable<byte> byteExpected = new byte[] { 1, 0, 1 };
            IEnumerable<short> shortExpected = new short[] { 1, 0, 1 };
            IEnumerable<ushort> ushortExpected = new ushort[] { 1, 0, 1 };
            IEnumerable<int> intExpected = new[] { 1, 0, 1 };
            IEnumerable<uint> uintExpected = new uint[] { 1, 0, 1 };
            IEnumerable<long> longExpected = new long[] { 1, 0, 1 };
            IEnumerable<ulong> ulongExpected = new ulong[] { 1, 0, 1 };
            IEnumerable<float> floatExpected = new float[] { 1, 0, 1.234F };
            IEnumerable<double> doubleExpected = new double[] { 1, 0, 1.234 };
            IEnumerable<decimal> decimalExpected = new decimal[] { 1, 0, 1.234M };
            IEnumerable<int> nullExpected = Enumerable.Empty<int>();
            IEnumerable<int> emptyExpected = Enumerable.Empty<int>();
            const int singleExpected = 1;
            IEnumerable<int> enumerableExpected = new[] { 1 };

            MockPublishedContent content = this.support.Content;
            content.Properties = new List<IPublishedProperty>
            {
                //MockFactory.CreateMockPublishedProperty
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.StringItems), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.SByteItems), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.ByteItems), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.ShortItems), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.UShortItems), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.IntItems), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.UIntItems), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.LongItems), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.ULongItems), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.FloatItems), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.DoubleItems), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.DecimalItems), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.EmptyItems), emptyInput),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.SingleItem), input),
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(CsvPublishedItem.EnumerableItems), singleInput)
            };

            CsvPublishedItem result = this.umbMapperService.MapTo<CsvPublishedItem>(content);

            Assert.Equal(stringExpected, result.StringItems);
            Assert.Equal(sbyteExpected, result.SByteItems);
            Assert.Equal(byteExpected, result.ByteItems);
            Assert.Equal(shortExpected, result.ShortItems);
            Assert.Equal(ushortExpected, result.UShortItems);
            Assert.Equal(intExpected, result.IntItems);
            Assert.Equal(uintExpected, result.UIntItems);
            Assert.Equal(longExpected, result.LongItems);
            Assert.Equal(ulongExpected, result.ULongItems);
            Assert.Equal(floatExpected, result.FloatItems);
            Assert.Equal(doubleExpected, result.DoubleItems);
            Assert.Equal(decimalExpected, result.DecimalItems);
            Assert.Equal(nullExpected, result.NullItems);
            Assert.Equal(emptyExpected, result.EmptyItems);
            Assert.Equal(singleExpected, result.SingleItem);
            Assert.Equal(enumerableExpected, result.EnumerableItems);
        }

        //TODO MapperCanMapPolymorphicTypes
        // Need to get umbMapperRegistry in FactoryPropertyMapperBase
        //[Fact]
        //public void MapperCanMapPolymorphicTypes()
        //{
        //    MockPublishedContent content = this.support.Content;

        //    PublishedItem result = this.umbMapperService.MapTo<PublishedItem>(content);

        //    Assert.True(result.Polymorphic.Any());
        //    Assert.Contains(result.Polymorphic, x => x.PolyMorphicText == "Foo");
        //    Assert.Contains(result.Polymorphic, x => x.PolyMorphicText == "Bar");
        //}

        [Fact]
        public void MapperCanMapToExistingInstance()
        {
            const int id = 999;
            const string name = "Foo";
            var created = new DateTime(2017, 1, 1);
            const PlaceOrder placeOrder = PlaceOrder.Second;

            MockPublishedContent content = this.support.Content;
            content.Id = id;
            content.Name = name;
            content.CreateDate = created;
            content.Properties = new List<IPublishedProperty>
            {
                UmbMapperMockFactory.CreateMockPublishedProperty(nameof(PublishedItem.PlaceOrder), PlaceOrder.Fourth)
            };

            PublishedItem result = this.umbMapperRegistry.CreateEmpty<PublishedItem>();

            // Set a value before mapping.
            result.PlaceOrder = placeOrder;

            this.umbMapperService.MapTo(content, result);

            Assert.Equal(id, result.Id);
            Assert.Equal(name, result.Name);
            Assert.Equal(created, result.CreateDate);

            // We expect it to be overwritten
            Assert.NotEqual(placeOrder, result.PlaceOrder);
        }

        [Fact]
        public void MapperCanMapInheritedMixedItems()
        {
            MockPublishedContent content = this.support.Content;
            var created = new DateTime(2017, 1, 1);
            content.Id = 98765;
            content.Name = "InheritedMapped";
            content.CreateDate = created;
            content.UpdateDate = created;

            InheritedPublishedItem result = this.umbMapperService.MapTo<InheritedPublishedItem>(content); // content.MapTo<InheritedPublishedItem>();

            Assert.NotNull(result);
            Assert.Equal(content.Id, result.Id);
            Assert.Equal(content.Name, result.Name);

            Assert.NotNull(result.Slug);
            Assert.True(result.Slug == result.Name.ToLowerInvariant());
            Assert.NotNull(result.Image);
            Assert.Equal(content.Value(nameof(BackedPublishedItem.Image)), result.Image);
        }

        //[Fact]
        //public void MapperCanRemoveMap()
        //{
        //    var map = new LazyPublishedItemMap();
        //    int mapCount = map.Mappings.Count();

        //    bool result = map.Ignore(x => x.CreateDate);

        //    Assert.True(result);
        //    Assert.Equal(mapCount - 1, map.Mappings.Count());
        //}

        //[Fact]
        //public void RecursivePropertiesCanBeInherited()
        //{
        //    IUmbMapperConfig mapper = UmbMapperRegistry.CurrentMappers().First(x => x.MappedType == typeof(InheritedPublishedItem));
        //    IPropertyMap mapping = mapper.Mappings
        //        .First(x => x.Info.Aliases.Contains(nameof(InheritedPublishedItem.Name), StringComparer.InvariantCultureIgnoreCase));

        //    Assert.True(mapping.Info.Recursive);
        //}
    }
}
