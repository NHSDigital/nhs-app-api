using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    [TestClass]
    public class TppLinkedAccountModelExtensionTests
    {
        private TppUserSession _tppUserSession;
        private GpLinkedAccountModel _linkedAccountModel;
        private IFixture _fixture;
        private static Mock<ILogger> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger =  _fixture.Freeze<Mock<ILogger>>();

            _tppUserSession = _fixture.Freeze<TppUserSession>();

            // clear suid for each user so each test must define who is authenticated
            _tppUserSession.Suid = null;
            foreach (var tppProxyUserSession in _tppUserSession.ProxyPatients)
            {
                tppProxyUserSession.Suid = null;
            }
            _linkedAccountModel = new GpLinkedAccountModel(_tppUserSession);
        }

        [TestMethod]
        public void BuildTppRequestParameters_IdMatchedAgainstMainUser()
        {
            // Act
            _tppUserSession.Suid = _fixture.Create<string>();
            var tppRequestParameters = TppLinkedAccountModelExtensions.BuildTppRequestParameters(_linkedAccountModel, _logger.Object);

            // Assert
            Assert.AreEqual(tppRequestParameters.PatientId, _tppUserSession.PatientId);
            Assert.AreEqual(tppRequestParameters.OdsCode, _tppUserSession.OdsCode);
            Assert.AreEqual(tppRequestParameters.OnlineUserId, _tppUserSession.OnlineUserId);
            Assert.AreEqual(tppRequestParameters.Suid, _tppUserSession.Suid);
        }

        [TestMethod]
        public void BuildTppRequestParameters_IdMatchedAgainstMainUserButUserNotAuthenticated_ThrowsInvalidPatientIdException()
        {
            // Assert
            Assert.ThrowsException<InvalidPatientIdException>(() =>
                TppLinkedAccountModelExtensions.BuildTppRequestParameters(_linkedAccountModel, _logger.Object));
        }

        [TestMethod]
        public void BuildTppRequestParameters_IdMatchedAgainstProxyPatient()
        {
            // Arrange
            var proxyPatient = _tppUserSession.ProxyPatients.ElementAt(0);
            proxyPatient.Suid = _fixture.Create<string>();
            _linkedAccountModel.PatientId = proxyPatient.Id;

            // Act
            var tppRequestParameters = TppLinkedAccountModelExtensions.BuildTppRequestParameters(_linkedAccountModel, _logger.Object);

            // Assert
            Assert.AreEqual(tppRequestParameters.PatientId, proxyPatient.PatientId);
            Assert.AreEqual(tppRequestParameters.OdsCode, _tppUserSession.OdsCode);
            Assert.AreEqual(tppRequestParameters.OnlineUserId, _tppUserSession.OnlineUserId);
            Assert.AreEqual(tppRequestParameters.Suid, proxyPatient.Suid);
        }

        [TestMethod]
        public void BuildTppRequestParameters_IdMatchedAgainstProxyPatientButProxyPatientNotAuthenticated_ThrowsInvalidPatientIdException()
        {
            // Arrange
            var proxyPatient = _tppUserSession.ProxyPatients.ElementAt(0);
            _linkedAccountModel.PatientId = proxyPatient.Id;

            // Act
            Assert.ThrowsException<InvalidPatientIdException>(() =>
                TppLinkedAccountModelExtensions.BuildTppRequestParameters(_linkedAccountModel, _logger.Object));
        }

        [TestMethod, ExpectedException(typeof(InvalidPatientIdException))]
        public void BuildTppRequestParameters_IdNotFound_ThrowsInvalidPatientIdException()
        {
            // Arrange
            _linkedAccountModel.PatientId = Guid.Empty;

            // Act
            TppLinkedAccountModelExtensions.BuildTppRequestParameters(_linkedAccountModel, _logger.Object);
        }
    }
}