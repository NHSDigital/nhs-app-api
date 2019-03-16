using System;
using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.PfsApi.OrganDonation;

namespace NHSOnline.Backend.PfsApi.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationValidationServiceTests
    {
        private OrganDonationValidationService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = fixture.Create<OrganDonationValidationService>();
        }

        [TestMethod]
        public void IsPostValid_WhenRequestValid_ReturnsTrue()
        {
            var isValid = _systemUnderTest.IsPostValid(ValidOrganDonationRegistrationRequest());

            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void IsPostValid_WhenRequestNull_ReturnsFalse()
        {
            Action act = () => _systemUnderTest.IsPostValid(null);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(7)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("request", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPostValid_WhenRequestMissingAdditionalDetails_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.AdditionalDetails = null;

            Action act = () => _systemUnderTest.IsPostValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("AdditionalDetails", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPostValid_WhenRequestMissingRegistration_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration = null;

            Action act = () => _systemUnderTest.IsPostValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(5)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("Registration", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPostValid_WhenRequestMissingAddressFull_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration.AddressFull = null;

            Action act = () => _systemUnderTest.IsPostValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("AddressFull", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPostValid_WhenRequestMissingADateOfBirth_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration.DateOfBirth = null;

            Action act = () => _systemUnderTest.IsPostValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("DateOfBirth", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPostValid_WhenRequestMissingANhsNumber_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration.NhsNumber = null;

            Action act = () => _systemUnderTest.IsPostValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("NhsNumber", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPostValid_WhenRequestMissingAName_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration.Name = null;

            Action act = () => _systemUnderTest.IsPostValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("Name", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPostValid_WhenRequestIsOptInAndMissingFaithDeclaration_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration.FaithDeclaration = null;
            request.Registration.Decision = Decision.OptIn;

            Action act = () => _systemUnderTest.IsPostValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("FaithDeclaration", StringComparison.Ordinal));
        }


        [TestMethod]
        public void IsPutValid_WhenRequestValid_ReturnsTrue()
        {
            var isValid = _systemUnderTest.IsPutValid(ValidOrganDonationRegistrationRequest());

            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void IsPutValid_WhenRequestNull_ReturnsFalse()
        {
            Action act = () => _systemUnderTest.IsPutValid(null);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(7)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("request", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPutValid_WhenRequestMissingAdditionalDetails_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.AdditionalDetails = null;

            Action act = () => _systemUnderTest.IsPutValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("AdditionalDetails", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPutValid_WhenRequestMissingRegistration_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration = null;

            Action act = () => _systemUnderTest.IsPutValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(5)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("Registration", StringComparison.Ordinal));
        }

        [TestMethod]

        public void IsPutValid_WhenRequestMissingIdentifier_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration.Identifier = null;

            Action act = () => _systemUnderTest.IsPutValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("Identifier", StringComparison.Ordinal));
        }
        
        [TestMethod]
        public void IsPutValid_WhenRequestMissingAddressFull_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration.AddressFull = null;

            Action act = () => _systemUnderTest.IsPutValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("AddressFull", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPutValid_WhenRequestMissingADateOfBirth_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration.DateOfBirth = null;

            Action act = () => _systemUnderTest.IsPutValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("DateOfBirth", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPutValid_WhenRequestMissingANhsNumber_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration.NhsNumber = null;

            Action act = () => _systemUnderTest.IsPutValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("NhsNumber", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPutValid_WhenRequestMissingAName_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration.Name = null;

            Action act = () => _systemUnderTest.IsPutValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("Name", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsPutValid_WhenRequestIsOptInAndMissingFaithDeclaration_ReturnsFalse()
        {
            var request = ValidOrganDonationRegistrationRequest();
            request.Registration.FaithDeclaration = null;
            request.Registration.Decision = Decision.OptIn;

            Action act = () => _systemUnderTest.IsPutValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("FaithDeclaration", StringComparison.Ordinal));
        }

        private OrganDonationRegistrationRequest ValidOrganDonationRegistrationRequest()
        {
            return new OrganDonationRegistrationRequest
            {
                AdditionalDetails = new AdditionalDetails(),
                Registration = new OrganDonationStoreRegistration
                {
                    AddressFull = "Bridgewater Place, LS11 5BZ",
                    Address = new Address { Text = "Bridgewater Place", PostCode = "LS11 5BZ" },
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Identifier = "Test",
                    Name = new Name { GivenName = "Test", Surname = "User" },
                    NhsNumber = "123 123 1234"
                }
            };
        }


        [TestMethod]
        public void IsDeleteValid_WhenRequestValid_ReturnsTrue()
        {
            var isValid = _systemUnderTest.IsDeleteValid(ValidOrganDonationWithdrawRequest());

            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void IsDeleteValid_WhenRequestNull_ReturnsFalse()
        {
            Action act = () => _systemUnderTest.IsDeleteValid(null);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(7)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("request", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsDeleteValid_WhenRequestMissingIdentifier_ReturnsFalse()
        {
            var request = ValidOrganDonationWithdrawRequest();
            request.Identifier = null;

            Action act = () => _systemUnderTest.IsDeleteValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("Identifier", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsDeleteValid_WhenRequestMissingAddressFull_ReturnsFalse()
        {
            var request = ValidOrganDonationWithdrawRequest();
            request.AddressFull = null;

            Action act = () => _systemUnderTest.IsDeleteValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("AddressFull", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsDeleteValid_WhenRequestMissingADateOfBirth_ReturnsFalse()
        {
            var request = ValidOrganDonationWithdrawRequest();
            request.DateOfBirth = null;

            Action act = () => _systemUnderTest.IsDeleteValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("DateOfBirth", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsDeleteValid_WhenRequestMissingANhsNumber_ReturnsFalse()
        {
            var request = ValidOrganDonationWithdrawRequest();
            request.NhsNumber = null;

            Action act = () => _systemUnderTest.IsDeleteValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("NhsNumber", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsDeleteValid_WhenRequestMissingAName_ReturnsFalse()
        {
            var request = ValidOrganDonationWithdrawRequest();
            request.Name = null;

            Action act = () => _systemUnderTest.IsDeleteValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("Name", StringComparison.Ordinal));
        }

        [TestMethod]
        public void IsDeleteValid_WhenRequestMissingAReasonId_ReturnsFalse()
        {
            var request = ValidOrganDonationWithdrawRequest();
            request.WithdrawReasonId = null;

            Action act = () => _systemUnderTest.IsDeleteValid(request);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("WithdrawReasonId", StringComparison.Ordinal));
        }

        private OrganDonationWithdrawRequest ValidOrganDonationWithdrawRequest()
        {
            return new OrganDonationWithdrawRequest
            {
                AddressFull = "Bridgewater Place, LS11 5BZ",
                Address = new Address { Text = "Bridgewater Place", PostCode = "LS11 5BZ" },
                DateOfBirth = new DateTime(1990, 1, 1),
                Identifier = "Test",
                Name = new Name { GivenName = "Test", Surname = "User" },
                NhsNumber = "123 123 1234",
                WithdrawReasonId = "ReasonId",
            };
        }
    }
}