using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.DataProtection;
using NHSOnline.Backend.Worker.Session;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker.UnitTests.Session
{
    [TestClass]
    public class SessionCacheServiceTests
    {
        private static IFixture _fixture;
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }
        
        [TestMethod]
        public async Task CreateUserSession_UserSessionIsStoredInRedis()
        {
            // Arrange
            var userSession = _fixture.Create<UserSession>();
            RedisValue userSessionJson = JsonConvert.SerializeObject(userSession);
            
            var sessionExpiryMinutes = _fixture.Create<int>();
            var configuration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configuration.SetupGet(r => r["SESSION_EXPIRY_MINUTES"]).Returns(sessionExpiryMinutes.ToString);
            
            string redisSessionKey = null;
            string redisValue = null;
            
            var cipherService = new Mock<ICipherService>();
            cipherService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns("encrypted_string");
            
            var database = new Mock<IDatabase>();
            database
                .Setup(x =>
                    x.StringSetAsync(
                        It.IsAny<RedisKey>(),
                        cipherService.Object.Encrypt(userSessionJson),
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
                
            var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
            connectionMultiplexer.Setup(x => x.GetDatabase(-1, null)).Returns(database.Object);
            
            var connectionMultiplexerFactory = new Mock<IConnectionMultiplexerFactory>();
            connectionMultiplexerFactory.Setup(x => x.GetMultiplexer(ConnectionMultiplexerName.Session))
                .Returns(connectionMultiplexer.Object);
                
            var systemUnderTest = new SessionCacheService(connectionMultiplexerFactory.Object, cipherService.Object, configuration.Object);
            
            // Act
            var result = await systemUnderTest.CreateUserSession(userSession);
            
            // Assert
            database.Verify();
            result.Should().Be(redisSessionKey);
            redisValue.Should().NotBeNullOrEmpty();
            redisValue.Should().NotBeSameAs(userSessionJson);
        }
    }
}
