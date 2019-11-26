using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis
{
    [TestClass]
    public class EmisLinkedAccountModelExtensionTests
    {
        private EmisUserSession _emisUserSession;
        private GpLinkedAccountModel _linkedAccountModel;
        private IFixture _fixture;
        private static Mock<ILogger> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger =  _fixture.Freeze<Mock<ILogger>>();

            _emisUserSession = _fixture.Freeze<EmisUserSession>();
            _linkedAccountModel = new GpLinkedAccountModel(_emisUserSession);
        }
        
        [TestMethod]
        public void BuildEmisHeaders_IdMatchedAgainstMainUser()
        {
            // Act
            var emisRequestParameters = EmisLinkedAccountModelExtensions.BuildEmisRequestParameters(_linkedAccountModel, _logger.Object);

            // Assert
            Assert.AreEqual(emisRequestParameters.UserPatientLinkToken, _emisUserSession.UserPatientLinkToken);
            Assert.AreEqual(emisRequestParameters.SessionId, _emisUserSession.SessionId);
            Assert.AreEqual(emisRequestParameters.EndUserSessionId, _emisUserSession.EndUserSessionId);
        }  
        
        [TestMethod]
        public void BuildEmisHeaders_IdMatchedAgainstProxyPatient()
        {
            // Arrange
            EmisProxyUserSession user = ((IList<EmisProxyUserSession>)_emisUserSession.ProxyPatients).ElementAt(0);
            _linkedAccountModel.PatientId = user.Id;
            
            // Act
            var emisRequestParameters = EmisLinkedAccountModelExtensions.BuildEmisRequestParameters(_linkedAccountModel, _logger.Object);

            // Assert
            Assert.AreEqual(emisRequestParameters.UserPatientLinkToken, user.UserPatientLinkToken);
            Assert.AreEqual(emisRequestParameters.SessionId, _emisUserSession.SessionId);
            Assert.AreEqual(emisRequestParameters.EndUserSessionId, _emisUserSession.EndUserSessionId);
        }

        [TestMethod, ExpectedException(typeof(InvalidPatientIdException))]
        public void BuildEmisHeaders_IdNotFound_ThrowsInvalidPatientIdException()
        {
            // Arrange
            _linkedAccountModel.PatientId = Guid.Empty;

            // Act
            var emisRequestParameters = EmisLinkedAccountModelExtensions.BuildEmisRequestParameters(_linkedAccountModel, _logger.Object);
        }
    }
}