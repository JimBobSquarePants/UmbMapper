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
        IEnumerable<T> MapTo<T>(IEnumerable<IPublishedContent> content)
            where T : class;

        IEnumerable<object> MapTo(IEnumerable<IPublishedContent> content, Type type);

        T MapTo<T>(IPublishedContent content)
            where T : class;

        object MapTo(IPublishedContent content, Type type);

        void MapTo<T>(IPublishedContent content, T destination);

        void MapTo(IPublishedContent content, Type type, object destination);
    }
}
