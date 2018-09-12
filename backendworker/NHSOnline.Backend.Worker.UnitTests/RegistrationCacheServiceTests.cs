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
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support.Cipher;
using NHSOnline.Backend.Worker.Support.Hasher;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker.UnitTests
{
    [TestClass]
    public class RegistrationCacheServiceTests
    {
        private IFixture _fixture;
        private Mock<IOptions<ConfigurationSettings>> _settings;
        private int _defaultSessionExpiryMinutes;
        private Mock<IDatabase> _database;
        private Mock<IConnectionMultiplexerFactory> _connectionMultiplexerFactory;
        private ILogger<RegistrationCacheService> _logger;
        private Mock<IHashingService> _hashingService; 
        
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
            _hashingService = new Mock<IHashingService>();

            _database = new Mock<IDatabase>();
            var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
            connectionMultiplexer.Setup(x => x.GetDatabase(-1, null)).Returns(_database.Object);
            
            _connectionMultiplexerFactory = new Mock<IConnectionMultiplexerFactory>();
            _connectionMultiplexerFactory.Setup(x => x.GetMultiplexer(ConnectionMultiplexerName.Session))
                .Returns(connectionMultiplexer.Object);

            _logger = new LoggerFactory().CreateLogger<RegistrationCacheService>();
        }

        [TestMethod]
        public async Task CreateRegistrationGuid_GuidIsStoredInRedis()
        {
            // Arrange
            var passedValue = _fixture.Create<Guid>();
            var passedKey = _fixture.Create<string>();
            RedisValue registrationValueJson = JsonConvert.SerializeObject(passedValue);
            
            string registrationGuidKey = null;
            string redisValue = null;

            const string encryptedKey = "encrypted_string";
            const string prefix = "AccessGuid:";
            
            _hashingService.Setup(
                x => x.Hash(passedKey)).Returns(encryptedKey);
            
            _database
                .Setup(x =>
                    x.StringSetAsync(
                        prefix + encryptedKey,
                        registrationValueJson,
                        TimeSpan.FromMinutes(_defaultSessionExpiryMinutes),
                        When.Always,
                        CommandFlags.None))
                .Returns(Task.FromResult(true))
                .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((key, value, expiry, when, flags) =>
                {
                    registrationGuidKey = key;  // Store the guid key that was used as Redis session key
                    redisValue = value;
                })
                .Verifiable();
                
                
            var systemUnderTest = new RegistrationCacheService(_connectionMultiplexerFactory.Object, _settings.Object, _logger, _hashingService.Object);
            
            // Act
            var result = await systemUnderTest.CreateRegistrationGuid(passedKey, passedValue);
            
            // Assert
            _database.Verify();
            result.Should().Be(prefix + encryptedKey);
            redisValue.Should().NotBeNullOrEmpty();
            redisValue.Should().NotBeSameAs(registrationValueJson);
        }
        
        [TestMethod]
        public async Task DeleteRegistrationGuid_SessionIsDeletedFromRedis()
        {
            // Arrange            
            string redisKey = "test";
            
            const string encryptedKey = "encrypted_string";
            const string prefix = "AccessGuid:";
            
            _hashingService.Setup(
                x => x.Hash(redisKey)).Returns(encryptedKey);
                        
            _database
                .Setup(x =>
                    x.KeyDeleteAsync(
                        prefix + encryptedKey,
                        CommandFlags.None))
                .Returns(Task.FromResult(true))
                .Verifiable();
            
            var systemUnderTest = new RegistrationCacheService(_connectionMultiplexerFactory.Object, _settings.Object, _logger, _hashingService.Object);
            
            // Act
            var result = await systemUnderTest.DeleteRegistrationGuid(redisKey);
            
            // Assert
            _database.Verify();
            result.Should().Be(true);
        }
    }
}