// <copyright file="IProxy.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace UmbMapper.Proxy
{
    /// <summary>
    /// The IProxy interface prides necessary properties to proxy base classes.
    /// </summary>
    /// <remarks>This needs to be public in order for a proxy to implement it.</remarks>
    public interface IProxy
    {
        /// <summary>
        /// Gets or sets the <see cref="IInterceptor"/> for intercepting <see cref="System.Reflection.MethodBase"/> calls.
        /// </summary>
        IInterceptor Interceptor { get; set; }
    }
}