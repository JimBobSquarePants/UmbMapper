using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mapping.Models.PropertyMapDefinitions;
using UmbMapper.Umbraco8.Tests.Mocks;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.Web.Models;

namespace UmbMapper.Umbraco8.Tests.Mapping
{
    public class UmbracoSupport : IDisposable
    {
        private bool disposed;
        private Link link;
        private ImageCropperValue dataSet;

        protected Composition Composition { get; private set; }

        public UmbracoSupport()
        {
            this.InitPublishedProperties();
        }

        public MockPublishedContent Content => this.GetContent();
        

        public void InitFactoryMappers(IUmbMapperInitialiser umbMapperInitialiser)
        {
            umbMapperInitialiser.AddMapper(new PublishedItemMapDefinition());
            umbMapperInitialiser.AddMapper(new BackedPublishedItemMapDefinition());
            umbMapperInitialiser.AddMapper(new CsvPublishedItemMapDefinition());
            umbMapperInitialiser.AddMapper(new InheritedPublishedItemMapDefinition());
            umbMapperInitialiser.AddMapper(new LazyPublishedItemMap());
            umbMapperInitialiser.AddMapperFor<AutoMappedItem>();
            umbMapperInitialiser.AddMapperFor<PolymorphicItemOne>();
            umbMapperInitialiser.AddMapperFor<PolymorphicItemTwo>();
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
            // JSON test data taken from Umbraco unit-test:
            // https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Tests/PropertyEditors/ImageCropperTest.cs
            string json = "{\"focalPoint\": {\"left\": 0.96,\"top\": 0.80827067669172936},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\":\"thumb\",\"width\": 100,\"height\": 100,\"coordinates\": {\"x1\": 0.58729977382575338,\"y1\": 0.055768992440203169,\"x2\": 0,\"y2\": 0.32457553600198386}}]}";
            this.dataSet = JsonConvert.DeserializeObject<ImageCropperValue>(json);
        }

        public MockPublishedContent GetContent()
        {
            return new MockPublishedContent
            {
                Properties = new[]
                {
                    new MockPublishedProperty(nameof(PublishedItem.PublishedContent), 1000, Mocks.UmbMapperMockFactory.CreateMockUmbracoContentPublishedPropertyType()),
                    new MockPublishedProperty(nameof(PublishedItem.PublishedInterfaceContent), 1001, Mocks.UmbMapperMockFactory.CreateMockUmbracoContentPublishedPropertyType()),
                    new MockPublishedProperty(nameof(PublishedItem.Image), this.dataSet),
                    new MockPublishedProperty(nameof(PublishedItem.Child), 1003, Mocks.UmbMapperMockFactory.CreateMockUmbracoContentPublishedPropertyType()),

                    // We're deliberately switching these values to test enumerable conversion
                    new MockPublishedProperty(nameof(PublishedItem.Link), this.link),
                    new MockPublishedProperty(nameof(PublishedItem.Links), new List<Link> { this.link } ),
                    new MockPublishedProperty(nameof(PublishedItem.NullLinks), null),

                    // Polymorphic collections
                    new MockPublishedProperty(
                        nameof(PublishedItem.Polymorphic),
                        new MockPublishedContent[]{

                            new MockPublishedContent()
                            {
                                ContentType = new PublishedContentType(1, nameof(Models.PolymorphicItemOne), PublishedItemType.Content,Enumerable.Empty<string>(), Enumerable.Empty<PublishedPropertyType>(), ContentVariation.Nothing),
                                Properties = new[]{ new MockPublishedProperty(nameof(IPolyMorphic.PolyMorphicText),"Foo") }
                            },
                            new MockPublishedContent()
                            {
                                ContentType = new PublishedContentType(1, nameof(Models.PolymorphicItemTwo), PublishedItemType.Content,Enumerable.Empty<string>(), Enumerable.Empty<PublishedPropertyType>(), ContentVariation.Nothing),
                                Properties = new[]{ new MockPublishedProperty(nameof(IPolyMorphic.PolyMorphicText),"Bar") }
                            }
                        }
                    )
                }
            };
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
        }
    }
}
