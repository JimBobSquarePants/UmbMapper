using System;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper
{
    public interface IMappingProcessor
    {
        object Map(IPublishedElement content);
        void Map(IPublishedElement content, object destination);
        object CreateEmpty();
        object CreateEmpty(IPublishedElement content);
    }
}