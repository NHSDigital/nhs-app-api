using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Session;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker.UnitTests.Session
{
    [TestClass]
    public class SessionCacheServiceTests
    {
        private static IFixture _fixture;
        
        private static Mock<IConfiguration> _configuration;
        private static Mock<ICipherService> _cipherService;
        private static Mock<IDatabase> _database;
        private static Mock<IConnectionMultiplexerFactory> _connectionMultiplexerFactory;
        
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            _configuration = new Mock<IConfiguration>();            
            _cipherService = new Mock<ICipherService>();

            _database = new Mock<IDatabase>();
            var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
            connectionMultiplexer.Setup(x => x.GetDatabase(-1, null)).Returns(_database.Object);
            
            _connectionMultiplexerFactory = new Mock<IConnectionMultiplexerFactory>();
            _connectionMultiplexerFactory.Setup(x => x.GetMultiplexer(ConnectionMultiplexerName.Session))
                .Returns(connectionMultiplexer.Object);
        }
        
        [TestMethod]
        public async Task CreateUserSession_UserSessionIsStoredInRedis()
        {
            // Arrange
            var userSession = _fixture.Create<EmisUserSession>();
            RedisValue userSessionJson = JsonConvert.SerializeObject(userSession);
            
            var sessionExpiryMinutes = _fixture.Create<int>();
            _configuration.SetupGet(r => r["SESSION_EXPIRY_MINUTES"]).Returns(sessionExpiryMinutes.ToString);
            
            string redisSessionKey = null;
            string redisValue = null;
            
            _cipherService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns("encrypted_string");
            
            _database
                .Setup(x =>
                    x.StringSetAsync(
                        It.IsAny<RedisKey>(),
                        _cipherService.Object.Encrypt(userSessionJson),
                        TimeSpan.FromMinutes(sessionExpiryMinutes),
                        When.Always,
                        CommandFlags.None))
                .Returns(Task.FromResult(true))
                .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((key, value, expiry, when, flags) =>
                {
                    redisSessionKey = key;  // Store the guid key that was used as Redis session key
                    redisValue = value;
                })
                .Verifiable();
                
                
            var systemUnderTest = new SessionCacheService(_connectionMultiplexerFactory.Object, _cipherService.Object, _configuration.Object);
            
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
            
            var systemUnderTest = new SessionCacheService(_connectionMultiplexerFactory.Object, _cipherService.Object, _configuration.Object);
            
            // Act
            var result = await systemUnderTest.DeleteUserSession(redisSessionKey);
            
            // Assert
            _database.Verify();
            result.Should().Be(true);
        }
    }
}
