﻿// <copyright file="MethodBaseCacheItem.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace UmbMapper.Invocations
{
    /// <summary>
    /// A single method base cache item for identifying methods.
    /// </summary>
    internal readonly struct MethodBaseCacheItem : IEquatable<MethodBaseCacheItem>
    {
        /// <summary>
        /// Gets or sets the method base.
        /// </summary>
        public readonly string MethodBase;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public readonly object Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBaseCacheItem"/> struct.
        /// </summary>
        /// <param name="methodBase">The method base.</param>
        /// <param name="type">The object type or property.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MethodBaseCacheItem(string methodBase, object type)
        {
            this.MethodBase = methodBase;
            this.Type = type;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to. </param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the
        /// same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is MethodBaseCacheItem methodBaseCacheItem && this.Equals(methodBaseCacheItem);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if <paramref name="other"/> and this instance are the same type and represent the
        /// same value; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(MethodBaseCacheItem other)
        {
            return string.Equals(this.MethodBase, other.MethodBase) && Equals(this.Type, other.Type);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.MethodBase?.GetHashCode() ?? 0) * 397) ^ (this.Type?.GetHashCode() ?? 0);
            }
        }
    }
}