// <copyright file="CompositeTypeTypeKey.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;

namespace UmbMapper.Extensions
{
    /// <summary>
    /// A lightweight struct for storing a pair of types for caching.
    /// </summary>
    internal readonly struct CompositeTypeTypeKey : IEquatable<CompositeTypeTypeKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeTypeTypeKey"/> struct.
        /// </summary>
        /// <param name="type1">The first type</param>
        /// <param name="type2">The second type</param>
        public CompositeTypeTypeKey(Type type1, Type type2)
        {
            this.Type1 = type1;
            this.Type2 = type2;
        }

        /// <summary>
        /// Gets the first type
        /// </summary>
        public Type Type1 { get; }

        /// <summary>
        /// Gets the second type
        /// </summary>
        public Type Type2 { get; }

        /// <inheritdoc/>
        public bool Equals(CompositeTypeTypeKey other)
        {
            return this.Type1 == other.Type1 && this.Type2 == other.Type2;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is CompositeTypeTypeKey compositeTypeTypeKey && this.Equals(compositeTypeTypeKey);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Type1.GetHashCode();
                return (hashCode * 397) ^ this.Type2.GetHashCode();
            }
        }
    }
}