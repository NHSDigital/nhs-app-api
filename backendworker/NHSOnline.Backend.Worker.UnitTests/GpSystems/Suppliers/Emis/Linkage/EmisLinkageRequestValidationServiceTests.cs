using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Linkage
{
    [TestClass]
    public class EmisLinkageRequestValidationServiceTests
    {
        private EmisLinkageRequestValidationService _systemUnderTest;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<EmisLinkageRequestValidationService>();
        }

        [TestMethod]
        public void ValidateGetLinkageRequest_ForCompleteRequest_ReturnsTrue()
        {
            var request = _fixture.Create<GetLinkageRequest>();

            var result = _systemUnderTest.Validate(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateGetLinkageRequest_ForNoIdentityToken_ReturnsFalse(string identityToken)
        {
            var request = _fixture.Create<GetLinkageRequest>();
            request.IdentityToken = identityToken;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateGetLinkageRequest_ForNoOdsCode_ReturnsFalse(string nhsNumber)
        {
            var request = _fixture.Create<GetLinkageRequest>();
            request.NhsNumber = nhsNumber;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateGetLinkageRequest_ForOdsCode_ReturnsFalse(string odsCode)
        {
            var request = _fixture.Create<GetLinkageRequest>();
            request.OdsCode = odsCode;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateGetLinkageRequest_ForNoSurname_ReturnsTrue(string surname)
        {
            var request = _fixture.Create<GetLinkageRequest>();
            request.Surname = surname;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        public void ValidateGetLinkageRequest_NoDateOfBirth_ReturnsTrue(DateTime? dateOfBirth)
        {
            var request = _fixture.Create<GetLinkageRequest>();
            request.DateOfBirth = dateOfBirth;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeTrue();
        }
        
        [TestMethod]
        public void ValidateCreateLinkageRequest_ForCompleteRequest_ReturnsTrue()
        {
            var request = _fixture.Create<CreateLinkageRequest>();

            var result = _systemUnderTest.Validate(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ForNoEmailAddress_ReturnsFalse(string emailAddress)
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.EmailAddress = emailAddress;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ForNoIdentityToken_ReturnsFalse(string identityToken)
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.IdentityToken = identityToken;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        public void ValidateCreateLinkageRequest_ForNoDateOfBirth_ReturnsFalse(DateTime? dateOfBirth)
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.DateOfBirth = dateOfBirth;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ForNoSurname_ReturnsTrue(string surname)
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.Surname = surname;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ForNoNhsNumber_ReturnsFalse(string nhsNumber)
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.NhsNumber = nhsNumber;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ForNoOdsCode_ReturnsFalse(string odsCode)
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.OdsCode = odsCode;

            var result = _systemUnderTest.Validate(request);

            result.Should().BeFalse();
        }
    }
}