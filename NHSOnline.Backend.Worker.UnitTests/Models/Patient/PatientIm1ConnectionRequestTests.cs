using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using AutoFixture;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Models.Patient;

namespace NHSOnline.Backend.Worker.UnitTests.Models.Patient
{
    [TestClass]
    public class PatientIm1ConnectionRequestTests
    {
        private static Fixture _fixture;
        private PatientIm1ConnectionRequest _sut;
        private ValidationContext _context;
        private List<ValidationResult> _validationResults;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _fixture = new AutoFixture.Fixture();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _sut = _fixture.Create<PatientIm1ConnectionRequest>();
            _context = new ValidationContext(_sut, null, null);
            _validationResults = new List<ValidationResult>();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void PatientIm1ConnectionValidation_AccountId_Required(string odsCode)
        {
            // Arrange
            _sut.AccountId = odsCode;

            // Act
            var valid = Validator.TryValidateObject(_sut, _context, _validationResults, true);

            // Assert
            valid.Should().BeFalse();
            _validationResults.Count.Should().Be(1);
            _validationResults.Single().MemberNames.Should().BeEquivalentTo("AccountId");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void PatientIm1ConnectionValidation_LinkageKey_Required(string linkageKey)
        {
            // Arrange
            _sut.LinkageKey = linkageKey;

            // Act
            var valid = Validator.TryValidateObject(_sut, _context, _validationResults, true);

            // Assert
            valid.Should().BeFalse();
            _validationResults.Count.Should().Be(1);
            _validationResults.Single().MemberNames.Should().BeEquivalentTo("LinkageKey");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void PatientIm1ConnectionValidation_OdsCode_Required(string odsCode)
        {
            // Arrange
            _sut.OdsCode = odsCode;

            // Act
            var valid = Validator.TryValidateObject(_sut, _context, _validationResults, true);

            // Assert
            valid.Should().BeFalse();
            _validationResults.Count.Should().Be(1);
            _validationResults.Single().MemberNames.Should().BeEquivalentTo("OdsCode");
        }

        [DataTestMethod]
        [DataRow("ABCDEFGHJKH")]  // more than 8 characters
        //[DataRow("")]
        //[DataRow(" ")]
        public void PatientIm1ConnectionValidation_OdsCode_InvalidFormats(string odsCode)
        {
            // Arrange
            _sut.OdsCode = "ABCDEFGHIJ";

            // Act
            var valid = Validator.TryValidateObject(_sut, _context, _validationResults, true);

            // Assert
            valid.Should().BeFalse();
            _validationResults.Count.Should().Be(1);
            _validationResults.Single().MemberNames.Should().BeEquivalentTo("OdsCode");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void PatientIm1ConnectionValidation_Surname_Required(string surname)
        {
            // Arrange
            _sut.Surname = surname;

            // Act
            var valid = Validator.TryValidateObject(_sut, _context, _validationResults, true);

            // Assert
            valid.Should().BeFalse();
            _validationResults.Count.Should().Be(1);
            _validationResults.Single().MemberNames.Should().BeEquivalentTo("Surname");
        }
    }
}
