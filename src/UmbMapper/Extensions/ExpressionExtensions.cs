// <copyright file="ExpressionExtensions.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace UmbMapper.Extensions
{
    /// <summary>
    /// Extensions methods for the <see cref="Expression{TDelegate}"/> type.
    /// </summary>
    internal static class ExpressionExtensions
    {
        /// <summary>
        /// Resolves a property info from the given expression.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate that the <see cref="T:System.Linq.Expressions.Expression`1" /> represents.</typeparam>
        /// <param name="expression">The strongly typed lambda expression</param>
        /// <returns>The <see cref="PropertyInfo"/></returns>
        public static PropertyInfo ToPropertyInfo<TDelegate>(this Expression<TDelegate> expression)
        {
            // The property access might be getting converted to object to match the func
            // If so, get the operand and see if that's a member expression
            MemberExpression member = expression.Body as MemberExpression
                  ?? (expression.Body as UnaryExpression)?.Operand as MemberExpression;

            if (member == null)
            {
                throw new ArgumentException("Action must be a member expression.");
            }

            return member.Member as PropertyInfo;
        }
    }
}