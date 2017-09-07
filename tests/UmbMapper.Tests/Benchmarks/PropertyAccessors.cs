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
        private FastPropertyAccessor accessor;

        public string Value { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            this.obj = new PropertyAccessors();
            this.dlr = this.obj;
            this.prop = typeof(PropertyAccessors).GetProperty("Value");
            this.descriptor = TypeDescriptor.GetProperties(this.obj)["Value"];
            this.accessor = new FastPropertyAccessor(typeof(PropertyAccessors));
        }

        [Benchmark(Description = "Static C#", Baseline = true)]
        public string StaticCSharp()
        {
            this.obj.Value = "abc";
            return this.obj.Value;
        }

        [Benchmark(Description = "Dynamic C#")]
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

        [Benchmark(Description = "FastPropertyAccessor")]
        public string FastProperties()
        {
            this.accessor.SetValue(this.prop.Name, this.obj, "abc");
            return (string)this.accessor.GetValue(this.prop.Name, this.obj);
        }
    }
}