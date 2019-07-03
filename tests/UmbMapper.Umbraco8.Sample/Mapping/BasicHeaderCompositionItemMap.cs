using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UmbMapper.PropertyMappers;
using UmbMapper.Umbraco8.Sample.Models.UmbracoDocTypes;

namespace UmbMapper.Umbraco8.Sample.Mapping
{
    public class BasicHeaderCompositionItemMap : UmbMapperConfig<BasicHeaderComposition>
    {
        public BasicHeaderCompositionItemMap()
        {
            this.MapAll();

            this.AddMap(x => x.Image).SetMapper<UmbracoPropertyMapper>();
        }
    }
}