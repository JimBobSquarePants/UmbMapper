﻿// <copyright file="InterceptorEmitter.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace UmbMapper.Proxy
{
    /// <summary>
    /// Emits the <see cref="IProxy"/> <see cref="IInterceptor"/> property on the dynamic proxy class.
    /// </summary>
    internal static class InterceptorEmitter
    {
        /// <summary>
        /// Implements the proxy interceptor fields.
        /// </summary>
        /// <param name="typeBuilder">The <see cref="TypeBuilder"/> for the current proxy type.</param>
        public static void Emit(TypeBuilder typeBuilder)
        {
            // Implement the IProxy interface
            typeBuilder.AddInterfaceImplementation(typeof(IProxy));

            // Define the private "interceptor" filed.
            FieldBuilder fieldBuilder = typeBuilder.DefineField("interceptor", typeof(IInterceptor), FieldAttributes.Private);

            // Define the correct attributes fot the property. This makes it public virtual.
            const MethodAttributes attributes = MethodAttributes.Public
                                                | MethodAttributes.HideBySig
                                                | MethodAttributes.SpecialName
                                                | MethodAttributes.NewSlot
                                                | MethodAttributes.Virtual;

            // Implement the getter
            MethodBuilder getterMethod = typeBuilder.DefineMethod(
                "get_Interceptor",
                attributes,
                CallingConventions.HasThis,
                typeof(IInterceptor),
                new Type[0]);

            // Set the correct flags to signal the property is managed and implemented in intermediate language.
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            getterMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);

            ILGenerator il = getterMethod.GetILGenerator();

            // This is equivalent to:
            // get { return this.interceptor; }
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldBuilder);
            il.Emit(OpCodes.Ret);

            // Implement the setter
            MethodBuilder setterMethod = typeBuilder.DefineMethod(
                "set_Interceptor",
                attributes,
                CallingConventions.HasThis,
                typeof(void),
                new[] { typeof(IInterceptor) });

            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            setterMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);
            il = setterMethod.GetILGenerator();

            // This is equivalent to:
            // set { this.interceptor = value; }
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldBuilder);
            il.Emit(OpCodes.Ret);

            // Implement the properties on the IProxy interface
            MethodInfo originalSetter = typeof(IProxy).GetMethod("set_Interceptor");
            MethodInfo originalGetter = typeof(IProxy).GetMethod("get_Interceptor");

            // ReSharper disable AssignNullToNotNullAttribute
            typeBuilder.DefineMethodOverride(setterMethod, originalSetter);
            typeBuilder.DefineMethodOverride(getterMethod, originalGetter);
        }
    }
}