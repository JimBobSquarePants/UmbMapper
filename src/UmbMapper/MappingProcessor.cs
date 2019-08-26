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
            object result;
            if (mappingConfig.CreateProxy)
            {
                // Create a proxy instance to replace our object.
                result = mappingConfig.HasIPublishedConstructor ? mappingConfig.ProxyType.GetInstance(content) : mappingConfig.ProxyType.GetInstance();

                // Map the lazy properties and predicate mappings
                Dictionary<string, Lazy<object>> lazyProperties = this.MapLazyProperties(content, result);

                // Set the interceptor and replace our result with the proxy
                ((IProxy)result).Interceptor = new LazyInterceptor(lazyProperties);
            }
            else
            {
                result = mappingConfig.HasIPublishedConstructor ? mappingConfig.MappedType.GetInstance(content) : mappingConfig.MappedType.GetInstance();
            }

            // Users might want to use lazy loading with API controllers that do not inherit from UmbracoAPIController.
            // Certain mappers like Archetype require the context so we want to ensure it exists.
            //EnsureUmbracoContext();

            // Now map the non-lazy properties and non-lazy predicate mappings
            //this.MapNonLazyProperties(content, result);

            return result;
        }

        public void Map(IUmbMapperConfig mappingConfig, IPublishedContent content, object destination)
        {
            // Users might want to use lazy loading with API controllers that do not inherit from UmbracoAPIController.
            // Certain mappers like Archetype require the context so we want to ensure it exists.
            //TODO is this needed?
            //EnsureUmbracoContext();

            // We don't know whether the destination was created by UmbMapper or by something else so we have to check to see if it
            // is a proxy instance.
            if (destination is IProxy proxy)
            {
                // Map the lazy properties and predicate mappings
                Dictionary<string, Lazy<object>> lazyProperties = mappingConfig.MapLazyProperties(content, destination);

                // Replace the interceptor with our new one.
                var interceptor = new LazyInterceptor(lazyProperties);
                proxy.Interceptor = interceptor;
            }
            else
            {
                // Map our collated lazy properties as non-lazy instead.
                mappingConfig.MapLazyPropertiesAsNonLazy(content, destination);
            }

            // Map the non-lazy properties and non-lazy predicate mappings
            mappingConfig.MapNonLazyProperties(content, destination);
        }
    }
}
