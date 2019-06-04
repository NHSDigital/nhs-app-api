using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.CidApi.Areas.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;

namespace NHSOnline.Backend.CidApi.UnitTests.Areas.Im1Connection
{
    [TestClass]
    public class Im1ConnectionValidatorTests
    {

        private Im1ConnectionValidator _systemUnderTest;
        private PatientIm1ConnectionRequest _connectionRequest;
        private Im1RegistrationRequest _registrationRequest;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Freeze<Mock<ILogger<Im1ConnectionValidator>>>();
            _connectionRequest = fixture.Freeze<PatientIm1ConnectionRequest>();
            _connectionRequest.OdsCode = "A0B1C2";
            _registrationRequest = fixture.Freeze<Im1RegistrationRequest>();
            _registrationRequest.OdsCode = "A0B1C2";
            _systemUnderTest = fixture.Create<Im1ConnectionValidator>(); 
        }

        [DataTestMethod]
        [DataRow("", "")]
        [DataRow(null, "1234567890")]
        [DataRow("A1B2C3", null)]
        [DataRow("Fake Ods Code", "1234567890")]
        public void IsGetValid_InvalidData_ReturnsFalse(string odsCode, string connectionToken)
        {
            //Arrange

            //Act
            var result = _systemUnderTest.IsGetValid(connectionToken, odsCode);

            //Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsGetValid_ValidData_ReturnsTrue()
        {
            //Arrange
            string odsCode = "A1B2C3";
            string connectionToken = "1234567890";

            //Act
            var result = _systemUnderTest.IsGetValid(connectionToken, odsCode);

            //Assert
            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow("", "", "", "")]
        [DataRow(null, "1234567890", "A1B2C3", "Smith")]
        [DataRow("1234567890", null, "A1B2C3", "Smith")]
        [DataRow("1234567890", "1234567890", null, "Smith")]
        [DataRow("1234567890", "1234567890", "A1B2C3", null)]
        [DataRow("1234567890", "1234567890", "Fake Ods Code", "Smith")]
        public void IsPostValid_InvalidData_ReturnsFalse(string accountId, string linkageKey, string odsCode, string surname)
        {
            //Arrange
            PatientIm1ConnectionRequest request = new PatientIm1ConnectionRequest
            {
                AccountId = accountId,
                LinkageKey = linkageKey,
                OdsCode = odsCode,
                Surname = surname,
                DateOfBirth = DateTime.Now
            };

            //Act
            var result = _systemUnderTest.IsPostValid(request);

            //Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsPostValid_ValidData_ReturnsTrue()
        {
            //Arrange
            PatientIm1ConnectionRequest request = new PatientIm1ConnectionRequest
            {
                AccountId = "1234567890",
                LinkageKey = "1234567890",
                OdsCode = "A1B2C3",
                Surname = "Smith",
                DateOfBirth = DateTime.Now
            };

            //Act
            var result = _systemUnderTest.IsPostValid(request);

            //Assert
            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow("", "", "", "", "")]
        [DataRow(null, "Smith", "123ABC", "test@test.com", "IDToken")]
        [DataRow("A1B2C3", null, "123ABC", "test@test.com", "IDToken")]
        [DataRow("A1B2C3", "Smith", null, "test@test.com", "IDToken")]
        [DataRow("A1B2C3", "Smith", "123ABC", null, "IDToken")]
        [DataRow("A1B2C3", "Smith", "123ABC", "test@test.com", null)]
        [DataRow("Fake Ods Code", "Smith", "123ABC", "test@test.com", "IDToken")]
        public void IsCreateLinkageRequestValid_InvalidData_ReturnsFalse(
            string odsCode, string surname, string nhsNumber, string emailAddress, string identityToken)
        {
            //Arrange
            Im1RegistrationRequest request = new Im1RegistrationRequest
            {
                OdsCode = odsCode,
                Surname = surname,
                DateOfBirth = DateTime.Now,
                NhsNumber = nhsNumber,
                EmailAddress = emailAddress,
                IdentityToken = identityToken
            };

            //Act
            var result = _systemUnderTest.IsCreateLinkageRequestValid(request);

            //Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCreateLinkageRequestValid_ValidData_ReturnsTrue()
        {
            //Arrange
            Im1RegistrationRequest request = new Im1RegistrationRequest
            {
                OdsCode = "A1B2C3",
                Surname = "Smith",
                DateOfBirth = DateTime.Now,
                NhsNumber = "123ABC",
                EmailAddress = "test@test.com",
                IdentityToken = "IDToken"
            };

            //Act
            var result = _systemUnderTest.IsCreateLinkageRequestValid(request);

            //Assert
            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow("", "", "", "")]
        [DataRow(null, "1234", "A1B2C3", "Smith")]
        [DataRow("AccountID", null, "A1B2C3", "Smith")]
        [DataRow("AccountID", "1234", null, "Smith")]
        [DataRow("AccountID", "1234", "A1B2C3", null)]
        [DataRow("AccountID", "1234", "Fake Ods Code", "Smith")]
        public void IsPatientIm1ConnectionRequestValid_InvalidData_ReturnsFalse(
            string accountId, string linkageKey, string odsCode, string surname)
        {
            //Arrange
            Im1RegistrationRequest request = new Im1RegistrationRequest
            {
                AccountId = accountId,
                LinkageKey = linkageKey,
                OdsCode = odsCode,
                Surname = surname,
                DateOfBirth = DateTime.Now
            };

            //Act
            var result = _systemUnderTest.IsPatientIm1ConnectionRequestValid(request);

            //Assert
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public void IsPatientIm1ConnectionRequestValid_ValidData_ReturnsTrue()
        {
            //Arrange
            Im1RegistrationRequest request = new Im1RegistrationRequest
            {      
                AccountId = "AccountID",
                LinkageKey = "1234",
                OdsCode = "A1B2C3",
                Surname = "Smith",
                DateOfBirth = DateTime.Now
            };

            //Act
            var result = _systemUnderTest.IsPatientIm1ConnectionRequestValid(request);

            //Assert
            result.Should().BeTrue();
        }
    }
}
