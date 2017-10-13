using System;
using System.Globalization;
using UmbMapper.Converters;
using Xunit;

namespace UmbMapper.Tests
{
    public class SimpleConverterTests
    {
        const double Pi = 3.14159265358979;
        static readonly string PiString = Pi.ToString(CultureInfo.InvariantCulture);

        public static TheoryData<object, string, Type> IntegralValues = new TheoryData<object, string, Type> {
            { (float)1, "1", TypeConstants.Float },
            { (double)1, "1", TypeConstants.Double },
            { (decimal)1, "1",TypeConstants.Decimal }
        };

        public static TheoryData<object, string, Type> RealValues = new TheoryData<object, string, Type> {
            { (float)Pi, PiString , TypeConstants.Float},
            { (double)Pi, PiString , TypeConstants.Double},
            { (decimal)Pi, PiString , TypeConstants.Decimal},
        };

        [Theory]
        [MemberData(nameof(IntegralValues))]
        [MemberData(nameof(RealValues))]
        public void CanConvertFromRealNumberString<T>(T expected, string value, Type type)
        {
            var actual = (T)SimpleConverter<T>.ConvertFrom(CultureInfo.InvariantCulture, value, type);
            Assert.Equal(expected, actual);
        }
    }
}
