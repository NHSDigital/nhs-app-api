using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis
{
    [TestClass]
    public class EmisConnectionTokenParserTests
    {
        private IFixture _fixture;

        [TestInitialize]
        public void Initialise()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void Parse_ConnectionTokenIsAGuid_ReturnsEitherMatchingString()
        {
            //Arrange
            var connectionToken = _fixture.Create<Guid>().ToString();

            //Act
            var result = EmisConnectionTokenParser.Parse(connectionToken);

            //Assert
            result.Match(s => true, ct => false).Should().BeTrue();
        }

        [TestMethod]
        public void Parse_ConnectionTokenIsAConnectionToken_ReturnsEitherMatchingConnectionToken()
        {
            //Arrange
            var connectionToken = _fixture.Create<EmisConnectionToken>().SerializeJson();

            //Act
            var result = EmisConnectionTokenParser.Parse(connectionToken);

            //Assert
            result.Match(s => false, ct => true).Should().BeTrue();
        }
    }
}
