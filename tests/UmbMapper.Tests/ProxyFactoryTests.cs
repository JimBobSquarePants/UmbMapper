using System;
using System.Collections.Generic;
using System.Reflection;
using UmbMapper.Proxy;
using Xunit;

namespace UmbMapper.Tests
{
    public class ProxyFactoryTests
    {
        [Fact]
        public void ProxyCanGetSetProperties()
        {
            var factory = new ProxyFactory();
            IProxy proxy = factory.CreateProxy(typeof(TestClass), new List<string> { "id", "name" });
            proxy.Interceptor = new LazyInterceptor(new Dictionary<string, Lazy<object>>());

            // This is the method we are replicating within the property emitter.
            var tc = new TestClass();

            var idMethod =
                MethodBase.GetMethodFromHandle(typeof(TestClass).GetMethod("set_Id", new[] { typeof(int) }).MethodHandle);

            var nameMethod =
                MethodBase.GetMethodFromHandle(typeof(TestClass).GetMethod("set_Name", new[] { typeof(string) }).MethodHandle);

            idMethod.Invoke(tc, new object[] { 1 });
            nameMethod.Invoke(tc, new object[] { "Foo" });

            Assert.Equal(1, tc.Id);
            Assert.Equal("Foo", tc.Name);

            // ReSharper disable once SuspiciousTypeConversion.Global
            var testClass = (TestClass)proxy;

            testClass.Id = 1;
            testClass.Name = "Foo";

            Assert.Equal(1, testClass.Id);
            Assert.Equal("Foo", testClass.Name);
        }
    }

    public class TestClass
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
    }
}
