using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support.Cipher;
using NHSOnline.Backend.Worker.Support.Session;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker.UnitTests.Support.Session
{
    [TestClass]
    public class SessionCacheServiceTests
    {
        private IFixture _fixture;
        private Mock<IOptions<ConfigurationSettings>> _settings;
        private Mock<ICipherService> _cipherService;
        private Mock<IDatabase> _database;
        private Mock<IConnectionMultiplexerFactory> _connectionMultiplexerFactory;
        private int _defaultSessionExpiryMinutes;
        private ILogger<RedisSessionCacheService> _logger;
        
        [TestInitialize]
        public void TestInitializeInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _defaultSessionExpiryMinutes = _fixture.Create<int>();
            _settings = new Mock<IOptions<ConfigurationSettings>>();
            _settings.Setup(x => x.Value).Returns(
                new ConfigurationSettings
                {
                    DefaultSessionExpiryMinutes = _defaultSessionExpiryMinutes
                });
            _cipherService = new Mock<ICipherService>();

            _database = new Mock<IDatabase>();
            var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
            connectionMultiplexer.Setup(x => x.GetDatabase(-1, null)).Returns(_database.Object);
            
            _connectionMultiplexerFactory = new Mock<IConnectionMultiplexerFactory>();
            _connectionMultiplexerFactory.Setup(x => x.GetMultiplexer(ConnectionMultiplexerName.Session))
                .Returns(connectionMultiplexer.Object);

            _logger = new LoggerFactory().CreateLogger<RedisSessionCacheService>();
        }
        
        [TestMethod]
        public async Task CreateUserSession_UserSessionIsStoredInRedis()
        {
            // Arrange
            var userSession = _fixture.Create<EmisUserSession>();
            RedisValue userSessionJson = JsonConvert.SerializeObject(userSession);
            
            string redisSessionKey = null;
            string redisValue = null;
            
            _cipherService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns("encrypted_string");
            
            _database
                .Setup(x =>
                    x.StringSetAsync(
                        It.IsAny<RedisKey>(),
                        _cipherService.Object.Encrypt(userSessionJson),
                        TimeSpan.FromMinutes(_defaultSessionExpiryMinutes),
                        When.Always,
                        CommandFlags.None))
                .Returns(Task.FromResult(true))
                .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((key, value, expiry, when, flags) =>
                {
                    redisSessionKey = key;  // Store the guid key that was used as Redis session key
                    redisValue = value;
                })
                .Verifiable();
                
                
            var systemUnderTest = new RedisSessionCacheService(_connectionMultiplexerFactory.Object, _cipherService.Object, _settings.Object, _logger);
            
            // Act
            var result = await systemUnderTest.CreateUserSession(userSession);
            
            // Assert
            _database.Verify();
            result.Should().Be(redisSessionKey);
            redisValue.Should().NotBeNullOrEmpty();
            redisValue.Should().NotBeSameAs(userSessionJson);
        }


        [TestMethod]
        public async Task DeleteUserSession_SessionIsDeletedFromRedis()
        {
            // Arrange            
            string redisSessionKey = "test";
                        
            _database
                .Setup(x =>
                    x.KeyDeleteAsync(
                            redisSessionKey,
                            CommandFlags.None))
                .Returns(Task.FromResult(true))
                .Verifiable();
            
            var systemUnderTest = new RedisSessionCacheService(_connectionMultiplexerFactory.Object, _cipherService.Object, _settings.Object, _logger);
            
            // Act
            var result = await systemUnderTest.DeleteUserSession(redisSessionKey);
            
            // Assert
            _database.Verify();
            result.Should().Be(true);
        }

        [TestMethod]
        public async Task UpdateUserSession_UserSessionIsStoredInRedis()
        {
            // Arrange
            var userSession = _fixture.Create<EmisUserSession>();
            RedisValue userSessionJson = JsonConvert.SerializeObject(userSession);
            const string encryptedOutput = "encrypted_string";

            string redisSessionKey = null;
            string redisValue = null;

            _cipherService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns(encryptedOutput);

            _database
                .Setup(x =>
                    x.StringSetAsync(
                        userSession.Key,
                        "encrypted_string",
                        TimeSpan.FromMinutes(_defaultSessionExpiryMinutes),
                        When.Always,
                        CommandFlags.None))
                .Returns(Task.FromResult(true))
                .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((key, value, expiry, when, flags) =>
                {
                    redisSessionKey = key;  // Store the guid key that was used as Redis session key
                    redisValue = value;
                })
                .Verifiable();

            var systemUnderTest = new RedisSessionCacheService(_connectionMultiplexerFactory.Object, _cipherService.Object, _settings.Object, _logger);

            // Act
            await systemUnderTest.UpdateUserSession(userSession);

            // Assert
            _database.Verify();
            redisValue.Should().NotBeNullOrEmpty();
            redisValue.Should().Be(encryptedOutput);
        }
    }
}
