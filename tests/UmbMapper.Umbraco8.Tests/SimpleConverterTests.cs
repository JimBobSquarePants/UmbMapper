using System.Globalization;
using UmbMapper.Converters;
using Xunit;

namespace UmbMapper.Umbraco8.Tests
{
    public class SimpleConverterTests
    {
        private const double Pi = 3.14159265358979;
        private static readonly string PiString = Pi.ToString(CultureInfo.InvariantCulture);

        public static TheoryData<object, string> IntegralValues = new TheoryData<object, string> {
            { (float)1, "1" },
            { (double)1, "1" },
            { (decimal)1, "1" }
        };

        public static TheoryData<object, string> RealValues = new TheoryData<object, string> {
            { (float)Pi, PiString },
            { Pi, PiString},
            { (decimal)Pi, PiString },
        };

        [Theory]
        [MemberData(nameof(IntegralValues))]
        [MemberData(nameof(RealValues))]
        public void CanConvertFromRealNumberString<T>(T expected, string value)
        {
            var actual = (T)SimpleConverter<T>.ConvertFrom(CultureInfo.InvariantCulture, value);
            Assert.Equal(expected, actual);
        }
    }
}