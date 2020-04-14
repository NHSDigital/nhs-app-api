using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Cipher;

namespace NHSOnline.Backend.GpSystems.UnitTests.SessionManager
{
    [TestClass]
    public sealed class MongoSessionCacheServiceTests
    {
        private Mock<ICipherService> _mockCipherService;
        private Mock<ILogger<MongoSessionCacheService>> _mockLogger;
        private Mock<IMongoClient> _mockMongoClient;
        private Mock<IMongoDatabase> _mockMongoDatabase;
        private Mock<IMongoCollection<BsonDocument>> _mockMongoCollection;
        private Mock<IMongoSessionCacheServiceConfig> _mockConfig;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockCipherService = new Mock<ICipherService>();
            _mockLogger = new Mock<ILogger<MongoSessionCacheService>>();
            _mockMongoClient = new Mock<IMongoClient>();
            _mockMongoDatabase = new Mock<IMongoDatabase>();
            _mockMongoCollection = new Mock<IMongoCollection<BsonDocument>>();
            _mockConfig = new Mock<IMongoSessionCacheServiceConfig>();

            _mockMongoClient
                .Setup(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(_mockMongoDatabase.Object);
            _mockMongoDatabase
                .Setup(x => x.GetCollection<BsonDocument>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(_mockMongoCollection.Object);
        }

        [TestMethod]
        public async Task GetUserSession_CreatedWithTppGpSession_ReturnsTppGpSession()
        {
            var userSession = new P9UserSession
            {
                GpUserSession = new TppUserSession()
            };

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            var sut = new MongoSessionCacheService(_mockCipherService.Object, _mockLogger.Object, _mockMongoClient.Object, _mockConfig.Object);
            var session = await sut.GetUserSession("");

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().GpUserSession.Should().BeOfType<TppUserSession>($"{nameof(TppUserSession)} should be deserialised");
        }

        [TestMethod]
        public async Task GetUserSession_CreatedWithEmisGpSession_ReturnsEmisGpSession()
        {
            var userSession = new P9UserSession
            {
                GpUserSession = new EmisUserSession()
            };

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            var sut = new MongoSessionCacheService(_mockCipherService.Object, _mockLogger.Object, _mockMongoClient.Object, _mockConfig.Object);
            var session = await sut.GetUserSession("");

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().GpUserSession.Should().BeOfType<EmisUserSession>($"{nameof(EmisUserSession)} should be deserialised");
        }

        [TestMethod]
        public async Task GetUserSession_CreatedWithMicrotestGpSession_ReturnsMicrotestGpSession()
        {
            var userSession = new P9UserSession
            {
                GpUserSession = new MicrotestUserSession()
            };

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            var sut = new MongoSessionCacheService(_mockCipherService.Object, _mockLogger.Object, _mockMongoClient.Object, _mockConfig.Object);
            var session = await sut.GetUserSession("");

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().GpUserSession.Should().BeOfType<MicrotestUserSession>($"{nameof(MicrotestUserSession)} should be deserialised");
        }

        [TestMethod]
        public async Task GetUserSession_CreatedWithVisionGpSession_ReturnsVisionGpSession()
        {
            var userSession = new P9UserSession
            {
                GpUserSession = new VisionUserSession()
            };

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            var sut = new MongoSessionCacheService(_mockCipherService.Object, _mockLogger.Object, _mockMongoClient.Object, _mockConfig.Object);
            var session = await sut.GetUserSession("");

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().GpUserSession.Should().BeOfType<VisionUserSession>($"{nameof(VisionUserSession)} should be deserialised");
        }

        [TestMethod]
        public async Task GetUserSession_StoredSessionBasedOnOldTypeName_ReturnsP9UserSession()
        {
            const string json =
                "{" +
                "\"$type\":\"NHSOnline.Backend.Support.UserSession, NHSOnline.Backend.Support\"," +
                "\"Key\":\"Keyf5687d2c-5857-450b-8625-a8b4c5b84068\"," +
                "\"CsrfToken\":\"CsrfTokend3aea227-7dcf-47ac-bf7c-5a61a8870dc9\"," +
                "\"GpUserSession\":{" +
                "\"$type\":\"NHSOnline.Backend.GpSystems.Suppliers.Tpp.TppUserSession, NHSOnline.Backend.GpSystems\"," +
                "\"Supplier\":2," +
                "\"HasLinkedAccounts\":true," +
                "\"Suid\":null," +
                "\"PatientId\":null," +
                "\"OnlineUserId\":null," +
                "\"UnitId\":null," +
                "\"Name\":null," +
                "\"NhsNumber\":null," +
                "\"OdsCode\":null," +
                "\"Im1MessagingEnabled\":false," +
                "\"Id\":\"00000000-0000-0000-0000-000000000000\"" +
                "}," +
                "\"CitizenIdUserSession\":{" +
                "\"$type\":\"NHSOnline.Backend.Support.CitizenIdUserSession, NHSOnline.Backend.Support\"," +
                "\"AccessToken\":\"AccessTokend9db6c38-f1c4-4231-81d7-181968250edb\"," +
                "\"FamilyName\":\"FamilyName8148b3f2-ee75-405b-8560-846bf35b2e20\"," +
                "\"DateOfBirth\":\"2021-01-25T00:13:15.961268\"," +
                "\"IdTokenJti\":\"IdTokenJtic7829065-b3aa-4504-8788-56d0d1dc6143\"" +
                "}," +
                "\"OrganDonationSessionId\":\"0495a20c-228e-44fd-9988-e6e0834d75c2\"," +
                "\"Im1ConnectionToken\":\"This is a Connection Token\"" +
                "}";

            ArrangeNoEncryption();
            ArrangeSessionData(json);

            var sut = new MongoSessionCacheService(_mockCipherService.Object, _mockLogger.Object, _mockMongoClient.Object, _mockConfig.Object);
            var session = await sut.GetUserSession("");

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().Should().BeOfType<P9UserSession>("P9UserSession should be deserialised");
        }

        private async Task<string> CreateSessionAndCaptureJson(P9UserSession userSession)
        {
            BsonDocument update = null;
            _mockMongoCollection
                .Setup(x => x.InsertOneAsync(
                    It.IsAny<BsonDocument>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()))
                .Callback<BsonDocument, InsertOneOptions, CancellationToken>((doc, opts, token) => update = doc);

            var sut = new MongoSessionCacheService(_mockCipherService.Object, _mockLogger.Object, _mockMongoClient.Object, _mockConfig.Object);
            await sut.CreateUserSession(userSession);

            return update["session"].AsString;
        }

        private void ArrangeNoEncryption()
        {
            _mockCipherService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns<string>(cipherText => cipherText);
            _mockCipherService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns<string>(input => input);
        }

        private void ArrangeSessionData(string json)
        {
            var document = new BsonDocument(new Dictionary<string, object> { {"session", json}});

            _mockMongoCollection
                .Setup(x => x.FindOneAndUpdateAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>(),
                    It.IsAny<FindOneAndUpdateOptions<BsonDocument, BsonDocument>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(document);
        }
    }
}
