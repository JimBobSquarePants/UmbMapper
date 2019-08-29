using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UmbMapper.Extensions;
using UmbMapper.Invocations;
using UmbMapper.PropertyMappers;
using UmbMapper.Proxy;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper
{
    public class MappingProcessor<T> : IMappingProcessor
        where T : class
    {
        private readonly IUmbMapperConfig mappingConfig;
        private readonly IUmbMapperService umbMapperService;

        public MappingProcessor(IUmbMapperConfig mappingConfig, IUmbMapperService umbMapperService)
        {
            this.mappingConfig = mappingConfig;
            this.umbMapperService = umbMapperService;
        }

        public object CreateEmpty()
        {
            if (mappingConfig.CreateProxy)
            {
                var proxy = (IProxy)mappingConfig.ProxyType.GetInstance();
                proxy.Interceptor = new LazyInterceptor(new Dictionary<string, Lazy<object>>());
                return proxy;
            }

            return mappingConfig.MappedType.GetInstance();
        }

        public object CreateEmpty(IPublishedContent content)
        {
            if (mappingConfig.CreateProxy)
            {
                var proxy = (IProxy)mappingConfig.ProxyType.GetInstance(content);
                proxy.Interceptor = new LazyInterceptor(new Dictionary<string, Lazy<object>>());
                return proxy;
            }

            return mappingConfig.MappedType.GetInstance(content);
        }

        public object Map(IPublishedContent content)
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
            this.MapNonLazyProperties(content, result);

            return result;
        }

        public void Map(IPublishedContent content, object destination)
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
                Dictionary<string, Lazy<object>> lazyProperties = this.MapLazyProperties(content, destination);

                // Replace the interceptor with our new one.
                var interceptor = new LazyInterceptor(lazyProperties);
                proxy.Interceptor = interceptor;
            }
            else
            {
                // Map our collated lazy properties as non-lazy instead.
                this.MapLazyPropertiesAsNonLazy(content, destination);
            }

            // Map the non-lazy properties and non-lazy predicate mappings
            this.MapNonLazyProperties(content, destination);
        }

        //private static object RecursivelyMap(object value, PropertyMapInfo info)
        private static object RecursivelyMap(object value, PropertyMapInfo info, IUmbMapperService umbMapperService)
        {
            if (!info.PropertyType.IsInstanceOfType(value))
            {
                // If the property value is an IPublishedContent, then we can map it to the target type.
                if (value is IPublishedContent content && info.PropertyType.IsClass)
                {
                    //object returnObject = null;
                    //this.OnRecursivelyMapSingle?.Invoke(content, info.PropertyType, out returnObject);

                    //return returnObject;

                    return umbMapperService.MapTo(content, info.PropertyType);
                }

                // If the property value is an IEnumerable<IPublishedContent>, then we can map it to the target type.
                if (value.GetType().IsEnumerableOfType(typeof(IPublishedContent)) && info.IsEnumerableType)
                {
                    Type genericType = info.EnumerableParamType;
                    if (genericType?.IsClass == true)
                    {
                        //IEnumerable<object> returnObjects = null;
                        //this.umbMapperService.MapTo((IEnumerable<IPublishedContent>)value, genericType, returnObjects);

                        //return returnObjects;

                        return umbMapperService.MapTo((IEnumerable<IPublishedContent>)value, genericType);
                    }
                }
            }

            return value;
        }

        private Dictionary<string, Lazy<object>> MapLazyProperties(IPublishedContent content, object result)
        {
            // First add any lazy mappings, use count to prevent allocations
            var lazyProperties = new Dictionary<string, Lazy<object>>(this.mappingConfig.LazyNames.Count);
            for (int i = 0; i < this.mappingConfig.LazyMaps.Length; i++)
            {
                // It's better to allocate the `int` via closure than PropertyMap<T>
                int i1 = i;
                lazyProperties[this.mappingConfig.LazyMaps[i].Info.Property.Name] = new Lazy<object>(() =>
                {
                    //EnsureUmbracoContext();
                    return this.MapProperty(this.mappingConfig.LazyMaps[i1], content, result);
                });
            }

            // Then lazy predicate mappings
            for (int i = 0; i < this.mappingConfig.LazyPredicateMaps.Length; i++)
            {
                // It's better to allocate the `int` via closure than PropertyMap<T>
                int i1 = i;
                lazyProperties[this.mappingConfig.LazyPredicateMaps[i].Info.Property.Name] = new Lazy<object>(() =>
                {
                    //EnsureUmbracoContext();
                    return this.MapProperty(this.mappingConfig.LazyPredicateMaps[i1], content, result);
                });
            }

            return lazyProperties;
        }

        private void MapNonLazyProperties(IPublishedContent content, object destination)
        {
            // First map the non-lazy properties
            for (int i = 0; i < this.mappingConfig.NonLazyMaps.Length; i++)
            {
                PropertyMap<T> map = this.mappingConfig.NonLazyMaps[i] as PropertyMap<T>;
                object value = this.MapProperty(map, content, destination);
                if (value != null)
                {
                    this.mappingConfig.PropertyAccessor.SetValue(map.Info.Property.Name, destination, value);
                }
            }

            // Then non-lazy predicate mappings
            for (int i = 0; i < this.mappingConfig.NonLazyPredicateMaps.Length; i++)
            {
                PropertyMap<T> map = this.mappingConfig.NonLazyPredicateMaps[i] as PropertyMap<T>;
                object value = this.MapProperty(map, content, destination);
                if (value != null)
                {
                    this.mappingConfig.PropertyAccessor.SetValue(map.Info.Property.Name, destination, value);
                }
            }
        }

        private void MapLazyPropertiesAsNonLazy(IPublishedContent content, object destination)
        {
            // First map the lazy properties
            for (int i = 0; i < this.mappingConfig.LazyMaps.Length; i++)
            {
                PropertyMap<T> map = this.mappingConfig.LazyMaps[i] as PropertyMap<T>;
                object value = this.MapProperty(map, content, destination);
                if (value != null)
                {
                    this.mappingConfig.PropertyAccessor.SetValue(map.Info.Property.Name, destination, value);
                }
            }

            // Then lazy predicate mappings
            for (int i = 0; i < this.mappingConfig.LazyPredicateMaps.Length; i++)
            {
                PropertyMap<T> map = this.mappingConfig.LazyPredicateMaps[i] as PropertyMap<T>;
                object value = MapProperty(map, content, destination);
                if (value != null)
                {
                    this.mappingConfig.PropertyAccessor.SetValue(map.Info.Property.Name, destination, value);
                }
            }
        }

        private static object SantizeValue(object value, PropertyMapInfo info)
        {
            bool propertyIsCastableEnumerable = info.IsCastableEnumerableType;
            bool propertyIsConvertableEnumerable = info.IsConvertableEnumerableType;

            if (value != null)
            {
                Type valueType = value.GetType();
                if (valueType == info.PropertyType)
                {
                    return value;
                }

                bool valueIsConvertableEnumerable = valueType.IsConvertableEnumerableType();

                // You cannot set an enumerable of type from an empty object array.
                // This should allow the casting back of IEnumerable<T> to an empty List<T> Collection<T> etc.
                // I cant think of any that don't have an empty constructor
                if (value.Equals(UmbMapperConfigStatics.Empty) && propertyIsCastableEnumerable)
                {
                    Type typeArg = info.EnumerableParamType;
                    return info.PropertyType.IsInterface ? EnumerableInvocations.Cast(typeArg, (IEnumerable)value) : info.PropertyType.GetInstance();
                }

                // Ensure only a single item is returned when requested.
                if (valueIsConvertableEnumerable && !propertyIsConvertableEnumerable)
                {
                    // Property is not enumerable, but value is, so grab first item
                    IEnumerator enumerator = ((IEnumerable)value).GetEnumerator();
                    return enumerator.MoveNext() ? enumerator.Current : null;
                }

                // And now check for the reverse situation.
                if (!valueIsConvertableEnumerable && propertyIsConvertableEnumerable)
                {
                    var array = Array.CreateInstance(value.GetType(), 1);
                    array.SetValue(value, 0);
                    return array;
                }
            }
            else
            {
                if (propertyIsCastableEnumerable)
                {
                    if (info.PropertyType.IsInterface && !info.IsEnumerableOfKeyValueType)
                    {
                        // Value is null, but property is enumerable interface, so return empty enumerable
                        return EnumerableInvocations.Empty(info.EnumerableParamType);
                    }

                    // Concrete enumerables cannot be cast from Array so we create an instance and return it
                    // if we know it has an empty constructor.
                    ParameterInfo[] constructorParams = info.ConstructorParams;
                    if (constructorParams.Length == 0)
                    {
                        // Internally this uses Activator.CreateInstance which is heavily optimized.
                        return info.PropertyType.GetInstance();
                    }
                }
            }

            return value;
        }

        private object MapProperty(IPropertyMap map, IPublishedContent content, object result)
        {
            var propertyMap = map as PropertyMap<T>;

            return MapProperty(propertyMap, content, result, this.umbMapperService);
        }

        //private static object MapProperty(PropertyMap<T> map, IPublishedContent content, object result)
        private static object MapProperty(PropertyMap<T> map, IPublishedContent content, object result, IUmbMapperService umbMapperService)
        {
            object value = null;

            // If we have a mapping function, use that and skip Umbraco.
            if (map.Info.HasPredicate)
            {
                value = map.Predicate.Invoke((T)result, content);
            }
            else
            {
                // Get the raw value from the content.
                value = map.PropertyMapper.GetRawValue(content);

                // Now map using the given mappers.
                value = map.PropertyMapper.Map(content, value);
            }

            PropertyMapInfo info = map.Info;

            // Try to return if the value is correct.
            if (!(value.IsNullOrEmptyString() || value.Equals(info.DefaultValue))
                && info.PropertyType.IsInstanceOfType(value))
            {
                return value;
            }

            // Ensure everything is the correct return type.
            value = SantizeValue(value, info);

            if (value != null)
            {
                value = RecursivelyMap(value, info, umbMapperService);
            }

            return value;
        }

        public event RecursivelyMapSingle OnRecursivelyMapSingle;

        public event RecursivelyMapMultiple OnRecursivelyMapMultiple;
    }
}
