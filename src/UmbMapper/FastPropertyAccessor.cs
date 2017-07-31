// <copyright file="FastPropertyAccessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace UmbMapper
{
    /// <summary>
    /// Provides a way to set a properties value using a combination of dynamic methods and IL generation.
    /// This is much faster, 5.6x, than <see cref="M:PropertyInfo.SetValue"/> as it avoids the normal overheads of reflection.
    /// Once a method is invoked for a given type then it is cached so that subsequent calls do not require
    /// any overhead compilation costs.
    /// </summary>
    internal class FastPropertyAccessor
    {
        /// <summary>
        /// The method cache for storing function implementations.
        /// </summary>
        private readonly Dictionary<string, Func<object, object>> getterCache
            = new Dictionary<string, Func<object, object>>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// The method cache for storing action implementations.
        /// </summary>
        private readonly Dictionary<string, Action<object, object>> setterCache
            = new Dictionary<string, Action<object, object>>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="FastPropertyAccessor"/> class.
        /// </summary>
        /// <param name="type">The type to generate the accessors for</param>
        public FastPropertyAccessor(Type type)
        {
            IEnumerable<PropertyInfo> properties = type.GetProperties(UmbMapperConstants.MappableFlags);

            foreach (PropertyInfo property in properties)
            {
                string name = property.Name;
                Type propertyType = property.PropertyType;

                this.getterCache[name] = MakeGetMethod(property.GetGetMethod(), propertyType);

                if (property.CanWrite && property.GetSetMethod() != null)
                {
                    this.setterCache[name] = MakeSetMethod(property.GetSetMethod(), propertyType);
                }
            }
        }

        /// <summary>
        /// Gets the value of the property on the given instance or <code>null</code>.
        /// </summary>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="instance">The current instance to return the property from.</param>
        /// <returns>The <see cref="object"/> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetValue(string propertyName, object instance)
        {
            return this.getterCache.ContainsKey(propertyName) ? this.getterCache[propertyName](instance) : null;
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
            if (this.setterCache.ContainsKey(propertyName))
            {
                this.setterCache[propertyName](instance, value);
            }
        }

        /// <summary>
        /// Builds the get accessor for the given type.
        /// </summary>
        /// <param name="method">The method to compile.</param>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The <see cref="Action{Object, Object}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Func<object, object> MakeGetMethod(MethodInfo method, Type propertyType)
        {
            Type type = method.DeclaringType;
            var dmethod = new DynamicMethod("Getter", typeof(object), new[] { typeof(object) }, type, true);
            ILGenerator il = dmethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // Load our value to the stack.
            il.Emit(OpCodes.Castclass, type); // Cast to the value type.

            // Call the set method.
            il.Emit(OpCodes.Callvirt, method);

            if (propertyType.IsValueType)
            {
                // Cast if a value type to set the correct type.
                il.Emit(OpCodes.Box, propertyType);
            }

            // Return the result.
            il.Emit(OpCodes.Ret);

            return (Func<object, object>)dmethod.CreateDelegate(typeof(Func<object, object>));
        }

        /// <summary>
        /// Builds the set accessor for the given type.
        /// </summary>
        /// <param name="method">The method to compile.</param>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The <see cref="Action{Object, Object}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Action<object, object> MakeSetMethod(MethodInfo method, Type propertyType)
        {
            Type type = method.DeclaringType;
            var dmethod = new DynamicMethod("Setter", typeof(void), new[] { typeof(object), typeof(object) }, type, true);
            ILGenerator il = dmethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // Load our instance to the stack.
            il.Emit(OpCodes.Castclass, type); // Cast to the instance type.
            il.Emit(OpCodes.Ldarg_1); // Load our value to the stack.

            if (propertyType.IsValueType)
            {
                // Cast if a value type to set the correct type.
                il.Emit(OpCodes.Unbox_Any, propertyType);
            }

            // Call the set method and return.
            il.Emit(OpCodes.Callvirt, method);
            il.Emit(OpCodes.Ret);

            return (Action<object, object>)dmethod.CreateDelegate(typeof(Action<object, object>));
        }
    }
}