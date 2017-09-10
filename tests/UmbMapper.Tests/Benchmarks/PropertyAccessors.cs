using System.ComponentModel;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace UmbMapper.Tests.Benchmarks
{
    public class PropertyAccessors
    {
        private PropertyAccessors obj;
        private dynamic dlr;
        private PropertyInfo prop;
        private PropertyDescriptor descriptor;
        private static readonly FastPropertyAccessor Accessor = new FastPropertyAccessor(typeof(PropertyAccessors));
        private static readonly FastPropertyAccessorExpressions AccessorExpressions = new FastPropertyAccessorExpressions(typeof(PropertyAccessors));

        public string Value { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            this.obj = new PropertyAccessors();
            this.dlr = this.obj;
            this.prop = typeof(PropertyAccessors).GetProperty("Value");
            this.descriptor = TypeDescriptor.GetProperties(this.obj)["Value"];
        }

        [Benchmark(Description = "Compile-time Standard C#", Baseline = true)]
        public string StaticCSharp()
        {
            this.obj.Value = "abc";
            return this.obj.Value;
        }

        [Benchmark(Description = "Compile-time Dynamic C#")]
        public string DynamicCSharp()
        {
            this.dlr.Value = "abc";
            return this.dlr.Value;
        }

        [Benchmark(Description = "PropertyInfo")]
        public string PropertyInfo()
        {
            this.prop.SetValue(this.obj, "abc", null);
            return (string)this.prop.GetValue(this.obj, null);
        }

        [Benchmark(Description = "PropertyDescriptor")]
        public string PropertyDescriptor()
        {
            this.descriptor.SetValue(this.obj, "abc");
            return (string)this.descriptor.GetValue(this.obj);
        }

        [Benchmark(Description = "FastPropertyAccessor IL")]
        public string FastProperties()
        {
            Accessor.SetValue("Value", this.obj, "abc");
            return (string)Accessor.GetValue("VALUE", this.obj);
        }

        [Benchmark(Description = "FastPropertyAccessor Expression Trees")]
        public string TypeWrapperProperties()
        {
            AccessorExpressions.SetValue("Value", this.obj, "abc");
            return (string)AccessorExpressions.GetValue("VALUE", this.obj);
        }
    }
}