using System.Collections.Generic;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class CsvPublishedItem
    {
        public virtual IEnumerable<string> StringItems { get; set; }
        public IEnumerable<sbyte> SByteItems { get; set; }
        public IEnumerable<byte> ByteItems { get; set; }
        public IEnumerable<short> ShortItems { get; set; }
        public IEnumerable<ushort> UShortItems { get; set; }
        public IEnumerable<int> IntItems { get; set; }
        public IEnumerable<uint> UIntItems { get; set; }
        public IEnumerable<long> LongItems { get; set; }
        public IEnumerable<ulong> ULongItems { get; set; }
        public IEnumerable<float> FloatItems { get; set; }
        public IEnumerable<double> DoubleItems { get; set; }
        public IEnumerable<decimal> DecimalItems { get; set; }
        public IEnumerable<int> NullItems { get; set; }
        public IEnumerable<int> EmptyItems { get; set; }
        public int SingleItem { get; set; }
        public IEnumerable<int> EnumerableItems { get; set; }
    }
}
