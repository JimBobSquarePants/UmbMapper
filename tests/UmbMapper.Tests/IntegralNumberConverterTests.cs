using System;
using System.Globalization;
using UmbMapper.Converters;
using Xunit;

namespace UmbMapper.Tests
{
    public class IntegralNumberConverterTests
    {
        private const double Pi = 3.14159265358979;
        private static readonly string PiString = Pi.ToString(CultureInfo.InvariantCulture);
        private static readonly double RoundedPi = Math.Round(Pi, MidpointRounding.AwayFromZero);

        public static TheoryData<object, string> IntegralValues = new TheoryData<object, string> {
            { (sbyte)1, "1"},
            { (byte)1, "1" },
            { (short)1, "1"},
            { (ushort)1, "1" },
            { 1, "1" },
            { (uint)1, "1" },
            { (long)1, "1"},
            { (ulong)1, "1" }
        };

        public static TheoryData<object, string> RealValues = new TheoryData<object, string> {
            { (sbyte)RoundedPi, PiString},
            { (byte)RoundedPi, PiString},
            { (short)RoundedPi, PiString },
            { (ushort)RoundedPi, PiString },
            { (int)RoundedPi, PiString },
            { (uint)RoundedPi, PiString},
            { (long)RoundedPi, PiString },
            { (ulong)RoundedPi, PiString }
        };

        [Theory]
        [MemberData(nameof(IntegralValues))]
        [MemberData(nameof(RealValues))]
        public void CanConvertFromRealNumberString<T>(T expected, string value)
        {
            var actual = (T)IntegralNumberConverter<T>.ConvertFrom(CultureInfo.InvariantCulture, value);
            Assert.Equal(expected, actual);
        }
    }
}