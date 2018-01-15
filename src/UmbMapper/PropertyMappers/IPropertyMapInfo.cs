// <copyright file="IPropertyMapInfo.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Defines the contract for classes containing the propererties required for mapping an Umbraco property
    /// </summary>
    public interface IPropertyMapInfo
    {
        /// <summary>
        /// Gets the property
        /// </summary>
        PropertyInfo Property { get; }

        /// <summary>
        /// Gets the array of potential contructor parameters
        /// </summary>
        ParameterInfo[] ConstructorParams { get; }

        /// <summary>
        /// Gets the property type
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// Gets the generic parameter type for the type if it is an enumerable with a single parameter; otherwise, null.
        /// </summary>
        Type EnumerableParamType { get; }

        /// <summary>
        /// Gets a value indicating whether the specified type is an enumerable type.
        /// </summary>
        bool IsEnumerableType { get; }

        /// <summary>
        /// Gets a value indicating whether the specified type is an enumerable type that is safe to convert
        /// from <see cref="IEnumerable{T}"/> to a single item following processing via a mapper.
        /// </summary>
        bool IsConvertableEnumerableType { get; }

        /// <summary>
        /// Gets a value indicating whether the specified type is an enumerable type that is safe to cast
        /// following processing via a type converter
        /// </summary>
        bool IsCastableEnumerableType { get; }

        /// <summary>
        /// Gets a value indicating whether the specified type is an enumerable type containing a
        /// key value pair as the generic type parameter.
        /// </summary>
        bool IsEnumerableOfKeyValueType { get; }

        /// <summary>
        /// Gets the property aliases
        /// </summary>
        string[] Aliases { get; }

        /// <summary>
        /// Gets a value indicating whether to map the property recursively up the tree
        /// </summary>
        bool Recursive { get; }

        /// <summary>
        /// Gets a value indicating whether to lazily map the property
        /// </summary>
        bool Lazy { get; }

        /// <summary>
        /// Gets a value indicating whether the property is evaluated via a predicate
        /// </summary>
        bool HasPredicate { get; }

        /// <summary>
        /// Gets the default value
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// Gets the culture
        /// </summary>
        CultureInfo Culture { get; }
    }
}