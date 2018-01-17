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
            { (sbyte)1, "1", typeof(sbyte) },
            { (byte)1, "1", typeof(byte) },
            { (short)1, "1",typeof(short) },
            { (ushort)1, "1",typeof(ushort) },
            { (int)1, "1" ,typeof(int)},
            { (uint)1, "1",typeof(uint) },
            { (long)1, "1",typeof(long) },
            { (ulong)1, "1",typeof(ulong) }
        };

        public static TheoryData<object, string, Type> RealValues = new TheoryData<object, string, Type> {
            { (sbyte)RoundedPi, PiString , typeof(sbyte)},
            { (byte)RoundedPi, PiString, typeof(byte)},
            { (short)RoundedPi, PiString , typeof(short)},
            { (ushort)RoundedPi, PiString , typeof(ushort)},
            { (int)RoundedPi, PiString , typeof(int)},
            { (uint)RoundedPi, PiString , typeof(uint)},
            { (long)RoundedPi, PiString , typeof(long)},
            { (ulong)RoundedPi, PiString, typeof(ulong) }
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