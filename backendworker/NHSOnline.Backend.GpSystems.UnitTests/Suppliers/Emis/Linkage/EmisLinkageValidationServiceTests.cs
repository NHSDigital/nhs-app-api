using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Linkage
{
    [TestClass]
    public class EmisLinkageValidationServiceTests
    {
        private EmisLinkageValidationService _systemUnderTest;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<EmisLinkageValidationService>();
        }

        [TestMethod]
        public void ValidateGetLinkageRequest_ForCompleteRequest_ReturnsTrue()
        {
            var request = ValidGetLinkageRequest();

            var result = _systemUnderTest.IsGetValid(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateGetLinkageRequest_ForNoIdentityToken_ReturnsFalse(string identityToken)
        {
            var request = ValidGetLinkageRequest();
            request.IdentityToken = identityToken;

            var result = _systemUnderTest.IsGetValid(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateGetLinkageRequest_ForNoOdsCode_ReturnsFalse(string nhsNumber)
        {
            var request = ValidGetLinkageRequest();
            request.NhsNumber = nhsNumber;

            var result = _systemUnderTest.IsGetValid(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateGetLinkageRequest_ForOdsCode_ReturnsFalse(string odsCode)
        {
            var request = ValidGetLinkageRequest();
            request.OdsCode = odsCode;

            var result = _systemUnderTest.IsGetValid(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateGetLinkageRequest_ForNoSurname_ReturnsTrue(string surname)
        {
            var request = ValidGetLinkageRequest();
            request.Surname = surname;

            var result = _systemUnderTest.IsGetValid(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        public void ValidateGetLinkageRequest_NoDateOfBirth_ReturnsTrue(DateTime? dateOfBirth)
        {
            var request = ValidGetLinkageRequest();
            request.DateOfBirth = dateOfBirth;

            var result = _systemUnderTest.IsGetValid(request);

            result.Should().BeTrue();
        }
        
        [TestMethod]
        public void ValidateCreateLinkageRequest_ForCompleteRequest_ReturnsTrue()
        {
            var request = ValidCreateLinkageRequest();

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ForNoEmailAddress_ReturnsFalse(string emailAddress)
        {
            var request = ValidCreateLinkageRequest();
            request.EmailAddress = emailAddress;

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ForNoIdentityToken_ReturnsFalse(string identityToken)
        {
            var request = ValidCreateLinkageRequest();
            request.IdentityToken = identityToken;

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        public void ValidateCreateLinkageRequest_ForNoDateOfBirth_ReturnsFalse(DateTime? dateOfBirth)
        {
            var request = ValidCreateLinkageRequest();
            request.DateOfBirth = dateOfBirth;

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ForNoSurname_ReturnsTrue(string surname)
        {
            var request = ValidCreateLinkageRequest();
            request.Surname = surname;

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeTrue();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ForNoNhsNumber_ReturnsFalse(string nhsNumber)
        {
            var request = ValidCreateLinkageRequest();
            request.NhsNumber = nhsNumber;

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ValidateCreateLinkageRequest_ForNoOdsCode_ReturnsFalse(string odsCode)
        {
            var request = ValidCreateLinkageRequest();
            request.OdsCode = odsCode;

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeFalse();
        }

        private CreateLinkageRequest ValidCreateLinkageRequest()
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.OdsCode = "A12345";
            return request;
        }

        private GetLinkageRequest ValidGetLinkageRequest()
        {
            var request = _fixture.Create<GetLinkageRequest>();
            request.OdsCode = "A12345";
            return request;
        }
    }
}