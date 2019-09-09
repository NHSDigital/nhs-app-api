using System;
using System.Collections.Generic;
using System.Linq;
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
            // Act
            var result = _systemUnderTest.IsGetValid(connectionToken, odsCode);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsGetValid_ValidData_ReturnsTrue()
        {
            // Arrange
            const string odsCode = "A1B2C3";
            const string connectionToken = "1234567890";

            // Act
            var result = _systemUnderTest.IsGetValid(connectionToken, odsCode);

            // Assert
            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow("", "", "", "", new []{"AccountId", "LinkageKey", "OdsCode", "Surname"})]
        [DataRow(null, "1234567890", "A1B2C3", "Smith", new[] { "AccountId" })]
        [DataRow("1234567890", null, "A1B2C3", "Smith", new[] {  "LinkageKey" })]
        [DataRow("1234567890", "1234567890", null, "Smith", new[] { "OdsCode" })]
        [DataRow("1234567890", "1234567890", "A1B2C3", null, new[] { "Surname" })]
        [DataRow("1234567890", "1234567890", "Fake Ods Code", "Smith", new[] { "OdsCode" })]
        public void IsPostValid_InvalidData_ReturnsFalse(string accountId, string linkageKey, string odsCode, string surname, IEnumerable<string> expectedInvalidParams)
        {
            // Arrange
            var request = new PatientIm1ConnectionRequest
            {
                AccountId = accountId,
                LinkageKey = linkageKey,
                OdsCode = odsCode,
                Surname = surname,
                DateOfBirth = DateTime.Now
            };

            // Act
            var result = _systemUnderTest.IsPostValid(request, out var invalidParams);

            // Assert
            result.Should().BeFalse();
            invalidParams.Should().BeEquivalentTo(expectedInvalidParams);
        }

        [TestMethod]
        public void IsPostValid_ValidData_ReturnsTrue()
        {
            // Arrange
            var request = new PatientIm1ConnectionRequest
            {
                AccountId = "1234567890",
                LinkageKey = "1234567890",
                OdsCode = "A1B2C3",
                Surname = "Smith",
                DateOfBirth = DateTime.Now
            };

            // Act
            var result = _systemUnderTest.IsPostValid(request, out var invalidParams);

            // Assert
            result.Should().BeTrue();
            invalidParams.Should().BeEquivalentTo(Enumerable.Empty<string>());
        }

        [DataTestMethod]
        [DataRow("", "", "", "", "", new[] { "OdsCode", "Surname", "NhsNumber", "EmailAddress", "IdentityToken" })]
        [DataRow(null, "Smith", "123ABC", "test@test.com", "IDToken", new[] { "OdsCode"})]
        [DataRow("A1B2C3", null, "123ABC", "test@test.com", "IDToken", new[] { "Surname" })]
        [DataRow("A1B2C3", "Smith", null, "test@test.com", "IDToken", new[] { "NhsNumber" })]
        [DataRow("A1B2C3", "Smith", "123ABC", null, "IDToken", new[] { "EmailAddress" })]
        [DataRow("A1B2C3", "Smith", "123ABC", "test@test.com", null, new[] { "IdentityToken" })]
        [DataRow("Fake Ods Code", "Smith", "123ABC", "test@test.com", "IDToken", new[] { "OdsCode" })]
        public void IsCreateLinkageRequestValid_InvalidData_ReturnsFalse(
            string odsCode, string surname, string nhsNumber, string emailAddress, string identityToken, IEnumerable<string> expectedInvalidParams)
        {
            // Arrange
            var request = new Im1RegistrationRequest
            {
                OdsCode = odsCode,
                Surname = surname,
                DateOfBirth = DateTime.Now,
                NhsNumber = nhsNumber,
                EmailAddress = emailAddress,
                IdentityToken = identityToken
            };

            // Act
            var result = _systemUnderTest.IsCreateLinkageRequestValid(request, out var invalidParams);

            // Assert
            result.Should().BeFalse();
            invalidParams.Should().BeEquivalentTo(expectedInvalidParams);
        }

        [TestMethod]
        public void IsCreateLinkageRequestValid_ValidData_ReturnsTrue()
        {
            // Arrange
            var request = new Im1RegistrationRequest
            {
                OdsCode = "A1B2C3",
                Surname = "Smith",
                DateOfBirth = DateTime.Now,
                NhsNumber = "123ABC",
                EmailAddress = "test@test.com",
                IdentityToken = "IDToken"
            };

            // Act
            var result = _systemUnderTest.IsCreateLinkageRequestValid(request, out var invalidParams);

            // Assert
            result.Should().BeTrue();
            invalidParams.Should().BeEquivalentTo(Enumerable.Empty<string>());
        }

        [DataTestMethod]
        [DataRow("", "", "", "", new[] { "AccountId", "LinkageKey", "OdsCode", "Surname" })]
        [DataRow(null, "1234", "A1B2C3", "Smith", new[] { "AccountId"})]
        [DataRow("AccountID", null, "A1B2C3", "Smith", new[] {  "LinkageKey"})]
        [DataRow("AccountID", "1234", null, "Smith", new[] { "OdsCode" })]
        [DataRow("AccountID", "1234", "A1B2C3", null, new[] {"Surname" })]
        [DataRow("AccountID", "1234", "Fake Ods Code", "Smith", new[] { "OdsCode"})]
        public void IsPatientIm1ConnectionRequestValid_InvalidData_ReturnsFalse(
            string accountId, string linkageKey, string odsCode, string surname, IEnumerable<string> expectedInvalidParams)
        {
            // Arrange
            var request = new Im1RegistrationRequest
            {
                AccountId = accountId,
                LinkageKey = linkageKey,
                OdsCode = odsCode,
                Surname = surname,
                DateOfBirth = DateTime.Now
            };

            // Act
            var result = _systemUnderTest.IsPatientIm1ConnectionRequestValid(request, out var invalidParams);

            // Assert
            result.Should().BeFalse();
            invalidParams.Should().BeEquivalentTo(expectedInvalidParams);
        }
        
        [TestMethod]
        public void IsPatientIm1ConnectionRequestValid_ValidData_ReturnsTrue()
        {
            // Arrange
            var request = new Im1RegistrationRequest
            {      
                AccountId = "AccountID",
                LinkageKey = "1234",
                OdsCode = "A1B2C3",
                Surname = "Smith",
                DateOfBirth = DateTime.Now
            };

            // Act
            var result = _systemUnderTest.IsPatientIm1ConnectionRequestValid(request, out var invalidParams);

            // Assert
            result.Should().BeTrue();
            invalidParams.Should().BeEquivalentTo(Enumerable.Empty<string>());
        }
    }
}
