// <copyright file="LazyInterceptor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;

namespace UmbMapper.Proxy
{
    /// <summary>
    /// Intercepts virtual properties in classes replacing them with lazily implemented versions.
    /// </summary>
    internal class LazyInterceptor : IInterceptor
    {
        /// <summary>
        /// The lazy dictionary.
        /// </summary>
        private readonly Dictionary<string, Lazy<object>> lazyDictionary = new Dictionary<string, Lazy<object>>();
        private readonly FastPropertyAccessor propertyAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyInterceptor"/> class.
        /// </summary>
        /// <param name="propertyAccessor">The property accessor</param>
        /// <param name="values">
        /// The dictionary of values containing the property name to replace and the value to replace it with.
        /// </param>
        public LazyInterceptor(FastPropertyAccessor propertyAccessor, Dictionary<string, Lazy<object>> values)
        {
            this.propertyAccessor = propertyAccessor;
            foreach (KeyValuePair<string, Lazy<object>> pair in values)
            {
                this.lazyDictionary.Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Intercepts the <see cref="MethodBase"/> in the proxy to return a replaced value.
        /// </summary>
        /// <param name="methodBase">
        /// The <see cref="MethodBase"/> containing information about the current
        /// invoked property.
        /// </param>
        /// <param name="proxy">The proxied instance.</param>
        /// <returns>
        /// The <see cref="object"/> replacing the original implementation value.
        /// </returns>
        public object Intercept(MethodBase methodBase, IProxy proxy)
        {
            string name = methodBase.Name;
            string key = name.Substring(4);

            // Attempt to get the value from the lazy members.
            if (this.lazyDictionary.ContainsKey(key))
            {
                object value = this.lazyDictionary[key].Value;
                this.propertyAccessor.SetValue(key, proxy, value);
                this.lazyDictionary.Remove(key);
                return value;
            }

            return null;
        }
    }
}