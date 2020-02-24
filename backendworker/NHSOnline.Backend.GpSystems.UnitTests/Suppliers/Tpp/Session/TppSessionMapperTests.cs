using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Session
{
    [TestClass]
    public class TppSessionMapperTests
    {
        private IFixture _fixture;
        private TppSessionMapper _systemUnderTest;
        private string _odsCode;
        private string _nhsNumber;
        private string _suid;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<TppSessionMapper>();

            _odsCode = _fixture.Create<string>();
            _nhsNumber = _fixture.Create<string>();
            _suid = _fixture.Create<string>();
        }

        [TestMethod]
        public void Map_WhenValidResponseObjectInput_MapsCorrectlyAndReturnsOptionTppUserSession()
        {
            // Arrange
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();

            // realistic response has:
            // - online user id and patient id equal
            // - the main user in the person collection
            response.Body.OnlineUserId = response.Body.User.Person.PatientId;
            response.Body.People.Add(response.Body.User.Person);
            
            response.Headers.Add("suid", _suid);

            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            result.HasValue.Should().BeTrue();

            var value = result.ValueOrFailure();

            value.Name.Should().Be(response.Body.User.Person.PersonName.Name);
            value.Suid.Should().Be(_suid);
            value.OnlineUserId.Should().Be(response.Body.OnlineUserId);
            value.PatientId.Should().Be(response.Body.User.Person.PatientId);
            value.OdsCode.Should().Be(_odsCode);
            value.NhsNumber.Should().Be(_nhsNumber);
            value.ProxyPatients.Count.Should().Be(response.Body.People.Count - 1); // 1 less

            var proxies = value.ProxyPatients;

            foreach (var proxy in proxies)
            {
                var associatedPerson = response.Body.People.First(x => string.Equals(x.PatientId, proxy.PatientId, StringComparison.Ordinal));
                proxy.Id.Should().NotBeEmpty();
                proxy.Name.Should().Be(associatedPerson.PersonName.Name);
                proxy.NhsNumber.Should().Be(associatedPerson.NationalId.Value);
                proxy.PatientId.Should().Be(associatedPerson.PatientId);
                proxy.DateOfBirth.Should().Be(associatedPerson.DateOfBirth);
                proxy.Suid.Should().BeNull();
            }
        }
        
        [TestMethod]
        public void Map_WhenNoProxyUsers_MapsCorrectlyAndReturnsOptionTppUserSession()
        {
            // Arrange
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();

            // realistic response has:
            // - online user id and patient id equal
            // - the main user in the person collection
            response.Body.OnlineUserId = response.Body.User.Person.PatientId;
            response.Body.People = new List<Person> { response.Body.User.Person };
            
            response.Headers.Add("suid", _suid);

            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            result.HasValue.Should().BeTrue();

            var value = result.ValueOrFailure();

            value.Name.Should().Be(response.Body.User.Person.PersonName.Name);
            value.Suid.Should().Be(_suid);
            value.OnlineUserId.Should().Be(response.Body.OnlineUserId);
            value.PatientId.Should().Be(response.Body.User.Person.PatientId);
            value.OdsCode.Should().Be(_odsCode);
            value.NhsNumber.Should().Be(_nhsNumber);
            value.ProxyPatients.Count.Should().Be(0);
        }
        
        [TestMethod]
        public void Map_WhenResponseObjectIsNull_ReturnsOptionNoResult()
        {
            // Arrange
            TppApiObjectResponse<AuthenticateReply> response = null;

            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void Map_WhenResponseObjectInputHasNoHeader_ReturnsOptionNoResult()
        {
            // Arrange
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();

            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }
        
        [TestMethod]
        public void Map_WhenResponseObjectHasNullBody_ReturnsOptionNoResult()
        {
            // Arrange
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();
            response.Headers.Add("suid", _suid);
            response.Body = null;
            
            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }
        
        [DataRow(null)]
        [DataRow("")]
        [DataTestMethod]
        public void Map_WhenResponseObjectBodyIsMissingPatientId_ReturnsOptionNoResult(string patientId)
        {
            // Arrange
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();
            response.Headers.Add("suid", _suid);
            response.Body.User.Person.PatientId = patientId;

            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }
        
        [DataRow(null)]
        [DataRow("")]
        [DataTestMethod]
        public void Map_WhenResponseObjectBodyIsMissingOnlineUserId_ReturnsOptionNoResult(string onlineUserId)
        {
            // Arrange
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();
            response.Headers.Add("suid", _suid);
            response.Body.OnlineUserId = onlineUserId;

            // Act
            var result = _systemUnderTest.Map(response, _odsCode, _nhsNumber);
            
            // Assert
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }
    }
}