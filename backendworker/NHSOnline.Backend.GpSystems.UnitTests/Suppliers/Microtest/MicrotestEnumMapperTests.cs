using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest
{
    [TestClass]
    public class MicrotestEnumMapperTests
    {
        private IMicrotestEnumMapper _systemUnderTest;
        private ILogger<MicrotestEnumMapper> _logger;
        private IFixture _fixture;

        [TestInitialize]
        public void Initialise()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILoggerFactory>().CreateLogger<MicrotestEnumMapper>();
            _systemUnderTest = new MicrotestEnumMapper(_logger);
        }

        [DataTestMethod]
        [DataRow("Other", Channel.Unknown)]
        [DataRow("Surgery", Channel.Unknown)]
        [DataRow("Telephone", Channel.Telephone)]
        [DataRow("Visit", Channel.Unknown)]
        public void MapChannel_ChannelIsMapped_ReturnsEnum(string channel, Channel expected)
        {
            //Act
            var result = _systemUnderTest.MapChannel(channel, null);

            //Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void MapChannel_ChannelIsNotMapped_ReturnsRequestedDefault()
        {
            //Act
            var result = _systemUnderTest.MapChannel("not mapped", Channel.Telephone);

            //Assert
            result.Should().Be(Channel.Telephone);
        }
    }
}
