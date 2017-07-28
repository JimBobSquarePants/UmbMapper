// <copyright file="PluginManagerInvocations.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Umbraco.Core;

namespace UmbMapper.Invocations
{
    /// <summary>
    /// Provides ways to return methods from <see cref="PluginManager"/> without knowing the type at runtime.
    /// Once a method is invoked for a given type then it is cached so that subsequent calls do not require
    /// any overhead compilation costs.
    /// </summary>
    internal class PluginManagerInvocations : CachedInvocations
    {
        /// <summary>
        /// The base resolve method
        /// </summary>
        private static readonly MethodInfo ResolveMethod = typeof(PluginManager).GetMethod(nameof(PluginManager.ResolveTypes));

        /// <summary>
        /// Generic method to find the specified type and cache the result
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="cacheResult">Whether to cache the result</param>
        /// <param name="specificAssemblies">Specific assemblies to search within</param>
        /// <returns>The <see cref="IEnumerable{Type}"/></returns>
        public static IEnumerable<Type> ResolveTypes(Type type, bool cacheResult = true, IEnumerable<Assembly> specificAssemblies = null)
        {
            MethodBaseCacheItem key = GetMethodCacheKey(type);

            Delegate f;
            if (!DelegateCache.TryGetValue(key, out f))
            {
                f = CreateGenericResolveMethod<bool, IEnumerable<Assembly>>(ResolveMethod.MakeGenericMethod(type));
                DelegateCache[key] = f;
            }

            return (IEnumerable<Type>)((Func<bool, IEnumerable<Assembly>, object>)f)(cacheResult, specificAssemblies);
        }

        /// <summary>
        /// Provides a generic way of generating the resolve methods for each generic method
        /// </summary>
        /// <param name="method">
        /// The <see cref="MethodInfo"/> to generate.
        /// </param>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.This type parameter is contravariant. That is, you can use either the type you specified or any type that is less derived. For more information about covariance and contravariance, see Covariance and Contravariance in Generics.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <returns>
        /// <returns>The return value of the method that this delegate encapsulates.</returns>
        /// </returns>
        private static Func<T1, T2, object> CreateGenericResolveMethod<T1, T2>(MethodInfo method)
        {
            ParameterInfo[] parameter = method.GetParameters();
            ParameterExpression cacheResult = Expression.Parameter(typeof(bool), "cacheResult");
            ParameterExpression specificAssemblies = Expression.Parameter(typeof(IEnumerable<Assembly>), "specificAssemblies");

            MethodCallExpression methodCall = Expression.Call(
                Expression.Constant(PluginManager.Current),
                method,
                Expression.Convert(cacheResult, parameter[0].ParameterType),
                Expression.Convert(specificAssemblies, parameter[1].ParameterType));

            return Expression.Lambda<Func<T1, T2, object>>(
                Expression.Convert(methodCall, typeof(object)), // TODO: Clean this up, we shouldn't need to cast
                cacheResult,
                specificAssemblies).Compile();
        }
    }
}