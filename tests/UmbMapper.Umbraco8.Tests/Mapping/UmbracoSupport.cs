using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Newtonsoft.Json;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mocks;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
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
        IFactory Factory { get; set; }

        public UmbracoSupport()
        {
            this.Setup();
            this.InitPublishedProperties();
        }

        public MockPublishedContent Content => this.GetContent();

        private void TearDown()
        {
            UmbMapperRegistry.ClearMappers();
            //Current.Reset();
        }

        private void Setup()
        {
            /*
             * Ideally we want to mock a composition to do everything
            var compositionMock = new Mock<Composition>();

            this.Composition = compositionMock.Object;
            this.Composition.WithCollectionBuilder<PropertyValueConverterCollectionBuilder>();
            */

            //Current.Factory = Factory = Mock.Of<IFactory>();
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
                    new MockPublishedProperty(nameof(PublishedItem.PublishedContent), 1000, this.GetPublishedPropertyType(nameof(PublishedItem.PublishedContent))),
                    new MockPublishedProperty(nameof(PublishedItem.PublishedInterfaceContent), 1001, this.GetPublishedPropertyType()),
                    new MockPublishedProperty(nameof(PublishedItem.Image), this.dataSet, this.GetPublishedPropertyType("image")),
                    new MockPublishedProperty(nameof(PublishedItem.Child), 3333, this.GetPublishedPropertyType()),

                    // We're deliberately switching these values to test enumerable conversion
                    new MockPublishedProperty(nameof(PublishedItem.Link), this.link, this.GetPublishedPropertyType("link")),
                    new MockPublishedProperty(nameof(PublishedItem.Links), new List<Link> { this.link }, this.GetPublishedPropertyType("links") ),
                    new MockPublishedProperty(nameof(PublishedItem.NullLinks), null, this.GetPublishedPropertyType()),

                    // Polymorphic collections
                    new MockPublishedProperty(
                        nameof(PublishedItem.Polymorphic),
                        new MockPublishedContent[]{

                            new MockPublishedContent()
                            {
                                ContentType = new PublishedContentType(1, nameof(PolymorphicItemOne), PublishedItemType.Content,Enumerable.Empty<string>(), Enumerable.Empty<PublishedPropertyType>(), ContentVariation.Nothing),
                                Properties = new[]{ new MockPublishedProperty(nameof(IPolyMorphic.PolyMorphicText),"Foo", this.GetPublishedPropertyType(nameof(IPolyMorphic.PolyMorphicText))) }
                            },
                            new MockPublishedContent()
                            {
                                ContentType = new PublishedContentType(1, nameof(PolymorphicItemTwo), PublishedItemType.Content,Enumerable.Empty<string>(), Enumerable.Empty<PublishedPropertyType>(), ContentVariation.Nothing),
                                Properties = new[]{ new MockPublishedProperty(nameof(IPolyMorphic.PolyMorphicText),"Bar", this.GetPublishedPropertyType(nameof(IPolyMorphic.PolyMorphicText))) }
                            }
                        },
                        this.GetPublishedPropertyType(nameof(PublishedItem.Polymorphic))
                    )
                }
            };
        }

        protected virtual AppCaches GetAppCaches()
        {
            return AppCaches.Disabled;
        }

        public PublishedPropertyType GetPublishedPropertyType(string alias="test")
        {
            var mockPublishedContentTypeFactory = new Mock<IPublishedContentTypeFactory>();

            var publishedPropType = new PublishedPropertyType(
                alias,
                1,
                true,
                ContentVariation.CultureAndSegment,
                new PropertyValueConverterCollection(Enumerable.Empty<IPropertyValueConverter>()),
                Mock.Of<IPublishedModelFactory>(),
                mockPublishedContentTypeFactory.Object);

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
