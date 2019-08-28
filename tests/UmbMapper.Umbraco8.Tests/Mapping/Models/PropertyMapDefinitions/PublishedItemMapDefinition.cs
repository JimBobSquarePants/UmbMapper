using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmbMapper.Models;
using UmbMapper.PropertyMappers;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models.PropertyMapDefinitions
{
    public class PublishedItemMapDefinition : MappingDefinition<PublishedItem>
    {
        public PublishedItemMapDefinition()
        {
            this.AddMappingDefinition(x => x.Id).SetMapper<UmbracoPropertyMapper>();

            this.AddMappingDefinition(p => p.Name).SetMapper<UmbracoPropertyMapper>();
            this.AddMappingDefinition(p => p.CreateDate).SetMapper<UmbracoPropertyMapper>();
            this.AddMappingDefinition(p => p.UpdateDate).SetAlias(p => p.UpdateDate, p => p.CreateDate).SetMapper<UmbracoPropertyMapper>();
            this.AddMappingDefinition(p => p.PlaceOrder).SetMapper<EnumPropertyMapper>();
            this.AddMappingDefinition(p => p.Image); // as we can mock the composition with property value editors
            this.AddMappingDefinition(p => p.Link);
            this.AddMappingDefinition(p => p.Links);
            this.AddMappingDefinition(p => p.NullLinks);
            this.AddMappingDefinition(p => p.Polymorphic).SetFactoryMapper<DocTypeFactoryPropertyMapper>();
            this.AddMappingDefinition(p => p.PublishedContent);
            this.AddMappingDefinition(p => p.PublishedInterfaceContent); //.SetMapper<UmbracoPickerPropertyMapper>();
            this.AddMappingDefinition(p => p.Child); //.SetMapp
        }
    }
}
