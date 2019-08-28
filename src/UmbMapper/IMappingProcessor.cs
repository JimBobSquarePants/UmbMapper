using System;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper
{
    public delegate void RecursivelyMapSingle(IPublishedContent item, Type type, out object returnObject);
    public delegate void RecursivelyMapMultiple(IEnumerable<IPublishedContent> items, Type type, out IEnumerable<object> returnObjects);

    public interface IMappingProcessor
    {
        object Map(IPublishedContent content);
        void Map(IPublishedContent content, object destination);
        object CreateEmpty();
        object CreateEmpty(IPublishedContent content);

        event RecursivelyMapSingle OnRecursivelyMapSingle;

        event RecursivelyMapMultiple OnRecursivelyMapMultiple;
    }
}
