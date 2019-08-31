using System;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper
{
    public interface IMappingProcessor
    {
        object Map(IPublishedContent content);
        void Map(IPublishedContent content, object destination);
        object CreateEmpty();
        object CreateEmpty(IPublishedContent content);
    }
}