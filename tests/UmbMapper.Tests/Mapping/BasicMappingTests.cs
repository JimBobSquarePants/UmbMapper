using System;
using System.Collections.Generic;
using System.Linq;
using UmbMapper.Extensions;
using UmbMapper.Tests.Mapping.Models;
using UmbMapper.Tests.Mocks;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Xunit;

namespace UmbMapper.Tests.Mapping
{
    public class BasicMappingTests : IClassFixture<UmbracoSupport>
    {
        private readonly UmbracoSupport support;

        public BasicMappingTests(UmbracoSupport support)
        {
            this.support = support;
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


            PublishedItem result = content.MapTo<PublishedItem>();

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

            PublishedItem result = content.MapTo<PublishedItem>();

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

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.Equal(created, result.CreateDate);
            Assert.Equal(created, result.UpdateDate);
        }

        [Fact]
        public void MapperCanMapRelatedLinks()
        {
            MockPublishedContent content = this.support.Content;

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.NotNull(result.RelatedLink);
            Assert.NotNull(result.RelatedLinks);
            Assert.True(result.RelatedLinks.GetType().IsEnumerableOfType(typeof(RelatedLink)));
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

            AutoMappedItem result = content.MapTo<AutoMappedItem>();

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

            BackedPublishedItem result = content.MapTo<BackedPublishedItem>();

            Assert.NotNull(result);
            Assert.Equal(content.Id, result.Id);
            Assert.Equal(content.Name, result.Name);
            Assert.Equal(content.CreateDate, result.CreateDate);
            Assert.Equal(content.UpdateDate, result.UpdateDate);

            Assert.NotNull(result.Slug);
            Assert.True(result.Slug == result.Name.ToLowerInvariant());
            Assert.NotNull(result.Image);
            Assert.Equal(content.GetPropertyValue(nameof(BackedPublishedItem.Image)), result.Image);
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

            InheritedPublishedItem result = content.MapTo<InheritedPublishedItem>();

            Assert.NotNull(result);
            Assert.Equal(content.Id, result.Id);
            Assert.Equal(content.Name, result.Name);

            Assert.NotNull(result.Slug);
            Assert.True(result.Slug == result.Name.ToLowerInvariant());
            Assert.NotNull(result.Image);
            Assert.Equal(content.GetPropertyValue(nameof(BackedPublishedItem.Image)), result.Image);
        }

        [Fact]
        public void RecursivePropertiesCanBeInherited()
        {
            IUmbMapperConfig mapper = UmbMapperRegistry.CurrentMappers().First(x => x.MappedType == typeof(InheritedPublishedItem));
            IPropertyMap mapping = mapper.Mappings
                .First(x => x.Info.Aliases.Contains(nameof(InheritedPublishedItem.Name), StringComparer.InvariantCultureIgnoreCase));

            Assert.True(mapping.Info.Recursive);
        }

        [Fact]
        public void MapperCanRemoveMap()
        {
            var map = new LazyPublishedItemMap();
            int mapCount = map.Mappings.Count();

            bool result = map.Ignore(x => x.CreateDate);

            Assert.True(result);
            Assert.Equal(mapCount - 1, map.Mappings.Count());
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
            int singleExpected = 1;
            IEnumerable<int> enumerableExpected = new[] { 1 };

            MockPublishedContent content = this.support.Content;
            content.Properties = new List<IPublishedProperty>
            {
                new MockPublishedContentProperty(nameof(CsvPublishedItem.StringItems), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.SByteItems), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.ByteItems), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.ShortItems), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.UShortItems), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.IntItems), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.UIntItems), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.LongItems), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.ULongItems), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.FloatItems), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.DoubleItems), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.DecimalItems), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.EmptyItems), emptyInput),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.SingleItem), input),
                new MockPublishedContentProperty(nameof(CsvPublishedItem.EnumerableItems), singleInput)
            };

            CsvPublishedItem result = content.MapTo<CsvPublishedItem>();
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

        [Fact]
        public void MapperCanMapPolymorphicTypes()
        {
            MockPublishedContent content = this.support.Content;

            PublishedItem result = content.MapTo<PublishedItem>();

            Assert.True(result.Polymorphic.Any());
            Assert.Contains(result.Polymorphic, x => x.PolyMorphicText == "Foo");
            Assert.Contains(result.Polymorphic, x => x.PolyMorphicText == "Bar");
        }
    }
}