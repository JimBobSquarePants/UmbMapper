using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using UmbMapper.Invocations;
using UmbMapper.PropertyMappers;
using Umbraco.Core;

namespace UmbMapper.Benchmarks
{
    /// <summary>
    /// We use two different types to resolve both with the same inheritence tree. 
    /// This prevents the PluginManager cache skewing our results
    /// </summary>
    public class PluginManagerInvocation
    {
        [Benchmark(Description = "PluginManagerInvocations.ResolveTypes")]
        public IEnumerable<Type> PluginManagerInvoke()
        {
            return PluginManagerInvocations.ResolveTypes(typeof(EnumPropertyMapper));
        }

        [Benchmark(Baseline = true, Description = "MethodBase.Invoke")]
        public IEnumerable<Type> MethodInfoInvoke()
        {
            return InvokeResolver(typeof(UmbracoPropertyMapper));
        }

        private static IEnumerable<Type> InvokeResolver(Type baseType)
        {
            // There is no non generic version of ResolveTypes so we have to call it via reflection.
            MethodInfo method = typeof(PluginManager).GetMethod("ResolveTypes");
            MethodInfo generic = method.MakeGenericMethod(baseType);
            return ((IEnumerable<Type>)generic.Invoke(PluginManager.Current, new object[] { true, null })).ToArray();
        }
    }
}
