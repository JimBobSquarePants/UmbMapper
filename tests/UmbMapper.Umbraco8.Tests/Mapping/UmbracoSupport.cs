using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Moq;
using Newtonsoft.Json;
using UmbMapper.Umbraco8.Tests.Mapping.Models;
using UmbMapper.Umbraco8.Tests.Mapping.Models.PropertyMapDefinitions;
using UmbMapper.Umbraco8.Tests.Mocks;
using UmbMapper.Umbraco8TestSupport.Objects;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.Core.Services;
//using Umbraco.Tests.TestHelpers;
//using Umbraco.Tests.Testing.Objects.Accessors;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

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
            //UmbMapperRegistry.ClearMappers();
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
            // Current.Factory = Mock.Of<IFactory>();
            //this.InitMappers();
        }

        public void InitMappers(IUmbMapperRegistry umbMapperRegistry)
        {
            //umbMapperRegistry.AddMapper<PublishedItemMap, PublishedItem>();  //umbMapperRegistry.AddMapper(new PublishedItemMap());
            ////umbMapperRegistry.AddMapper(new LazyPublishedItemMap());
            //umbMapperRegistry.AddMapperFor<AutoMappedItem>();
            //umbMapperRegistry.AddMapper<BackedPublishedItemMap, BackedPublishedItem>();  //umbMapperRegistry.AddMapper(new BackedPublishedItemMap());

            //umbMapperRegistry.AddMapper<InheritedPublishedItemMap, InheritedPublishedItem>(); // umbMapperRegistry.AddMapper(new InheritedPublishedItemMap());

            //umbMapperRegistry.AddMapper<CsvPublishedItemMap, CsvPublishedItem>();
            //umbMapperRegistry.AddMapperFor<PolymorphicItemOne>();
            //umbMapperRegistry.AddMapperFor<PolymorphicItemTwo>();
        }

        //public void InitFactoryMappers(IUmbMapperRegistry umbMapperRegistry)
        //{
        //    //umbMapperRegistry.AddMapper(new PublishedItemMapDefinition());
        //}

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

            this.TearDown();
            this.disposed = true;
        }
    }
}
