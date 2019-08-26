using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmbMapper.Extensions;
using UmbMapper.Proxy;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper
{
    class MappingProcessor : IMappingProcessor
    {
        public object CreateEmpty(IUmbMapperConfig mappingConfig)
        {
            if (mappingConfig.CreateProxy)
            {
                var proxy = (IProxy)mappingConfig.ProxyType.GetInstance();
                proxy.Interceptor = new LazyInterceptor(new Dictionary<string, Lazy<object>>());
                return proxy;
            }

            return mappingConfig.MappedType.GetInstance();
        }

        public object CreateEmpty(IUmbMapperConfig mappingConfig, IPublishedContent content)
        {
            if (mappingConfig.CreateProxy)
            {
                var proxy = (IProxy)mappingConfig.ProxyType.GetInstance(content);
                proxy.Interceptor = new LazyInterceptor(new Dictionary<string, Lazy<object>>());
                return proxy;
            }

            return mappingConfig.MappedType.GetInstance(content);
        }

        public object Map(IUmbMapperConfig mappingConfig, IPublishedContent content)
        {
            throw new NotImplementedException();
        }

        public void Map(IUmbMapperConfig mappingConfig, IPublishedContent content, object destination)
        {
            throw new NotImplementedException();
        }
    }
}
