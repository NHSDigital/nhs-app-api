using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Linkage
{
    [TestClass]
    public class VisionLinkageValidationServiceTests
    {
        private VisionLinkageValidationService _systemUnderTest;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<VisionLinkageValidationService>();
        }

        [TestMethod]
        public void IsGetValid_ReturnsTrueForCompleteRequest()
        {
            var request = MakeGetLinkageRequest();

            var result = _systemUnderTest.IsGetValid(request);

            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsGetValid_ReturnsTrueForNoIdentityToken(string identityToken)
        {
            var request = MakeGetLinkageRequest();
            request.IdentityToken = identityToken;

            var result = _systemUnderTest.IsGetValid(request);

            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsGetValid_ReturnsFalseForNoNhsNumber(string nhsNumber)
        {
            var request = MakeGetLinkageRequest();
            request.NhsNumber = nhsNumber;

            var result = _systemUnderTest.IsGetValid(request);

            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsGetValid_ReturnsFalseForOdsCode(string odsCode)
        {
            var request = MakeGetLinkageRequest();
            request.OdsCode = odsCode;

            var result = _systemUnderTest.IsGetValid(request);

            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsGetValid_ReturnsFalseForSurname(string surname)
        {
            var request = MakeGetLinkageRequest();
            request.Surname = surname;

            var result = _systemUnderTest.IsGetValid(request);

            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        public void IsGetValid_ReturnsFalseNoDateOfBirth(DateTime? dateOfBirth)
        {
            var request = MakeGetLinkageRequest();
            request.DateOfBirth = dateOfBirth;

            var result = _systemUnderTest.IsGetValid(request);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsPostValid_ReturnsTrueForCompleteRequest()
        {
            var request = MakeCreateLinkageRequest();

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsPostValid_ReturnsTrueForNoEmailAddress(string emailAddress)
        {
            var request = MakeCreateLinkageRequest();
            request.EmailAddress = emailAddress;

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsPostValid_ReturnsTrueForNoIdentityToken(string identityToken)
        {
            var request = MakeCreateLinkageRequest();
            request.IdentityToken = identityToken;

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null)]
        public void IsPostValid_ReturnsFalseForNoDateOfBirth(DateTime? dateOfBirth)
        {
            var request = MakeCreateLinkageRequest();
            request.DateOfBirth = dateOfBirth;

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsPostValid_ReturnsFalseForNoSurname(string surname)
        {
            var request = MakeCreateLinkageRequest();
            request.Surname = surname;

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsPostValid_ReturnsFalseForNoNhsNumber(string nhsNumber)
        {
            var request = MakeCreateLinkageRequest();
            request.NhsNumber = nhsNumber;

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsPostValid_ReturnsFalseForNoOdsCode(string odsCode)
        {
            var request = MakeCreateLinkageRequest();
            request.OdsCode = odsCode;

            var result = _systemUnderTest.IsPostValid(request);

            result.Should().BeFalse();
        }

        private GetLinkageRequest MakeGetLinkageRequest()
        {
            var request = _fixture.Create<GetLinkageRequest>();

            request.OdsCode = "A1B2C3";

            return request;
        }

        private CreateLinkageRequest MakeCreateLinkageRequest()
        {
            var request = _fixture.Create<CreateLinkageRequest>();

            request.OdsCode = "A1B2C3";

            return request;
        }
    }
}