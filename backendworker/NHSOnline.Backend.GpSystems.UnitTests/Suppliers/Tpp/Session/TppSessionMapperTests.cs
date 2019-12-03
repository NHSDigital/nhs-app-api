using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.GpSystems.Suppliers.Tpp.TppClient;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Session
{
    [TestClass]
    public class TppSessionMapperTests
    {
        private IFixture _fixture;
        private TppSessionMapper _systemUnderTest;
        private string _odsCode;
        private string _nhsNumber;
        private string _suid;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<TppSessionMapper>();

            _odsCode = _fixture.Create<string>();
            _nhsNumber = _fixture.Create<string>();
            _suid = _fixture.Create<string>();
        }

        [TestMethod]
        public void Map_WhenValidResponseObjectInput_ReturnsOptionTppUserSession()
        {
            // Arrange
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();
            response.Headers.Add("suid", _suid);

            var expectedResponse = 
                new TppUserSession
                {
                    Name = response.Body.User.Person.PersonName.Name,
                    Suid = _suid,
                    OnlineUserId = response.Body.OnlineUserId,
                    PatientId = response.Body.User.Person.PatientId,
                    OdsCode = _odsCode,
                    NhsNumber = _nhsNumber
                };
            
            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            result.ValueOrFailure().Should().BeEquivalentTo(expectedResponse);
            result.HasValue.Should().BeTrue();
        }
        
        [TestMethod]
        public void Map_WhenResponseObjectIsNull_ReturnsOptionNoResult()
        {
            // Arrange
            TppApiObjectResponse<AuthenticateReply> response = null;

            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void Map_WhenResponseObjectInputHasNoHeader_ReturnsOptionNoResult()
        {
            // Arrange
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();

            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }
        
        [TestMethod]
        public void Map_WhenResponseObjectHasNullBody_ReturnsOptionNoResult()
        {
            // Arrange
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();
            response.Headers.Add("suid", _suid);
            response.Body = null;
            
            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }
        
        [DataRow(null)]
        [DataRow("")]
        [DataTestMethod]
        public void Map_WhenResponseObjectBodyIsMissingPatientId_ReturnsOptionNoResult(string patientId)
        {
            // Arrange
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();
            response.Headers.Add("suid", _suid);
            response.Body.User.Person.PatientId = patientId;

            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }
        
        [DataRow(null)]
        [DataRow("")]
        [DataTestMethod]
        public void Map_WhenResponseObjectBodyIsMissingOnlineUserId_ReturnsOptionNoResult(string onlineUserId)
        {
            // Arrange
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();
            response.Headers.Add("suid", _suid);
            response.Body.OnlineUserId = onlineUserId;

            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }
    }
}