using System;
using System.Collections.Generic;
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
            _linkedAccountModel = new GpLinkedAccountModel(_tppUserSession);
        }

        [TestMethod]
        public void BuildTppRequestParameters_IdMatchedAgainstMainUser()
        {
            // Act
            var tppRequestParameters = TppLinkedAccountModelExtensions.BuildTppRequestParameters(_linkedAccountModel, _logger.Object);

            // Assert
            Assert.AreEqual(tppRequestParameters.PatientId, _tppUserSession.PatientId);
            Assert.AreEqual(tppRequestParameters.OdsCode, _tppUserSession.OdsCode);
            Assert.AreEqual(tppRequestParameters.OnlineUserId, _tppUserSession.OnlineUserId);
            Assert.AreEqual(tppRequestParameters.Suid, _tppUserSession.Suid);
        }

        [TestMethod]
        public void BuildTppRequestParameters_IdMatchedAgainstProxyPatient()
        {
            // Arrange
            TppProxyUserSession user = ((IList<TppProxyUserSession>)_tppUserSession.ProxyPatients).ElementAt(0);
            _linkedAccountModel.PatientId = user.Id;

            // Act
            var tppRequestParameters = TppLinkedAccountModelExtensions.BuildTppRequestParameters(_linkedAccountModel, _logger.Object);

            // Assert
            Assert.AreEqual(tppRequestParameters.PatientId, user.PatientId);
            Assert.AreEqual(tppRequestParameters.OdsCode, _tppUserSession.OdsCode);
            Assert.AreEqual(tppRequestParameters.OnlineUserId, _tppUserSession.OnlineUserId);
            Assert.AreEqual(tppRequestParameters.Suid, user.Suid);
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