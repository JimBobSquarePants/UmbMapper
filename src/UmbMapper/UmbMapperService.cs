using System;
using System.Collections.Generic;
using System.Linq;
using UmbMapper.Invocations;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper
{
    public class UmbMapperService : IUmbMapperService
    {
        private readonly IUmbMapperRegistry umbMapperRegistry;

        public UmbMapperService(IUmbMapperRegistry umbMapperRegistry)
        {
            this.umbMapperRegistry = umbMapperRegistry;
        }

        public IEnumerable<T> MapTo<T>(IEnumerable<IPublishedContent> content) where T : class
        {
            return this.MapTo(content, typeof(T)).Select(x => x as T);
        }

        public IEnumerable<object> MapTo(IEnumerable<IPublishedContent> content, Type type)
        {
            IEnumerable<object> typedItems = content.Select(x => this.MapTo(x, type));

            // We need to cast back here as nothing is strong typed anymore.
            return (IEnumerable<object>)EnumerableInvocations.Cast(type, typedItems);
        }

        public T MapTo<T>(IPublishedContent content)
            where T : class
        {
            Type type = typeof(T);
            return (T)this.MapTo(content, type);
        }

        public object MapTo(IPublishedContent content, Type type)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.umbMapperRegistry.Mappers.TryGetValue(type, out IUmbMapperConfig mapper);

            if (mapper is null)
            {
                throw new InvalidOperationException($"No mapper for the given type {type} has been registered.");
            }

            // TODO change this  type of structure
            // instead of getting a mapper and have it responsible for the mapping
            // IUmbMapperConfig has the config and something like 
            // IUmbMapperProcessor.Map(IUmbMapperConfig mapperConfig, content)
            // performs the actual mapping 
            return mapper.Map(content);
        }

        public void MapTo<T>(IPublishedContent content, T destination)
        {
            Type type = typeof(T);
            this.MapTo(content, type, destination);
        }

        public void MapTo(IPublishedContent content, Type type, object destination)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.umbMapperRegistry.Mappers.TryGetValue(type, out IUmbMapperConfig mapper);

            if (mapper is null)
            {
                throw new InvalidOperationException($"No mapper for the given type {type} has been registered.");
            }

            mapper.Map(content, destination);
        }
    }
}
