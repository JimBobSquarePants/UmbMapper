// <copyright file="AssemblyExtensions.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.IO;
using System.Reflection;

namespace UmbMapper.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Assembly"/> class
    /// </summary>
    internal static class AssemblyExtensions
    {
        /// <summary>
        /// Private var determining if we have a loadable app code assembly. We only want to do this once
        /// per app recyle so we make it static and lazy so that it only populates when used.
        /// </summary>
        private static readonly Lazy<bool> HasLoadableAppCodeAssembly = new Lazy<bool>(() =>
        {
            try
            {
                Assembly.Load("App_Code");
                return true;
            }
            catch (FileNotFoundException)
            {
                // This will occur if it cannot load the assembly
                return false;
            }
        });

        /// <summary>
        /// Returns true if the assembly is the App_Code assembly
        /// </summary>
        /// <param name="assembly">The assembly to check</param>
        /// <returns>The <see cref="bool"/></returns>
        /// <remarks>Taken from <see cref="Umbraco.Core.AssemblyExtensions"/></remarks>
        public static bool IsAppCodeAssembly(this Assembly assembly)
        {
            return assembly.FullName.StartsWith("App_Code") && HasLoadableAppCodeAssembly.Value;
        }
    }
}