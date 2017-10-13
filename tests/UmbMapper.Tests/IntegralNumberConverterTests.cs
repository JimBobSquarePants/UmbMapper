using System;
using System.Globalization;
using UmbMapper.Converters;
using Xunit;

namespace UmbMapper.Tests
{
    public class IntegralNumberConverterTests
    {
        const double Pi = 3.14159265358979;
        static readonly string PiString = Pi.ToString(CultureInfo.InvariantCulture);
        static readonly double RoundedPi = Math.Round(Pi, MidpointRounding.AwayFromZero);

        public static TheoryData<object, string, Type> IntegralValues = new TheoryData<object, string, Type> {
            { (sbyte)1, "1", TypeConstants.Sbyte },
            { (byte)1, "1", TypeConstants.Byte },
            { (short)1, "1",TypeConstants.Short },
            { (ushort)1, "1",TypeConstants.UShort },
            { (int)1, "1" ,TypeConstants.Int},
            { (uint)1, "1",TypeConstants.UInt },
            { (long)1, "1",TypeConstants.Long },
            { (ulong)1, "1",TypeConstants.ULong }
        };

        public static TheoryData<object, string, Type> RealValues = new TheoryData<object, string, Type> {
            { (sbyte)RoundedPi, PiString , TypeConstants.Sbyte},
            { (byte)RoundedPi, PiString, TypeConstants.Byte},
            { (short)RoundedPi, PiString , TypeConstants.Short},
            { (ushort)RoundedPi, PiString , TypeConstants.UShort},
            { (int)RoundedPi, PiString , TypeConstants.Int},
            { (uint)RoundedPi, PiString , TypeConstants.UInt},
            { (long)RoundedPi, PiString , TypeConstants.Long},
            { (ulong)RoundedPi, PiString, TypeConstants.ULong }
        };

        [Theory]
        [MemberData(nameof(IntegralValues))]
        [MemberData(nameof(RealValues))]
        public void CanConvertFromRealNumberString<T>(T expected, string value, Type type)
        {
            var actual = (T)IntegralNumberConverter<T>.ConvertFrom(CultureInfo.InvariantCulture, value, type);
            Assert.Equal(expected, actual);
        }
    }
}
