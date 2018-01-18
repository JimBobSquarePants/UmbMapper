// <copyright file="ObjectExtensions.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using UmbMapper.Converters;
using Umbraco.Core;

namespace UmbMapper.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="object"/>.
    /// </summary>
    internal static class ObjectExtensions
    {
        // Used for caching the various type lookups
        private static readonly Dictionary<Type, Type> NullableGenericCache = new Dictionary<Type, Type>();
        private static readonly Dictionary<TypePair, TypeConverter> InputTypeConverterCache = new Dictionary<TypePair, TypeConverter>();
        private static readonly Dictionary<TypePair, TypeConverter> DestinationTypeConverterCache = new Dictionary<TypePair, TypeConverter>();
        private static readonly Dictionary<TypePair, IConvertible> AssignableTypeCache = new Dictionary<TypePair, IConvertible>();
        private static readonly Dictionary<Type, bool> BoolConvertCache = new Dictionary<Type, bool>();

        private static readonly char[] NumberDecimalSeparatorsToNormalize = { '.', ',' };
        private static readonly CustomBooleanTypeConverter CustomBooleanTypeConverter = new CustomBooleanTypeConverter();

        /// <summary>
        /// Gets a value indicating whether the <see cref="object"/> is null or an empty <see cref="string"/>.
        /// </summary>
        /// <param name="value">The object to test against.</param>
        /// <returns>True; if the value is null or an empty string; otherwise; false.</returns>
        public static bool IsNullOrEmptyString(this object value)
        {
            return value == null || value as string == string.Empty;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="object"/> is null or an empty <see cref="string"/>.
        /// </summary>
        /// <param name="value">The object to test against.</param>
        /// <returns>True; if the value is null or an empty string; otherwise; false.</returns>
        public static bool IsNullOrWhiteSpaceString(this object value)
        {
            return string.IsNullOrWhiteSpace(value as string);
        }

        /// <summary>
        /// Attempts to convert the input object to the output type.
        /// </summary>
        /// <remarks>This code is an optimized version of the original Umbraco method</remarks>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="input">The input.</param>
        /// <returns>The <see cref="Attempt{T}"/></returns>
        public static Attempt<T> UmbMapperTryConvertTo<T>(this object input)
        {
            Attempt<object> result = UmbMapperTryConvertTo(input, typeof(T));
            if (!result.Success)
            {
                // Just try a straight up conversion
                try
                {
                    var converted = (T)input;
                    return Attempt<T>.Succeed(converted);
                }
                catch (Exception e)
                {
                    return Attempt<T>.Fail(e);
                }
            }

            return Attempt<T>.Succeed((T)result.Result);
        }

        /// <summary>
        /// Attempts to convert the input object to the output type.
        /// </summary>
        /// <remarks>This code is an optimized version of the original Umbraco method</remarks>
        /// <param name="input">The input.</param>
        /// <param name="destinationType">The type to convert to</param>
        /// <returns>The <see cref="Attempt{Object}"/></returns>
        public static Attempt<object> UmbMapperTryConvertTo(this object input, Type destinationType)
        {
            try
            {
                if (destinationType == null)
                {
                    return Attempt<object>.Fail();
                }

                if (input == null)
                {
                    // Nullable is ok
                    if (destinationType.IsGenericType && GetCachedGenericNullableType(destinationType) != null)
                    {
                        return Attempt<object>.Succeed(null);
                    }

                    // Reference types are ok
                    return Attempt<object>.SucceedIf(!destinationType.IsValueType, null);
                }

                Type inputType = input.GetType();

                // Easy
                if (destinationType == typeof(object) || inputType == destinationType)
                {
                    return Attempt.Succeed(input);
                }

                // Check for string so that overloaders of ToString() can take advantage of the conversion.
                if (destinationType == typeof(string))
                {
                    return Attempt<object>.Succeed(input.ToString());
                }

                // If we've got a nullable of something, we try to convert directly to that thing.
                // We cache the destination type and underlying nullable types
                // Any other generic types need to fall through
                if (destinationType.IsGenericType)
                {
                    Type underlyingGenericType = GetCachedGenericNullableType(destinationType);
                    if (underlyingGenericType != null)
                    {
                        // Special case for empty strings for bools/dates which should return null if an empty string.
                        if (input is string asString)
                        {
                            if (string.IsNullOrEmpty(asString) && (underlyingGenericType == typeof(DateTime) || underlyingGenericType == typeof(bool)))
                            {
                                return Attempt<object>.Succeed(null);
                            }
                        }

                        // Recursively call into this method with the inner (not-nullable) type and handle the outcome
                        Attempt<object> nonNullable = input.UmbMapperTryConvertTo(underlyingGenericType);

                        // And if sucessful, fall on through to rewrap in a nullable; if failed, pass on the exception
                        if (nonNullable.Success)
                        {
                            input = nonNullable.Result; // Now fall on through...
                        }
                        else
                        {
                            return Attempt<object>.Fail(nonNullable.Exception);
                        }
                    }
                }
                else
                {
                    if (input is string asString)
                    {
                        // Try convert from string, returns an Attempt if the string could be
                        // processed (either succeeded or failed), else null if we need to try
                        // other methods
                        Attempt<object>? result = TryConvertToFromString(asString, destinationType);
                        if (result.HasValue)
                        {
                            return result.Value;
                        }
                    }

                    // TODO: Do a check for destination type being IEnumerable<T> and source type implementing IEnumerable<T> with
                    // the same 'T', then we'd have to find the extension method for the type AsEnumerable() and execute it.
                    IConvertible convertible = GetCachedAssignableConvertibleResult(input, inputType, destinationType);
                    if (convertible != null)
                    {
                        return Attempt.Succeed(Convert.ChangeType(convertible, destinationType));
                    }
                }

                if (destinationType == typeof(bool))
                {
                    if (GetCanConvertToBooleanResult(inputType))
                    {
                        return Attempt.Succeed(CustomBooleanTypeConverter.ConvertFrom(input));
                    }
                }

                TypeConverter inputConverter = GetCachedInputTypeConverter(inputType, destinationType);
                if (inputConverter != null)
                {
                    return Attempt.Succeed(inputConverter.ConvertTo(input, destinationType));
                }

                TypeConverter outputConverter = GetCachedDestinationTypeConverter(inputType, destinationType);
                if (outputConverter != null)
                {
                    return Attempt.Succeed(outputConverter.ConvertFrom(input));
                }

                // Re-check convertables since we altered the input through recursion
                if (input is IConvertible convertible2)
                {
                    return Attempt.Succeed(Convert.ChangeType(convertible2, destinationType));
                }
            }
            catch (Exception e)
            {
                return Attempt<object>.Fail(e);
            }

            return Attempt<object>.Fail();
        }

        /// <summary>
        /// Attempts to convert the input string to the output type.
        /// </summary>
        /// <remarks>This code is an optimized version of the original Umbraco method</remarks>
        /// <param name="input">The input.</param>
        /// <param name="destinationType">The type to convert to</param>
        /// <returns>The <see cref="Nullable{Attempt}"/></returns>
        private static Attempt<object>? TryConvertToFromString(this string input, Type destinationType)
        {
            // Easy
            if (destinationType == typeof(string))
            {
                return Attempt<object>.Succeed(input);
            }

            // Null, empty, whitespaces
            if (string.IsNullOrWhiteSpace(input))
            {
                if (destinationType == typeof(bool))
                {
                    // null/empty = bool false
                    return Attempt<object>.Succeed(false);
                }

                if (destinationType == typeof(DateTime))
                {
                    // null/empty = min DateTime value
                    return Attempt<object>.Succeed(DateTime.MinValue);
                }

                // Cannot decide here,
                // Any of the types below will fail parsing and will return a failed attempt
                // but anything else will not be processed and will return null
                // so even though the string is null/empty we have to proceed.
            }

            // Look for type conversions in the expected order of frequency of use.
            //
            // By using a mixture of ordered if statements and switches we can optimize both for
            // fast conditional checking for most frequently used types and the branching
            // that does not depend on previous values available to switch statements.
            if (destinationType.IsPrimitive)
            {
                if (destinationType == typeof(int))
                {
                    if (int.TryParse(input, out int value))
                    {
                        return Attempt<object>.Succeed(value);
                    }

                    // Because decimal 100.01m will happily convert to integer 100, it
                    // makes sense that string "100.01" *also* converts to integer 100.
                    string input2 = NormalizeNumberDecimalSeparator(input);
                    return Attempt<object>.SucceedIf(decimal.TryParse(input2, out decimal value2), Convert.ToInt32(value2));
                }

                if (destinationType == typeof(long))
                {
                    if (long.TryParse(input, out long value))
                    {
                        return Attempt<object>.Succeed(value);
                    }

                    // Same as int
                    string input2 = NormalizeNumberDecimalSeparator(input);
                    return Attempt<object>.SucceedIf(decimal.TryParse(input2, out decimal value2), Convert.ToInt64(value2));
                }

                // TODO: Should we do the decimal trick for short, byte, unsigned?
                if (destinationType == typeof(bool))
                {
                    if (bool.TryParse(input, out bool value))
                    {
                        return Attempt<object>.Succeed(value);
                    }

                    // Don't declare failure so the CustomBooleanTypeConverter can try
                    return null;
                }

                // Calling this method directly is faster than any attempt to cache it.
                switch (Type.GetTypeCode(destinationType))
                {
                    case TypeCode.Int16:
                        return Attempt<object>.SucceedIf(short.TryParse(input, out short value), value);

                    case TypeCode.Double:
                        string input2 = NormalizeNumberDecimalSeparator(input);
                        return Attempt<object>.SucceedIf(double.TryParse(input2, out double valueD), valueD);

                    case TypeCode.Single:
                        string input3 = NormalizeNumberDecimalSeparator(input);
                        return Attempt<object>.SucceedIf(float.TryParse(input3, out float valueF), valueF);

                    case TypeCode.Char:
                        return Attempt<object>.SucceedIf(char.TryParse(input, out char valueC), valueC);

                    case TypeCode.Byte:
                        return Attempt<object>.SucceedIf(byte.TryParse(input, out byte valueB), valueB);

                    case TypeCode.SByte:
                        return Attempt<object>.SucceedIf(sbyte.TryParse(input, out sbyte valueSb), valueSb);

                    case TypeCode.UInt32:
                        return Attempt<object>.SucceedIf(uint.TryParse(input, out uint valueU), valueU);

                    case TypeCode.UInt16:
                        return Attempt<object>.SucceedIf(ushort.TryParse(input, out ushort valueUs), valueUs);

                    case TypeCode.UInt64:
                        return Attempt<object>.SucceedIf(ulong.TryParse(input, out ulong valueUl), valueUl);
                }
            }
            else if (destinationType == typeof(Guid))
            {
                return Attempt<object>.SucceedIf(Guid.TryParse(input, out Guid value), value);
            }
            else if (destinationType == typeof(DateTime))
            {
                if (DateTime.TryParse(input, out DateTime value))
                {
                    switch (value.Kind)
                    {
                        case DateTimeKind.Unspecified:
                        case DateTimeKind.Utc:
                            return Attempt<object>.Succeed(value);

                        case DateTimeKind.Local:
                            return Attempt<object>.Succeed(value.ToUniversalTime());

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return Attempt<object>.Fail();
            }
            else if (destinationType == typeof(DateTimeOffset))
            {
                return Attempt<object>.SucceedIf(DateTimeOffset.TryParse(input, out DateTimeOffset value), value);
            }
            else if (destinationType == typeof(TimeSpan))
            {
                return Attempt<object>.SucceedIf(TimeSpan.TryParse(input, out TimeSpan value), value);
            }
            else if (destinationType == typeof(decimal))
            {
                string input2 = NormalizeNumberDecimalSeparator(input);
                return Attempt<object>.SucceedIf(decimal.TryParse(input2, out decimal value), value);
            }
            else if (input != null && destinationType == typeof(Version))
            {
                return Attempt<object>.SucceedIf(Version.TryParse(input, out Version value), value);
            }

            // E_NOTIMPL IPAddress, BigInteger
            return null; // We can't decide...
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string NormalizeNumberDecimalSeparator(string s)
        {
            char normalized = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            return ReplaceMany(s, NumberDecimalSeparatorsToNormalize, normalized);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ReplaceMany(string text, char[] chars, char replacement)
        {
            // for... is faster
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < chars.Length; i++)
            {
                text = text.Replace(chars[i], replacement);
            }

            return text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TypeConverter GetCachedInputTypeConverter(Type inputType, Type destinationType)
        {
            var key = new TypePair(inputType, destinationType);
            if (!InputTypeConverterCache.ContainsKey(key))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(inputType);
                if (converter.CanConvertTo(destinationType))
                {
                    InputTypeConverterCache[key] = converter;
                    return converter;
                }
                else
                {
                    InputTypeConverterCache[key] = null;
                    return null;
                }
            }

            return InputTypeConverterCache[key];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TypeConverter GetCachedDestinationTypeConverter(Type inputType, Type destinationType)
        {
            var key = new TypePair(inputType, destinationType);
            if (!DestinationTypeConverterCache.ContainsKey(key))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
                if (converter.CanConvertFrom(inputType))
                {
                    DestinationTypeConverterCache[key] = converter;
                    return converter;
                }
                else
                {
                    DestinationTypeConverterCache[key] = null;
                    return null;
                }
            }

            return DestinationTypeConverterCache[key];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Type GetCachedGenericNullableType(Type destinationType)
        {
            if (!NullableGenericCache.ContainsKey(destinationType))
            {
                if (destinationType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type underlying = Nullable.GetUnderlyingType(destinationType);
                    NullableGenericCache[destinationType] = underlying;
                    return underlying;
                }
                else
                {
                    NullableGenericCache[destinationType] = null;
                    return null;
                }
            }

            return NullableGenericCache[destinationType];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IConvertible GetCachedAssignableConvertibleResult(object input, Type inputType, Type destinationType)
        {
            var key = new TypePair(inputType, destinationType);
            if (!AssignableTypeCache.ContainsKey(key))
            {
                if (destinationType.IsAssignableFrom(inputType))
                {
                    if (input is IConvertible convertable)
                    {
                        AssignableTypeCache[key] = convertable;
                        return convertable;
                    }
                }
                else
                {
                    AssignableTypeCache[key] = null;
                    return null;
                }
            }

            return AssignableTypeCache[key];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool GetCanConvertToBooleanResult(Type inputType)
        {
            if (!BoolConvertCache.ContainsKey(inputType))
            {
                bool canConvert = CustomBooleanTypeConverter.CanConvertFrom(inputType);
                BoolConvertCache[inputType] = canConvert;
                return canConvert;
            }

            return BoolConvertCache[inputType];
        }
    }
}