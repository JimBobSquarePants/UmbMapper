﻿// <copyright file="EnumerableInvocations.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace UmbMapper.Invocations
{
    /// <summary>
    /// Provides ways to return methods from <see cref="Enumerable"/> without knowing the type at runtime.
    /// Once a method is invoked for a given type then it is cached so that subsequent calls do not require
    /// any overhead compilation costs.
    /// </summary>
    internal class EnumerableInvocations : CachedInvocations
    {
        /// <summary>
        /// The cast method.
        /// </summary>
        private static readonly MethodInfo CastMethod =
            ((Func<IEnumerable, IEnumerable<object>>)Enumerable.Cast<object>)
            .Method
            .GetGenericMethodDefinition();

        /// <summary>
        /// The empty method.
        /// </summary>
        private static readonly MethodInfo EmptyMethod =
            ((Func<IEnumerable<object>>)Enumerable.Empty<object>)
            .Method
            .GetGenericMethodDefinition();

        /// <summary>
        /// Casts the elements of the given <see cref="IEnumerable"/> to the specified type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to cast to.</param>
        /// <param name="value">
        /// The <see cref="IEnumerable"/> to cast the items of.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> representing the cast enumerable.
        /// </returns>
        public static object Cast(Type type, IEnumerable value)
        {
            MethodBaseCacheItem key = GetMethodCacheKey(type);

            if (!DelegateCache.TryGetValue(key, out Delegate f))
            {
                f = StaticMethodSingleParameter<object>(CastMethod.MakeGenericMethod(type));
                DelegateCache[key] = f;
            }

            return ((Func<object, object>)f)(value);
        }

        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T}"/> that has the specified type argument.
        /// </summary>
        /// <param name="type">
        /// The <see cref="Type"/> to assign to the type parameter of the returned
        /// generic <see cref="IEnumerable{T}"/>.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> representing the empty enumerable.
        /// </returns>
        public static object Empty(Type type)
        {
            MethodBaseCacheItem key = GetMethodCacheKey(type);

            if (!DelegateCache.TryGetValue(key, out Delegate f))
            {
                f = StaticMethod<object>(EmptyMethod.MakeGenericMethod(type));
                DelegateCache[key] = f;
            }

            return ((Func<object, object>)f)(type);
        }

        /// <summary>
        /// Provides a generic way of generating a static method taking a no parameters.
        /// </summary>
        /// <param name="method">
        /// The <see cref="MethodInfo"/> to generate.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the generic argument.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Func{T,Object}"/>.
        /// </returns>
        private static Func<T, object> StaticMethod<T>(MethodInfo method)
        {
            ParameterExpression argument = Expression.Parameter(typeof(object), "argument");
            MethodCallExpression methodCall = Expression.Call(null, method);

            return Expression.Lambda<Func<T, object>>(
                Expression.Convert(methodCall, typeof(object)),
                argument).Compile();
        }

        /// <summary>
        /// Provides a generic way of generating a static method that takes a single parameter.
        /// </summary>
        /// <param name="method">
        /// The <see cref="MethodInfo"/> to generate.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the generic argument.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Func{T,Object}"/>.
        /// </returns>
        private static Func<T, object> StaticMethodSingleParameter<T>(MethodInfo method)
        {
            ParameterInfo parameter = method.GetParameters().Single();
            ParameterExpression argument = Expression.Parameter(typeof(object), "argument");

            MethodCallExpression methodCall = Expression.Call(
                null,
                method,
                Expression.Convert(argument, parameter.ParameterType));

            return Expression.Lambda<Func<T, object>>(
                Expression.Convert(methodCall, typeof(object)),
                argument).Compile();
        }
    }
}