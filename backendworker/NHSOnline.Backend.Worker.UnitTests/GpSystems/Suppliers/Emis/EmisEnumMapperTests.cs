using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using System;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis
{
    [TestClass]
    public class EmisEnumMapperTests
    {
        private IEmisEnumMapper _sut;
        private ILogger<EmisEnumMapper> _logger;
        private IFixture _fixture;

        [TestInitialize]
        public void Initialise()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Create<ILoggerFactory>().CreateLogger<EmisEnumMapper>();
            _sut = new EmisEnumMapper(_logger);
        }

        [DataTestMethod]
        [DataRow(null, Necessity.Mandatory)]
        [DataRow("", Necessity.Optional)]
        public void MapNecessity_KeyIsNullOrEmptyAndDefaultProvided_ReturnsDefault(string key, Necessity defaultNecessity)
        {
            //Act
            var result = _sut.MapNecessity(key, defaultNecessity);

            //Assert
            result.Should().Be(defaultNecessity);
        }

        [TestMethod]
        public void MapNecessity_KeyIsNotMapped_ReturnsDefault()
        {
            //Act
            var result = _sut.MapNecessity("this does not map", Necessity.Mandatory);

            //Assert
            result.Should().Be(Necessity.Mandatory);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [ExpectedException(typeof(ArgumentException))]
        public void MapNecessity_KeyIsNullOrEmptyAndDefaultNotProvided_ThrowsException(string key)
        {
            //Act
            _sut.MapNecessity(key, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MapNecessity_KeyIsNotMappedAndDefaultNotProvided_ThrowsException()
        {
            //Act
            _sut.MapNecessity("this does not match", null);
        }

        [DataTestMethod]
        [DataRow("NotRequested", Necessity.NotAllowed)]
        [DataRow("RequestedOptional", Necessity.Optional)]
        [DataRow("RequestedMandatory", Necessity.Mandatory)]
        public void MapNecessity_KeyIsMapped_ReturnsEnum(string key, Necessity expected)
        {
            //Act
            var result = _sut.MapNecessity(key, null);

            //Assert
            result.Should().Be(expected);
        }

        [DataTestMethod]
        [DataRow("notrequested", Necessity.NotAllowed)]
        [DataRow("RequestedOptional", Necessity.Optional)]
        [DataRow("requestedMandatory", Necessity.Mandatory)]
        public void MapNecessity_KeyIsMappedIgnoringCase_ReturnsEnum(string key, Necessity expected)
        {
            //Act
            var result = _sut.MapNecessity(key, null);

            //Assert
            result.Should().Be(expected);
        }
        
        [DataTestMethod]
        [DataRow("Unknown", Channel.Unknown)]
        [DataRow("Practice", Channel.Unknown)]
        [DataRow("Telephone", Channel.Telephone)]
        [DataRow("Visit", Channel.Unknown)]
        [DataRow("Video", Channel.Unknown)]
        public void MapSlotTypeStatus_KeyIsMapped_ReturnsEnum(string key, Channel expected)
        {
            //Act
            var result = _sut.MapSlotTypeStatus(key, null);

            //Assert
            result.Should().Be(expected);
        }

        [DataTestMethod]
        [DataRow("unknown", Channel.Unknown)]
        [DataRow("practice", Channel.Unknown)]
        [DataRow("telephone", Channel.Telephone)]
        [DataRow("visit", Channel.Unknown)]
        [DataRow("video", Channel.Unknown)]
        public void MapSlotTypeStatus_KeyIsMappedIgnoringCase_ReturnsEnum(string key, Channel expected)
        {
            //Act
            var result = _sut.MapSlotTypeStatus(key, null);

            //Assert
            result.Should().Be(expected);
        }        
    }
}
