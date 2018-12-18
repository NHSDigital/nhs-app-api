using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Session;
using NHSOnline.Backend.Worker.Support;
using static NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.TppClient;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Session
{
    [TestClass]
    public class TppSessionMapperTests
    {
        private IFixture _fixture;
        private TppSessionMapper _systemUnderTest;
        private string _odsCode;
        private string _accessToken;
        private string _nhsNumber;
        private string _suid;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<TppSessionMapper>();

            _odsCode = _fixture.Create<string>();
            _accessToken = _fixture.Create<string>();
            _nhsNumber = _fixture.Create<string>();
            _suid = _fixture.Create<string>();
        }

        [TestMethod]
        public void Map_WhenValidResponseObjectInput_ReturnsOptionTppUserSession()
        {
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();
            response.Headers.Add("suid", _suid);

            var expectedResponse = 
                new TppUserSession()
                {
                    Suid = _suid,
                    OnlineUserId = response.Body.OnlineUserId,
                    PatientId = response.Body.User.Person.PatientId,
                    OdsCode = _odsCode,
                    AccessToken = _accessToken,
                    NhsNumber = _nhsNumber
                };

            var result = _systemUnderTest.Map(response, _odsCode, _accessToken, _nhsNumber);
            
            result.ValueOrFailure().Should().BeEquivalentTo(expectedResponse);
            result.HasValue.Should().BeTrue();
        }
        
        [TestMethod]
        public void Map_WhenResponseObjectIsNull_ReturnsOptionNoResult()
        {
            TppApiObjectResponse<AuthenticateReply> response = null;

            var result = _systemUnderTest.Map(response, _odsCode, _accessToken, _nhsNumber);
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void Map_WhenResponseObjectInputHasNoHeader_ReturnsOptionNoResult()
        {
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();

            var result = _systemUnderTest.Map(response, _odsCode, _accessToken, _nhsNumber);
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }
        
        [TestMethod]
        public void Map_WhenResponseObjectHasNullBody_ReturnsOptionNoResult()
        {
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();
            response.Headers.Add("suid", _suid);
            response.Body = null;

            var result = _systemUnderTest.Map(response, _odsCode, _accessToken, _nhsNumber);
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }
        
        [DataRow(null)]
        [DataRow("")]
        [DataTestMethod]
        public void Map_WhenResponseObjectBodyIsMissingPatientId_ReturnsOptionNoResult(string patientId)
        {
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();
            response.Headers.Add("suid", _suid);
            response.Body.User.Person.PatientId = patientId;

            var result = _systemUnderTest.Map(response, _odsCode, _accessToken, _nhsNumber);
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }
        
        [DataRow(null)]
        [DataRow("")]
        [DataTestMethod]
        public void Map_WhenResponseObjectBodyIsMissingOnlineUserId_ReturnsOptionNoResult(string onlineUserId)
        {
            var response = _fixture.Create<TppApiObjectResponse<AuthenticateReply>>();
            response.Headers.Add("suid", _suid);
            response.Body.OnlineUserId = onlineUserId;

            var result = _systemUnderTest.Map(response, _odsCode, _accessToken, _nhsNumber);
            Action act = () => result.ValueOrFailure();
            
            act.Should().Throw<OptionalValueMissingException>();
            result.HasValue.Should().BeFalse();
        }
        
    }
}