using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Linkage;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Linkage
{
    [TestClass]
    public class TppLinkageRequestValidationServiceTests
    {
        private TppLinkageRequestValidationService _systemUnderTest;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<TppLinkageRequestValidationService>();
        }

        [TestMethod]
        public void ValidateGetLinkageRequest_ReturnsTrueForCompleteRequest()
        {
            var request = _fixture.Create<GetLinkageRequest>();

            var result = _systemUnderTest.Validate(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateGetLinkageRequest_ReturnsTrueForNoIdentityToken(string identityToken)
        {
            var request = _fixture.Create<GetLinkageRequest>();
            request.IdentityToken = identityToken;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateGetLinkageRequest_ReturnsFalseForNoNhsNumber(string nhsNumber)
        {
            var request = _fixture.Create<GetLinkageRequest>();
            request.NhsNumber = nhsNumber;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateGetLinkageRequest_ReturnsFalseForOdsCode(string odsCode)
        {
            var request = _fixture.Create<GetLinkageRequest>();
            request.OdsCode = odsCode;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateGetLinkageRequest_ReturnsFalseForSurname(string surname)
        {
            var request = _fixture.Create<GetLinkageRequest>();
            request.Surname = surname;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        public void ValidateGetLinkageRequest_ReturnsFalseNoDateOfBirth(DateTime? dateOfBirth)
        {
            var request = _fixture.Create<GetLinkageRequest>();
            request.DateOfBirth = dateOfBirth;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [TestMethod]
        public void ValidateCreateLinkageRequest_ReturnsTrueForCompleteRequest()
        {
            var request = _fixture.Create<CreateLinkageRequest>();

            var result = _systemUnderTest.Validate(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ReturnsTrueForNoEmailAddress(string emailAddress)
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.EmailAddress = emailAddress;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ReturnsTrueForNoIdentityToken(string identityToken)
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.IdentityToken = identityToken;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        public void ValidateCreateLinkageRequest_ReturnsFalseForNoDateOfBirth(DateTime? dateOfBirth)
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.DateOfBirth = dateOfBirth;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ReturnsFalseForNoSurname(string surname)
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.Surname = surname;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ReturnsFalseForNoNhsNumber(string nhsNumber)
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.NhsNumber = nhsNumber;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ReturnsFalseForNoOdsCode(string odsCode)
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.OdsCode = odsCode;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
    }
}