// <copyright file="FastPropertyAccessorExpressions.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace UmbMapper
{
    /// <summary>
    /// Provides a way to get and set a property value using expression trees.
    /// This is much faster, 5x, than <see cref="M:PropertyInfo.SetValue"/> as it avoids the normal overheads of reflection.
    /// Once a method is invoked for a given type then it is cached so that subsequent calls do not require
    /// any overhead compilation costs.
    /// </summary>
    /// <remarks>The getter expects invariant uppercase for fast comparison.</remarks>
    public class FastPropertyAccessorExpressions
    {
        private readonly Dictionary<string, Action<object, object>> setters
            = new Dictionary<string, Action<object, object>>();

        private readonly Dictionary<string, Func<object, object>> getters
            = new Dictionary<string, Func<object, object>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FastPropertyAccessorExpressions"/> class.
        /// </summary>
        /// <param name="type">The type to generate the accessors for</param>
        public FastPropertyAccessorExpressions(Type type)
        {
            foreach (PropertyInfo property in type.GetProperties(UmbMapperConstants.MappableFlags))
            {
                string name = property.Name;

                ParameterExpression wrappedObjectParameter = Expression.Parameter(typeof(object));
                ParameterExpression valueParameter = Expression.Parameter(typeof(object));

                if (property.CanWrite && property.GetSetMethod() != null)
                {
                    var setExpression = Expression.Lambda<Action<object, object>>(
                        Expression.Assign(
                            Expression.Property(
                                Expression.Convert(wrappedObjectParameter, type), property),
                            Expression.Convert(valueParameter, property.PropertyType)),
                        wrappedObjectParameter,
                        valueParameter);

                    this.setters.Add(name, setExpression.Compile());
                }

                var getExpression = Expression.Lambda<Func<object, object>>(
                    Expression.Convert(
                        Expression.Property(
                            Expression.Convert(wrappedObjectParameter, type), property),
                        typeof(object)),
                    wrappedObjectParameter);

                this.getters.Add(name.ToUpperInvariant(), getExpression.Compile());
            }
        }

        /// <summary>
        /// Set the value of the property on the given instance.
        /// </summary>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="instance">The current instance to assign the property to.</param>
        /// <param name="value">The value to set.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(string propertyName, object instance, object value)
        {
            this.setters[propertyName](instance, value);
        }

        /// <summary>
        /// Gets the value of the property on the given instance or <code>null</code>.
        /// </summary>
        /// <param name="propertyName">The name of the property to get. Uppercase</param>
        /// <param name="instance">The current instance to return the property from.</param>
        /// <returns>The <see cref="object"/> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetValue(string propertyName, object instance)
        {
            return this.getters.TryGetValue(propertyName, out var getter)
                ? getter(instance)
                : null;
        }
    }
}