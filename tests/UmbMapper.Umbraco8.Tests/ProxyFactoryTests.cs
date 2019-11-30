using System;
using System.Collections.Generic;
using System.Reflection;
using UmbMapper.Extensions;
using UmbMapper.Proxy;
using Xunit;

namespace UmbMapper.Umbraco8.Tests
{
    public class ProxyFactoryTests
    {
        [Fact]
        public void ProxyCanGetSetProperties()
        {
            Type proxyType = ProxyTypeFactory.CreateProxyType(typeof(TestClass), new List<string> { "id", "name" });
            var proxy = (IProxy)proxyType.GetInstance();

            proxy.Interceptor = new LazyInterceptor(new Dictionary<string, Lazy<object>>());

            // This is the method we are replicating within the property emitter.
            var tc = new TestClass();

            var idMethod =
                MethodBase.GetMethodFromHandle(typeof(TestClass).GetMethod("set_Id", new[] { typeof(int) }).MethodHandle);

            var nameMethod =
                MethodBase.GetMethodFromHandle(typeof(TestClass).GetMethod("set_Name", new[] { typeof(string) }).MethodHandle);

            var dateMethod =
                MethodBase.GetMethodFromHandle(typeof(TestClass).GetMethod("set_CreateDate", new[] { typeof(DateTime) }).MethodHandle);

            idMethod.Invoke(tc, new object[] { 1 });
            nameMethod.Invoke(tc, new object[] { "Foo" });
            dateMethod.Invoke(tc, new object[] { new DateTime(2017, 1, 1) });

            Assert.Equal(1, tc.Id);
            Assert.Equal("Foo", tc.Name);
            Assert.Equal(new DateTime(2017, 1, 1), tc.CreateDate);

            // ReSharper disable once SuspiciousTypeConversion.Global
            var testClass = (TestClass)proxy;

            testClass.Id = 1;
            testClass.Name = "Foo";
            testClass.CreateDate = new DateTime(2017, 1, 1);

            Assert.Equal(1, testClass.Id);
            Assert.Equal("Foo", testClass.Name);
            Assert.Equal(new DateTime(2017, 1, 1), testClass.CreateDate);
        }
    }

    public class TestClass
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
