using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using FluentAssertions;
using NHSOnline.Backend.Worker.Session;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionCacheServiceTests
    {
        private static IFixture _fixture;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
        }

        [TestMethod]
        public async Task CreateUserSession_ReturnsSessionGuidString()
        {
            var sessionKey = "1234-5678";
            var supplier = SupplierEnum.Emis;

            var userSession = new UserSession
            {
                SupplierSessionId = sessionKey,
                Supplier = supplier
            };

            RedisKey redisKey = "dsfdsf-dfsdf";
            RedisValue redisValue = JsonConvert.SerializeObject(userSession);

            var database = new Mock<IDatabase>();
            database.Setup(x => x.SetAddAsync(redisKey, redisValue, CommandFlags.None)).Returns(Task.FromResult(true));

            var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
            connectionMultiplexer.Setup(x => x.GetDatabase(-1, null)).Returns(database.Object);

            var connectionMultiplexerFactory = new Mock<IConnectionMultiplexerFactory>();
            connectionMultiplexerFactory.Setup(x => x.GetMultiplexer(ConnectionMultiplexerName.Session))
                .Returns(connectionMultiplexer.Object);

            var sut = _fixture.Create<SessionCacheService>();

            var result = await sut.CreateUserSession(userSession);

            result.Should().NotBeNullOrEmpty();
        }
    }
}
