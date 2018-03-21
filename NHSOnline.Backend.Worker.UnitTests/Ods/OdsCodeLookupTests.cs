using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Ods;
using NHSOnline.Backend.Worker.Suppliers;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker.UnitTests.Ods
{
    [TestClass]
    public class OdsCodeLookupTests
    {
        [TestMethod]
        public void Constructor_NullConnectionMultiplexerFactory_Throws()
        {
            Action act = () => new OdsCodeLookup(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("connectionMultiplexerFactory");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void LookupSupplier_NullOrEmptyOdsCode_Throws(string odsCode)
        {
            var connectionMultiplexerFactory = new Mock<IConnectionMultiplexerFactory>();
            var sut = new OdsCodeLookup(connectionMultiplexerFactory.Object);

            Func<Task> act = async () => await sut.LookupSupplier(odsCode);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("odsCode");
        }

        [TestMethod]
        public void LookupSupplier_RedisCacheReturnsNull_Throws()
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

            var sut = new OdsCodeLookup(connectionMultiplexerFactory.Object);

            Func<Task> act = async () => await sut.LookupSupplier(odsCode);

            act.Should().Throw<OdsCodeLookupException>()
                .WithMessage($"ODS Code '{odsCode}' could not be found.")
                .And.OdsCode.Should().Be(odsCode);
        }

        [TestMethod]
        public void LookupSupplier_RedisCacheReturnsUnknownSupplier_Throws()
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

            var sut = new OdsCodeLookup(connectionMultiplexerFactory.Object);

            Func<Task> act = async () => await sut.LookupSupplier(odsCode);

            act.Should().Throw<OdsCodeLookupException>()
                .WithMessage($"ODS Code '{odsCode}' is associated with unexpected supplier '{redisValue}'.")
                .And.OdsCode.Should().Be(odsCode);
        }

        [DataTestMethod]
        [DataRow(SupplierEnum.Emis)]
        [DataRow(SupplierEnum.Tpp)]
        public void LookupSupplier_RedisCacheReturnsValidSupplier_ReturnsEnum(SupplierEnum supplier)
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

            var sut = new OdsCodeLookup(connectionMultiplexerFactory.Object);

            var actual = sut.LookupSupplier(odsCode).Result;

            actual.Should().Be(supplier);
        }

        [DataTestMethod]
        [DataRow("EmIs", SupplierEnum.Emis)]
        [DataRow("tPp", SupplierEnum.Tpp)]
        public void LookupSupplier_RedisCacheReturnsValidSupplierButWithUnusualCasing_ReturnsEnum(string redisString,
            SupplierEnum supplier)
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

            var sut = new OdsCodeLookup(connectionMultiplexerFactory.Object);

            var actual = sut.LookupSupplier(odsCode).Result;

            actual.Should().Be(supplier);
        }
    }
}