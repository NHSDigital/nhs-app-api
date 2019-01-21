using System;
using AutoFixture;
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
        private PatientIm1ConnectionRequest _request;

        [TestInitialize]
        public void TestInitialize()
        {
            Mock<ILogger<Im1ConnectionValidator>> logger = new Mock<ILogger<Im1ConnectionValidator>>();
            _systemUnderTest = new Im1ConnectionValidator(logger.Object);

            IFixture fixture = new Fixture();
            _request = fixture.Freeze<PatientIm1ConnectionRequest>();
            _request.OdsCode = "A0B1C2";
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
            PatientIm1ConnectionRequest request = new PatientIm1ConnectionRequest()
            {
                AccountId = accountId,
                LinkageKey = linkageKey,
                DateOfBirth = DateTime.Now,
                OdsCode = odsCode,
                Surname = surname
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
            PatientIm1ConnectionRequest request = new PatientIm1ConnectionRequest()
            {
                AccountId = "1234567890",
                LinkageKey = "1234567890",
                DateOfBirth = DateTime.Now,
                OdsCode = "A1B2C3",
                Surname = "Smith"
            };

            //Act
            var result = _systemUnderTest.IsPostValid(request);

            //Assert
            result.Should().BeTrue();
        }

    }
}
