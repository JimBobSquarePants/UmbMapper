using System;
using System.Collections.Generic;
using System.Linq;
using UmbMapper.Factories;
using UmbMapper.Invocations;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper
{
    public class UmbMapperService : IUmbMapperService
    {
        private readonly IUmbMapperRegistry umbMapperRegistry;
        private readonly IMappingProcessorFactory mappingProcessorFactory;

        public UmbMapperService(IUmbMapperRegistry umbMapperRegistry, IMappingProcessorFactory mappingProcessorFactory)
        {
            this.umbMapperRegistry = umbMapperRegistry;
            this.mappingProcessorFactory = mappingProcessorFactory;
        }

        public IEnumerable<T> MapTo<T>(IEnumerable<IPublishedElement> content) where T : class
        {
            return this.MapTo(content, typeof(T)).Select(x => x as T);
        }

        public IEnumerable<object> MapTo(IEnumerable<IPublishedElement> content, Type type)
        {
            IEnumerable<object> typedItems = content.Select(x => this.MapTo(x, type));

            // We need to cast back here as nothing is strong typed anymore.
            return (IEnumerable<object>)EnumerableInvocations.Cast(type, typedItems);
        }

        public T MapTo<T>(IPublishedElement content)
            where T : class
        {
            Type type = typeof(T);
            return (T)this.MapTo(content, type);
        }

        public object MapTo(IPublishedElement content, Type type)
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

            IMappingProcessor mappingProcessor = this.GetMappingProcessor(mapper);

            return mappingProcessor.Map(content);
        }

        public void MapTo<T>(IPublishedElement content, T destination)
        {
            Type type = typeof(T);
            this.MapTo(content, type, destination);
        }

        public void MapTo(IPublishedElement content, Type type, object destination)
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

            IMappingProcessor mappingProcessor = this.GetMappingProcessor(mapper);

            mappingProcessor.Map(content, destination);
        }

        private IMappingProcessor GetMappingProcessor(IUmbMapperConfig config)
        {
            IMappingProcessor mappingProcessor = this.mappingProcessorFactory.Create(config, this);
            //mappingProcessor.OnRecursivelyMapSingle += this.MappingProcessor_OnRecursivelyMapSingle;
            //mappingProcessor.OnRecursivelyMapMultiple += this.MappingProcessor_OnRecursivelyMapMultiple;

            return mappingProcessor;
        }

        private void MappingProcessor_OnRecursivelyMapMultiple(IEnumerable<IPublishedElement> items, Type type, out IEnumerable<object> returnObjects)
        {
            returnObjects = this.MapTo(items, type);
        }

        private void MappingProcessor_OnRecursivelyMapSingle(IPublishedElement item, Type type, out object returnObject)
        {
            returnObject = this.MapTo(item, type);
        }
    }
}
