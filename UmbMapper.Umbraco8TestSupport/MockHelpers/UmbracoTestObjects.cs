//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Moq;
//using Umbraco.Core;
//using Umbraco.Core.Composing;
//using Umbraco.Core.Services;
//using Umbraco.Web;
//using UmbMapper.Umbraco8TestSupport.Factories;
//using Umbraco.Web.PublishedCache;
//using Umbraco.Web.Routing;
//using UmbMapper.Umbraco8TestSupport.Objects;
//using System.Web;
//using Umbraco.Core.Configuration.UmbracoSettings;
//using Umbraco.Core.Configuration;
//using Umbraco.Core.IO;
//using System.Linq.Expressions;
//using System.Data;
//using Umbraco.Core.Models;

//namespace UmbMapper.Umbraco8TestSupport.MockHelpers
//{
//    /// <summary>
//    /// Provides objects for tests.
//    /// Copied from Umbraco.Tests.TestHelpers.TestObjects (internal so can't use directly)
//    /// </summary>
//    public static class UmbracoTestObjects
//    {
//        //public IUmbracoDatabaseFactory GetDatabaseFactoryMock(bool configured = true, bool canConnect = true)
//        //{
//        //    var databaseFactoryMock = new Mock<IUmbracoDatabaseFactory>();
//        //    databaseFactoryMock.Setup(x => x.Configured).Returns(configured);
//        //    databaseFactoryMock.Setup(x => x.CanConnect).Returns(canConnect);
//        //    databaseFactoryMock.Setup(x => x.SqlContext).Returns(Mock.Of<ISqlContext>());

//        //    // can create a database - but don't try to use it!
//        //    if (configured && canConnect)
//        //        databaseFactoryMock.Setup(x => x.CreateDatabase()).Returns(GetUmbracoSqlCeDatabase(Mock.Of<ILogger>()));

//        //    return databaseFactoryMock.Object;
//        //}

//        public static ServiceContext GetServiceContextMock(IFactory container = null)
//        {
//            // FIXME: else some tests break - figure it out
//            container = null;

//            return ServiceContext.CreatePartial(
//                MockService<IContentService>(container),
//                MockService<IMediaService>(container),
//                MockService<IContentTypeService>(container),
//                MockService<IMediaTypeService>(container),
//                MockService<IDataTypeService>(container),
//                MockService<IFileService>(container),
//                MockService<ILocalizationService>(container),
//                MockService<IPackagingService>(container),
//                MockService<IEntityService>(container),
//                MockService<IRelationService>(container),
//                MockService<IMemberGroupService>(container),
//                MockService<IMemberTypeService>(container),
//                MockService<IMemberService>(container),
//                MockService<IUserService>(container),
//                MockService<ITagService>(container),
//                MockService<INotificationService>(container),
//                MockService<ILocalizedTextService>(container),
//                MockService<IAuditService>(container),
//                MockService<IDomainService>(container),
//                MockService<IMacroService>(container));
//        }

//        private static T MockService<T>(IFactory container = null)
//            where T : class
//        {
//            return container?.TryGetInstance<T>() ?? new Mock<T>().Object;
//        }

//        /// <summary>
//        /// Gets an opened database connection that can begin a transaction.
//        /// </summary>
//        /// <returns>A DbConnection.</returns>
//        /// <remarks>This is because NPoco wants a DbConnection, NOT an IDbConnection,
//        /// and DbConnection is hard to mock so we create our own class here.</remarks>
//        public static DbConnection GetDbConnection()
//        {
//            return new MockDbConnection();
//        }

//        /// <summary>
//        /// Gets an Umbraco context.
//        /// </summary>
//        /// <returns>An Umbraco context.</returns>
//        /// <remarks>This should be the minimum Umbraco context.</remarks>
//        public static UmbracoContext GetUmbracoContextMock(IUmbracoContextAccessor accessor = null, string url = "http://mywebsite/")
//        {
//            var httpContext = new FakeHttpContextFactory(url).HttpContext;
//            //var httpContext = Mock.Of<HttpContextBase>();

//            var publishedSnapshotMock = new Mock<IPublishedSnapshot>();
//            publishedSnapshotMock.Setup(x => x.Members).Returns(Mock.Of<IPublishedMemberCache>());
//            var publishedSnapshot = publishedSnapshotMock.Object;
//            var publishedSnapshotServiceMock = new Mock<IPublishedSnapshotService>();
//            publishedSnapshotServiceMock.Setup(x => x.CreatePublishedSnapshot(It.IsAny<string>())).Returns(publishedSnapshot);
//            var publishedSnapshotService = publishedSnapshotServiceMock.Object;

//            var umbracoSettings = GetUmbracoSettings();
//            var globalSettings = GetGlobalSettings();
//            var urlProviders = new UrlProviderCollection(Enumerable.Empty<IUrlProvider>());
//            //var mediaUrlProviders = new MediaUrlProviderCollection(Enumerable.Empty<IMediaUrlProvider>());

//            if (accessor == null) accessor = new TestUmbracoContextAccessor();

//            var umbracoContextFactory = new UmbracoContextFactory(
//                accessor,
//                publishedSnapshotService,
//                new TestVariationContextAccessor(),
//                new TestDefaultCultureAccessor(),
//                umbracoSettings,
//                globalSettings,
//                urlProviders,
//                // mediaUrlProviders,
//                Mock.Of<IUserService>());

//            return umbracoContextFactory.EnsureUmbracoContext(httpContext).UmbracoContext;
//        }

//        public static UmbracoContext GetUmbracoContextMockWithDummyData(IUmbracoContextAccessor accessor = null)
//        {
//            var httpContext = Mock.Of<HttpContextBase>();

//            var publishedSnapshotMock = new Mock<IPublishedSnapshot>();
//            publishedSnapshotMock.Setup(x => x.Members).Returns(Mock.Of<IPublishedMemberCache>());
//            var publishedSnapshot = publishedSnapshotMock.Object;
//            var publishedSnapshotServiceMock = new Mock<IPublishedSnapshotService>();
//            publishedSnapshotServiceMock.Setup(x => x.CreatePublishedSnapshot(It.IsAny<string>())).Returns(publishedSnapshot);
//            var publishedSnapshotService = publishedSnapshotServiceMock.Object;

//            var umbracoSettings = GetUmbracoSettings();
//            var globalSettings = GetGlobalSettings();
//            var urlProviders = new UrlProviderCollection(Enumerable.Empty<IUrlProvider>());

//            if (accessor == null) accessor = new TestUmbracoContextAccessor();

//            var umbracoContextFactory = new UmbracoContextFactory(
//                accessor,
//                publishedSnapshotService,
//                new TestVariationContextAccessor(),
//                new TestDefaultCultureAccessor(),
//                umbracoSettings,
//                globalSettings,
//                urlProviders,
//                Mock.Of<IUserService>());

//            return umbracoContextFactory.EnsureUmbracoContext(httpContext).UmbracoContext;
//        }

//        public static IUmbracoSettingsSection GetUmbracoSettings()
//        {
//            // FIXME: Why not use the SettingsForTest.GenerateMock ... ?
//            // FIXME: Shouldn't we use the default ones so they are the same instance for each test?

//            var umbracoSettingsMock = new Mock<IUmbracoSettingsSection>();
//            var webRoutingSectionMock = new Mock<IWebRoutingSection>();
//            //webRoutingSectionMock.Setup(x => x.UrlProviderMode).Returns(Umbraco.Web.Routing.UrlProviderMode.Auto.ToString());
//            //TODO
//            webRoutingSectionMock.Setup(x => x.UrlProviderMode).Returns("Auto");
//            umbracoSettingsMock.Setup(x => x.WebRouting).Returns(webRoutingSectionMock.Object);
//            return umbracoSettingsMock.Object;
//        }

//        public static IGlobalSettings GetGlobalSettings()
//        {
//            const string StaticReservedPaths = "~/app_plugins/,~/install/,~/mini-profiler-resources/,"; //must end with a comma!
//            const string StaticReservedUrls = "~/config/splashes/noNodes.aspx,~/.well-known,"; //must end with a comma!

//            var config = Mock.Of<IGlobalSettings>(
//                settings =>
//                    settings.ConfigurationStatus == UmbracoVersion.SemanticVersion.ToSemanticString() &&
//                    settings.UseHttps == false &&
//                    settings.HideTopLevelNodeFromPath == false &&
//                    settings.Path == IOHelper.ResolveUrl("~/umbraco") &&
//                    settings.TimeOutInMinutes == 20 &&
//                    settings.DefaultUILanguage == "en" &&
//                    settings.LocalTempStorageLocation == LocalTempStorage.Default &&
//                    settings.LocalTempPath == IOHelper.MapPath("~/App_Data/TEMP") &&
//                    settings.ReservedPaths == (StaticReservedPaths + "~/umbraco") &&
//                    settings.ReservedUrls == StaticReservedUrls);
//            return config;
//        }

//        public static IFileSystems GetFileSystemsMock()
//        {
//            var fileSystems = Mock.Of<IFileSystems>();

//            MockFs(fileSystems, x => x.MacroPartialsFileSystem);
//            MockFs(fileSystems, x => x.MvcViewsFileSystem);
//            MockFs(fileSystems, x => x.PartialViewsFileSystem);
//            MockFs(fileSystems, x => x.ScriptsFileSystem);
//            MockFs(fileSystems, x => x.StylesheetsFileSystem);

//            return fileSystems;
//        }

//        private static void MockFs(IFileSystems fileSystems, Expression<Func<IFileSystems, IFileSystem>> fileSystem)
//        {
//            var fs = Mock.Of<IFileSystem>();
//            Mock.Get(fileSystems).Setup(fileSystem).Returns(fs);
//        }

//        #region Inner classes

//        private class MockDbConnection : DbConnection
//        {
//            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
//            {
//                return Mock.Of<DbTransaction>(); // enough here
//            }

//            public override void Close()
//            {
//                throw new NotImplementedException();
//            }

//            public override void ChangeDatabase(string databaseName)
//            {
//                throw new NotImplementedException();
//            }

//            public override void Open()
//            {
//                throw new NotImplementedException();
//            }

//            public override string ConnectionString { get; set; }

//            protected override DbCommand CreateDbCommand()
//            {
//                throw new NotImplementedException();
//            }

//            public override string Database { get; }
//            public override string DataSource { get; }
//            public override string ServerVersion { get; }
//            public override ConnectionState State => ConnectionState.Open; // else NPoco reopens
//        }

//        public class TestDataTypeService : IDataTypeService
//        {
//            public TestDataTypeService()
//            {
//                DataTypes = new Dictionary<int, IDataType>();
//            }

//            public TestDataTypeService(params IDataType[] dataTypes)
//            {
//                DataTypes = dataTypes.ToDictionary(x => x.Id, x => x);
//            }

//            public TestDataTypeService(IEnumerable<IDataType> dataTypes)
//            {
//                DataTypes = dataTypes.ToDictionary(x => x.Id, x => x);
//            }

//            public Dictionary<int, IDataType> DataTypes { get; }

//            public Attempt<OperationResult<OperationResultType, EntityContainer>> CreateContainer(int parentId, string name, int userId = -1)
//            {
//                throw new NotImplementedException();
//            }

//            public Attempt<OperationResult> SaveContainer(EntityContainer container, int userId = -1)
//            {
//                throw new NotImplementedException();
//            }

//            public EntityContainer GetContainer(int containerId)
//            {
//                throw new NotImplementedException();
//            }

//            public EntityContainer GetContainer(Guid containerId)
//            {
//                throw new NotImplementedException();
//            }

//            public IEnumerable<EntityContainer> GetContainers(string folderName, int level)
//            {
//                throw new NotImplementedException();
//            }

//            public IEnumerable<EntityContainer> GetContainers(IDataType dataType)
//            {
//                throw new NotImplementedException();
//            }

//            public IEnumerable<EntityContainer> GetContainers(int[] containerIds)
//            {
//                throw new NotImplementedException();
//            }

//            public Attempt<OperationResult> DeleteContainer(int containerId, int userId = -1)
//            {
//                throw new NotImplementedException();
//            }

//            public Attempt<OperationResult<OperationResultType, EntityContainer>> RenameContainer(int id, string name, int userId = -1)
//            {
//                throw new NotImplementedException();
//            }

//            public IDataType GetDataType(string name)
//            {
//                throw new NotImplementedException();
//            }

//            public IDataType GetDataType(int id)
//            {
//                DataTypes.TryGetValue(id, out var dataType);
//                return dataType;
//            }

//            public IDataType GetDataType(Guid id)
//            {
//                throw new NotImplementedException();
//            }

//            public IEnumerable<IDataType> GetAll(params int[] ids)
//            {
//                if (ids.Length == 0) return DataTypes.Values;
//                return ids.Select(x => DataTypes.TryGetValue(x, out var dataType) ? dataType : null).WhereNotNull();
//            }

//            public void Save(IDataType dataType, int userId = -1)
//            {
//                throw new NotImplementedException();
//            }

//            public void Save(IEnumerable<IDataType> dataTypeDefinitions, int userId = -1)
//            {
//                throw new NotImplementedException();
//            }

//            public void Save(IEnumerable<IDataType> dataTypeDefinitions, int userId, bool raiseEvents)
//            {
//                throw new NotImplementedException();
//            }

//            public void Delete(IDataType dataType, int userId = -1)
//            {
//                throw new NotImplementedException();
//            }

//            public IEnumerable<IDataType> GetByEditorAlias(string propertyEditorAlias)
//            {
//                throw new NotImplementedException();
//            }

//            public Attempt<OperationResult<MoveOperationStatusType>> Move(IDataType toMove, int parentId)
//            {
//                throw new NotImplementedException();
//            }
//        }

//        #endregion

//    }
//}
