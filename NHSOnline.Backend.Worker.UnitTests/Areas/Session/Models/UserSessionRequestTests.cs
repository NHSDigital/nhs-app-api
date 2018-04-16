using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Session.Models;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Session.Models
{
    [TestClass]
    public class UserSessionRequestTests
    {
        private static Fixture _fixture;
        private UserSessionRequest _systemUnderTest;
        private ValidationContext _context;
        private List<ValidationResult> _validationResults;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _fixture = new Fixture();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = _fixture.Create<UserSessionRequest>();
            _context = new ValidationContext(_systemUnderTest, null, null);
            _validationResults = new List<ValidationResult>();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void SessionValidation_AuthCode_Required(string authCode)
        {
            // Arrange
            _systemUnderTest.AuthCode = authCode;

            // Act
            var valid = Validator.TryValidateObject(_systemUnderTest, _context, _validationResults, true);

            // Assert
            valid.Should().BeFalse();
            _validationResults.Count.Should().Be(1);
            _validationResults.Single().MemberNames.Should().BeEquivalentTo("AuthCode");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void SessionValidation_CodeVerifier_Required(string authCode)
        {
            // Arrange
            _systemUnderTest.CodeVerifier = authCode;

            // Act
            var valid = Validator.TryValidateObject(_systemUnderTest, _context, _validationResults, true);

            // Assert
            valid.Should().BeFalse();
            _validationResults.Count.Should().Be(1);
            _validationResults.Single().MemberNames.Should().BeEquivalentTo("CodeVerifier");
        }
    }
}