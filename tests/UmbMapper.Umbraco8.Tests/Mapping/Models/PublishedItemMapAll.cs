using System;
namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class PublishedItemMapAll : UmbMapperConfig<PublishedItem>
    {
        public PublishedItemMapAll()
        {
            this.MapAll();
        }
    }
}
