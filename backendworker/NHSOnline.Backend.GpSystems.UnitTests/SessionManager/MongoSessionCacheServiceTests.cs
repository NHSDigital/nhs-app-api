using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json.Linq;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Cipher;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.GpSystems.UnitTests.SessionManager
{
    [TestClass]
    public sealed class MongoSessionCacheServiceTests
    {
        private Mock<ICipherService> _mockCipherService;
        private Mock<ILogger<MongoSessionCacheService>> _mockLogger;
        private Mock<IMongoSessionCacheServiceConfig> _mockConfig;
        private Mock<IMongoClientService> _mockMongoClientService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockCipherService = new Mock<ICipherService>();
            _mockLogger = new Mock<ILogger<MongoSessionCacheService>>();
            _mockConfig = new Mock<IMongoSessionCacheServiceConfig>();
            _mockMongoClientService = new Mock<IMongoClientService>();
        }

        [TestMethod]
        public async Task CreateUserSession_P9UserSession_SetsTypeNameAsP9UserSession()
        {
            // Arrange
            var userSession = new P9UserSession(
                string.Empty,
                string.Empty,
                new CitizenIdUserSession(),
                new TppUserSession(),
                string.Empty);

            ArrangeNoEncryption();

            // Act
            var json = await CreateSessionAndCaptureJson(userSession);

            // Assert
            JObject
                .Parse(json)
                .SelectToken("$type")
                .Should().NotBeNull()
                .And.BeOfType<JValue>()
                .Which.Value.Should().BeOfType<string>()
                .Which.Should().StartWith("P9UserSession, ");
        }

        [TestMethod]
        public async Task CreateUserSession_P5UserSession_SetsTypeNameAsP5UserSession()
        {
            // Arrange
            var userSession = new P5UserSession(string.Empty, new CitizenIdUserSession());

            ArrangeNoEncryption();

            // Act
            var json = await CreateSessionAndCaptureJson(userSession);

            // Assert
            JObject
                .Parse(json)
                .SelectToken("$type")
                .Should().NotBeNull()
                .And.BeOfType<JValue>()
                .Which.Value.Should().BeOfType<string>()
                .Which.Should().StartWith("P5UserSession, ");
        }

        [TestMethod]
        public async Task GetUserSession_CreatedWithTppGpSession_ReturnsTppGpSession()
        {
            // Arrange
            var userSession = new P9UserSession(
                string.Empty,
                string.Empty,
                new CitizenIdUserSession(),
                new TppUserSession(),
                string.Empty);

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.InsertOneDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<BsonDocument>()));

            _mockMongoClientService.Verify(x => x.FindFirstDocument(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure()
                .Should().BeOfType<P9UserSession>().Subject
                .GpUserSession.Should().BeOfType<TppUserSession>($"{nameof(TppUserSession)} should be deserialised");
        }

        [TestMethod]
        public async Task GetAndUpdateUserSession_CreatedWithTppGpSession_ReturnsTppGpSession()
        {
            // Arrange
            var userSession = new P9UserSession(
                string.Empty,
                string.Empty,
                new CitizenIdUserSession(),
                new TppUserSession(),
                string.Empty);

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetAndUpdateUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.InsertOneDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<BsonDocument>()));

            _mockMongoClientService.Verify(x => x.FindOneAndUpdateDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure()
                .Should().BeOfType<P9UserSession>().Subject
                .GpUserSession.Should().BeOfType<TppUserSession>($"{nameof(TppUserSession)} should be deserialised");
        }

        [TestMethod]
        public async Task GetUserSession_CreatedWithEmisGpSession_ReturnsEmisGpSession()
        {
            // Arrange
            var userSession = new P9UserSession(
                string.Empty,
                string.Empty,
                new CitizenIdUserSession(),
                new EmisUserSession(),
                string.Empty);

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.InsertOneDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<BsonDocument>()));

            _mockMongoClientService.Verify(x => x.FindFirstDocument(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure()
                .Should().BeOfType<P9UserSession>().Subject
                .GpUserSession.Should().BeOfType<EmisUserSession>($"{nameof(EmisUserSession)} should be deserialised");
        }

        [TestMethod]
        public async Task GetAndUpdateUserSession_CreatedWithEmisGpSession_ReturnsEmisGpSession()
        {
            // Arrange
            var userSession = new P9UserSession(
                string.Empty,
                string.Empty,
                new CitizenIdUserSession(),
                new EmisUserSession(),
                string.Empty);

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetAndUpdateUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.InsertOneDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<BsonDocument>()));

            _mockMongoClientService.Verify(x => x.FindOneAndUpdateDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure()
                .Should().BeOfType<P9UserSession>().Subject
                .GpUserSession.Should().BeOfType<EmisUserSession>($"{nameof(EmisUserSession)} should be deserialised");
        }

        [TestMethod]
        public async Task GetUserSession_CreatedWithVisionGpSession_ReturnsVisionGpSession()
        {
            // Arrange
            var userSession = new P9UserSession(
                string.Empty,
                string.Empty,
                new CitizenIdUserSession(),
                new VisionUserSession(),
                string.Empty);

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.InsertOneDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<BsonDocument>()));

            _mockMongoClientService.Verify(x => x.FindFirstDocument(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure()
                .Should().BeOfType<P9UserSession>().Subject
                .GpUserSession.Should()
                .BeOfType<VisionUserSession>($"{nameof(VisionUserSession)} should be deserialised");
        }

        [TestMethod]
        public async Task GetAndUpdateUserSession_CreatedWithVisionGpSession_ReturnsVisionGpSession()
        {
            // Arrange
            var userSession = new P9UserSession(
                string.Empty,
                string.Empty,
                new CitizenIdUserSession(),
                new VisionUserSession(),
                string.Empty);

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetAndUpdateUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.InsertOneDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<BsonDocument>()));

            _mockMongoClientService.Verify(x => x.FindOneAndUpdateDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure()
                .Should().BeOfType<P9UserSession>().Subject
                .GpUserSession.Should()
                .BeOfType<VisionUserSession>($"{nameof(VisionUserSession)} should be deserialised");
        }

        [TestMethod]
        public async Task GetUserSession_CreatedWithP5Session_ReturnsP5Session()
        {
            // Arrange
            var userSession = new P5UserSession(string.Empty, new CitizenIdUserSession { ProofLevel = ProofLevel.P5 });

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.InsertOneDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<BsonDocument>()));

            _mockMongoClientService.Verify(x => x.FindFirstDocument(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            var actual = session.ValueOrFailure().Should().BeOfType<P5UserSession>().Subject;
            actual.CitizenIdUserSession.Should().NotBeNull();
            actual.CitizenIdUserSession.ProofLevel.Should().Be(ProofLevel.P5);
        }

        [TestMethod]
        public async Task GetAndUpdateUserSession_CreatedWithP5Session_ReturnsP5Session()
        {
            // Arrange
            var userSession = new P5UserSession(string.Empty, new CitizenIdUserSession { ProofLevel = ProofLevel.P5 });

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetAndUpdateUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.InsertOneDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<BsonDocument>()));

            _mockMongoClientService.Verify(x => x.FindOneAndUpdateDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            var actual = session.ValueOrFailure().Should().BeOfType<P5UserSession>().Subject;
            actual.CitizenIdUserSession.Should().NotBeNull();
            actual.CitizenIdUserSession.ProofLevel.Should().Be(ProofLevel.P5);
        }

        [TestMethod]
        public async Task GetUserSession_CreatedWithP9Session_ReturnsP9Session()
        {
            // Arrange
            var userSession = new P9UserSession(
                string.Empty,
                string.Empty,
                new CitizenIdUserSession { ProofLevel = ProofLevel.P9 },
                new EmisUserSession(),
                string.Empty);

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.InsertOneDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<BsonDocument>()));

            _mockMongoClientService.Verify(x => x.FindFirstDocument(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            var actual = session.ValueOrFailure().Should().BeOfType<P9UserSession>().Subject;
            actual.CitizenIdUserSession.Should().NotBeNull();
            actual.CitizenIdUserSession.ProofLevel.Should().Be(ProofLevel.P9);
        }

        [TestMethod]
        public async Task GetAndUpdateUserSession_CreatedWithP9Session_ReturnsP9Session()
        {
            // Arrange
            var userSession = new P9UserSession(
                string.Empty,
                string.Empty,
                new CitizenIdUserSession { ProofLevel = ProofLevel.P9 },
                new EmisUserSession(),
                string.Empty);

            ArrangeNoEncryption();
            var json = await CreateSessionAndCaptureJson(userSession);
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetAndUpdateUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.InsertOneDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<BsonDocument>()));

            _mockMongoClientService.Verify(x => x.FindOneAndUpdateDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            var actual = session.ValueOrFailure().Should().BeOfType<P9UserSession>().Subject;
            actual.CitizenIdUserSession.Should().NotBeNull();
            actual.CitizenIdUserSession.ProofLevel.Should().Be(ProofLevel.P9);
        }

        [TestMethod]
        public async Task GetUserSession_StoredP9UserSessionBasedOnName_ReturnsP9UserSession()
        {
            // Arrange
            var json = CreateSessionJsonWithTypeName("P9UserSession");

            ArrangeNoEncryption();
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.FindFirstDocument(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().Should().BeOfType<P9UserSession>("P9UserSession should be deserialised");
        }

        [TestMethod]
        public async Task GetAndUpdateUserSession_StoredP9UserSessionBasedOnName_ReturnsP9UserSession()
        {
            // Arrange
            var json = CreateSessionJsonWithTypeName("P9UserSession");

            ArrangeNoEncryption();
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetAndUpdateUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.FindOneAndUpdateDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().Should().BeOfType<P9UserSession>("P9UserSession should be deserialised");
        }

        [TestMethod]
        public async Task GetUserSession_StoredP9UserSessionBasedOnFullName_ReturnsP9UserSession()
        {
            // Arrange
            var json = CreateSessionJsonWithTypeName("NHSOnline.Backend.Support.Session.P9UserSession");

            ArrangeNoEncryption();
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.FindFirstDocument(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().Should().BeOfType<P9UserSession>("P9UserSession should be deserialised");
        }

        [TestMethod]
        public async Task GetAndUpdateUserSession_StoredP9UserSessionBasedOnFullName_ReturnsP9UserSession()
        {
            // Arrange
            var json = CreateSessionJsonWithTypeName("NHSOnline.Backend.Support.Session.P9UserSession");

            ArrangeNoEncryption();
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetAndUpdateUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.FindOneAndUpdateDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().Should().BeOfType<P9UserSession>("P9UserSession should be deserialised");
        }

        [TestMethod]
        public async Task GetUserSession_StoredP5UserSessionBasedOnName_ReturnsP5UserSession()
        {
            // Arrange
            var json = CreateSessionJsonWithTypeName("P5UserSession");

            ArrangeNoEncryption();
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.FindFirstDocument(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().Should().BeOfType<P5UserSession>("P5UserSession should be deserialised");
        }

        [TestMethod]
        public async Task GetAndUpdateUserSession_StoredP5UserSessionBasedOnName_ReturnsP5UserSession()
        {
            // Arrange
            var json = CreateSessionJsonWithTypeName("P5UserSession");

            ArrangeNoEncryption();
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetAndUpdateUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.FindOneAndUpdateDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().Should().BeOfType<P5UserSession>("P5UserSession should be deserialised");
        }

        [TestMethod]
        public async Task GetUserSession_StoredP5UserSessionBasedOnFullName_ReturnsP5UserSession()
        {
            // Arrange
            var json = CreateSessionJsonWithTypeName("NHSOnline.Backend.Support.Session.P5UserSession");

            ArrangeNoEncryption();
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.FindFirstDocument(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().Should().BeOfType<P5UserSession>("P5UserSession should be deserialised");
        }

        [TestMethod]
        public async Task GetAndUpdateUserSession_StoredP5UserSessionBasedOnFullName_ReturnsP5UserSession()
        {
            // Arrange
            var json = CreateSessionJsonWithTypeName("NHSOnline.Backend.Support.Session.P5UserSession");

            ArrangeNoEncryption();
            ArrangeSessionData(json);

            // Act
            var session = await CreateMongoSessionCacheService().GetAndUpdateUserSession("");

            // Assert
            _mockMongoClientService.Verify(x => x.FindOneAndUpdateDocumentAsync(
                It.IsAny<IRepositoryConfiguration>(),
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>()));

            _mockMongoClientService.VerifyNoOtherCalls();

            session.HasValue.Should().BeTrue("session should be returned");
            session.ValueOrFailure().Should().BeOfType<P5UserSession>("P5UserSession should be deserialised");
        }

        private MongoSessionCacheService CreateMongoSessionCacheService()
        {
            var serialiserService = new UserSessionSerialiserService();
            var encryptionService = new UserSessionEncryptionService(serialiserService, _mockCipherService.Object);
            var mongoSessionCache = new MongoSessionCacheAccessor(
                new Mock<ILogger<MongoSessionCacheAccessor>>().Object, _mockMongoClientService.Object, _mockConfig.Object);
            return new MongoSessionCacheService(_mockLogger.Object, encryptionService, mongoSessionCache);
        }

        private static string CreateSessionJsonWithTypeName(string typeName)
        {
            string json =
                "{" +
                $"\"$type\":\"{typeName}, NHSOnline.Backend.Support\"," +
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
            return json;
        }

        private async Task<string> CreateSessionAndCaptureJson(UserSession userSession)
        {
            BsonDocument update = null;

            _mockMongoClientService
                .Setup(x => x.InsertOneDocumentAsync(
                    It.IsAny<IRepositoryConfiguration>(),
                    It.IsAny<BsonDocument>()))
                .Callback<IRepositoryConfiguration, BsonDocument>((config, doc) => update = doc);

            var sut = CreateMongoSessionCacheService();
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
            var document = new BsonDocument(new Dictionary<string, object> { { "session", json } });

            _mockMongoClientService
                .Setup(x => x.FindOneAndUpdateDocumentAsync(
                    It.IsAny<IRepositoryConfiguration>(),
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>()))
                .ReturnsAsync(document);

            _mockMongoClientService
                .Setup(x => x.FindFirstDocument(
                    It.IsAny<IRepositoryConfiguration>(),
                    It.IsAny<FilterDefinition<BsonDocument>>()))
                .ReturnsAsync(document);
        }
    }
}
