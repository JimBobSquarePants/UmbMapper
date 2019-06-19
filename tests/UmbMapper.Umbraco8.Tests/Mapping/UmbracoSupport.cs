using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using UmbMapper.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mocks;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class UmbracoSupport : IDisposable
    {
        private bool disposed;
        private Link link;
        private ImageCropperValue dataSet;

        public UmbracoSupport()
        {
            this.Setup();
            this.InitPublishedProperties();
            this.InitMappers();
        }

        public MockPublishedContent Content => GetContent();

        private void TearDown()
        {
            UmbMapperRegistry.ClearMappers();
        }

        private void Setup()
        {
            //Composition.RegisterUnique<IPublishedModelFactory>(f => new PublishedModelFactory(f.GetInstance<TypeLoader>().GetTypes<PublishedContentModel>()));
        }

        private void InitPublishedProperties()
        {
            this.link = new Link
            {
                Name = "Test Caption",
                Target = "",
                Type = LinkType.Content,
                Url = ""
            };
        }

        private void InitMappers()
        {
            UmbMapperRegistry.AddMapper(new PublishedItemMap());
        }

        public MockPublishedContent GetContent()
        {
            return new MockPublishedContent
            {
                Properties = new[]
                {
                    new MockPublishedContentProperty(nameof(PublishedItem.PublishedContent), 1000, CreatePublishedPropertyType()),
                    new MockPublishedContentProperty(nameof(PublishedItem.PublishedInterfaceContent), 1001, CreatePublishedPropertyType()),
                    new MockPublishedContentProperty(nameof(PublishedItem.Image), this.dataSet, CreatePublishedPropertyType()),
                    new MockPublishedContentProperty(nameof(PublishedItem.Child), 3333, CreatePublishedPropertyType()),

                    // We're deliberately switching these values to test enumerable conversion
                    new MockPublishedContentProperty(nameof(PublishedItem.Link), this.link, CreatePublishedPropertyType()),
                    new MockPublishedContentProperty(nameof(PublishedItem.Links), new List<Link> { this.link }, CreatePublishedPropertyType() ),
                    new MockPublishedContentProperty(nameof(PublishedItem.NullLinks), null, CreatePublishedPropertyType()),

                    // Polymorphic collections
                    //new MockPublishedContentProperty(nameof(PublishedItem.Polymorphic)
                    //,
                    //new MockPublishedContent[]{

                    //    new MockPublishedContent()
                    //    {
                    //         = nameof(PolymorphicItemOne),
                    //        Properties = new[]{ new MockPublishedContentProperty(nameof(IPolyMorphic.PolyMorphicText),"Foo")}
                    //    },
                    //    new MockPublishedContent()
                    //    {
                    //        DocumentTypeAlias = nameof(PolymorphicItemTwo),
                    //        Properties = new[]{ new MockPublishedContentProperty(nameof(IPolyMorphic.PolyMorphicText),"Bar")}
                    //    }
                    //})
                }
            };
        }

        private PublishedPropertyType CreatePublishedPropertyType()
        {
            var mockPublishedContentTypeFactory = new Mock<IPublishedContentTypeFactory>();

            var publishedPropType = new PublishedPropertyType(
                new PublishedContentType(1234, "test", PublishedItemType.Content, Enumerable.Empty<string>(), Enumerable.Empty<PublishedPropertyType>(), ContentVariation.Nothing),
                new PropertyType("test", ValueStorageType.Nvarchar) { DataTypeId = 123 },
                new PropertyValueConverterCollection(Enumerable.Empty<IPropertyValueConverter>()),
                Mock.Of<IPublishedModelFactory>(), mockPublishedContentTypeFactory.Object);

            return publishedPropType;
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.TearDown();
            this.disposed = true;
        }
    }
}
