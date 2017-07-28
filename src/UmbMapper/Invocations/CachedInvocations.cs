// <copyright file="CachedInvocations.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace UmbMapper.Invocations
{
    /// <summary>
    /// Internal methods for cached invocations.
    /// </summary>
    internal class CachedInvocations
    {
        /// <summary>
        /// The method cache for storing method delegates.
        /// </summary>
        protected static readonly ConcurrentDictionary<MethodBaseCacheItem, Delegate> DelegateCache
            = new ConcurrentDictionary<MethodBaseCacheItem, Delegate>();

        /// <summary>
        /// Returns a cache key for the given method and type.
        /// </summary>
        /// <param name="type">The <see cref="object"/> the key reflects.</param>
        /// <param name="memberName">The method name. Generated at compile time.</param>
        /// <returns>
        /// The <see cref="MethodBaseCacheItem"/> for the given method and type.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static MethodBaseCacheItem GetMethodCacheKey(object type, [CallerMemberName] string memberName = null)
        {
            return new MethodBaseCacheItem(memberName, type);
        }
    }
}
