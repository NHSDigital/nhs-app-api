using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker.UnitTests
{
    [TestClass]
    public class OdsCodeLookupTests
    {
        private ILogger<OdsCodeLookup> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = Mock.Of<ILogger<OdsCodeLookup>>();
        }

        [TestMethod]
        public void Constructor_NullConnectionMultiplexerFactory_Throws()
        {
            Action act = () => new OdsCodeLookup(null, _logger);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("connectionMultiplexerFactory");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public async Task LookupSupplier_NullOrEmptyOdsCode_ReturnsOptionNone(string odsCode)
        {
            var connectionMultiplexerFactory = new Mock<IConnectionMultiplexerFactory>();
            var sut = new OdsCodeLookup(connectionMultiplexerFactory.Object, _logger);
            var result = await sut.LookupSupplier(odsCode);

            result.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public async Task LookupSupplier_RedisCacheReturnsNull_ReturnsOptionNone()
        {
            const string odsCode = "ABC123";
            RedisValue redisValue = default(RedisValue);

            var database = new Mock<IDatabase>();
            database.Setup(x => x.StringGetAsync(odsCode, CommandFlags.None)).Returns(Task.FromResult(redisValue));

            var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
            connectionMultiplexer.Setup(x => x.GetDatabase(-1, null)).Returns(database.Object);

            var connectionMultiplexerFactory = new Mock<IConnectionMultiplexerFactory>();
            connectionMultiplexerFactory.Setup(x => x.GetMultiplexer(ConnectionMultiplexerName.OdsCodeLookup))
                .Returns(connectionMultiplexer.Object);

            var sut = new OdsCodeLookup(connectionMultiplexerFactory.Object, _logger);

            var result = await sut.LookupSupplier(odsCode);
            result.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public async Task LookupSupplier_RedisCacheReturnsUnknownSupplier_Throws()
        {
            const string odsCode = "ABC123";
            RedisValue redisValue = "UnknownSupplier";

            var database = new Mock<IDatabase>();
            database.Setup(x => x.StringGetAsync(odsCode, CommandFlags.None)).Returns(Task.FromResult(redisValue));

            var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
            connectionMultiplexer.Setup(x => x.GetDatabase(-1, null)).Returns(database.Object);

            var connectionMultiplexerFactory = new Mock<IConnectionMultiplexerFactory>();
            connectionMultiplexerFactory.Setup(x => x.GetMultiplexer(ConnectionMultiplexerName.OdsCodeLookup))
                .Returns(connectionMultiplexer.Object);

            var sut = new OdsCodeLookup(connectionMultiplexerFactory.Object, _logger);

            var result = await sut.LookupSupplier(odsCode);
            result.HasValue.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(Supplier.Emis)]
        [DataRow(Supplier.Tpp)]
        public async Task LookupSupplier_RedisCacheReturnsValidSupplier_ReturnsValue(Supplier supplier)
        {
            const string odsCode = "ABC123";
            RedisValue redisValue = supplier.ToString();

            var database = new Mock<IDatabase>();
            database.Setup(x => x.StringGetAsync(odsCode, CommandFlags.None)).Returns(Task.FromResult(redisValue));

            var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
            connectionMultiplexer.Setup(x => x.GetDatabase(-1, null)).Returns(database.Object);

            var connectionMultiplexerFactory = new Mock<IConnectionMultiplexerFactory>();
            connectionMultiplexerFactory.Setup(x => x.GetMultiplexer(ConnectionMultiplexerName.OdsCodeLookup))
                .Returns(connectionMultiplexer.Object);

            var sut = new OdsCodeLookup(connectionMultiplexerFactory.Object, _logger);
            var actual = await sut.LookupSupplier(odsCode);

            actual.HasValue.Should().BeTrue();
            actual.ValueOrFailure().Should().Be(supplier);
        }

        [DataTestMethod]
        [DataRow("EmIs", Supplier.Emis)]
        [DataRow("tPp", Supplier.Tpp)]
        public async Task LookupSupplier_RedisCacheReturnsValidSupplierButWithUnusualCasing_ReturnsEnum(string redisString,
            Supplier supplier)
        {
            const string odsCode = "ABC123";
            RedisValue redisValue = redisString;

            var database = new Mock<IDatabase>();
            database.Setup(x => x.StringGetAsync(odsCode, CommandFlags.None)).Returns(Task.FromResult(redisValue));

            var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
            connectionMultiplexer.Setup(x => x.GetDatabase(-1, null)).Returns(database.Object);

            var connectionMultiplexerFactory = new Mock<IConnectionMultiplexerFactory>();
            connectionMultiplexerFactory.Setup(x => x.GetMultiplexer(ConnectionMultiplexerName.OdsCodeLookup))
                .Returns(connectionMultiplexer.Object);

            var sut = new OdsCodeLookup(connectionMultiplexerFactory.Object, _logger);

            var actual = await sut.LookupSupplier(odsCode);
            actual.HasValue.Should().BeTrue();
            actual.ValueOrFailure().Should().Be(supplier);
        }
    }
}
