using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace UmbMapper
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUmbMapperService
    {
        IEnumerable<T> MapTo<T>(IEnumerable<IPublishedElement> content)
            where T : class;

        IEnumerable<object> MapTo(IEnumerable<IPublishedElement> content, Type type);

        T MapTo<T>(IPublishedElement content)
            where T : class;

        object MapTo(IPublishedElement content, Type type);

        void MapTo<T>(IPublishedElement content, T destination);

        void MapTo(IPublishedElement content, Type type, object destination);
    }
}
