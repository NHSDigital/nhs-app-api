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
            var emisHttpRequesData = EmisLinkedAccountModelExtensions.BuildHttpRequestData(_linkedAccountModel, _logger.Object);

            // Assert
            Assert.AreEqual(emisHttpRequesData.UserPatientLinkToken, _emisUserSession.UserPatientLinkToken);
            Assert.AreEqual(emisHttpRequesData.HeaderParameters.SessionId, _emisUserSession.SessionId);
            Assert.AreEqual(emisHttpRequesData.HeaderParameters.EndUserSessionId, _emisUserSession.EndUserSessionId);
        }  
        
        [TestMethod]
        public void BuildEmisHeaders_IdMatchedAgainstProxyPatient()
        {
            // Arrange
            EmisProxyUserSession user = ((IList<EmisProxyUserSession>)_emisUserSession.ProxyPatients).ElementAt(0);
            _linkedAccountModel.PatientId = user.Id;
            
            // Act
            var emisHeaders = EmisLinkedAccountModelExtensions.BuildHttpRequestData(_linkedAccountModel, _logger.Object);

            // Assert
            Assert.AreEqual(emisHeaders.UserPatientLinkToken, user.UserPatientLinkToken);
            Assert.AreEqual(emisHeaders.HeaderParameters.SessionId, _emisUserSession.SessionId);
            Assert.AreEqual(emisHeaders.HeaderParameters.EndUserSessionId, _emisUserSession.EndUserSessionId);
        }     
        
        [TestMethod]
        public void BuildEmisHeaders_IdNotFound_UsesDefaultUserPatientLinkToken()
        {
            // Arrange
            _linkedAccountModel.PatientId = Guid.Empty;
            
            // Act
            var emisHeaders = EmisLinkedAccountModelExtensions.BuildHttpRequestData(_linkedAccountModel, _logger.Object);
            
            // Assert
            Assert.AreEqual(emisHeaders.UserPatientLinkToken, _emisUserSession.UserPatientLinkToken);
            Assert.AreEqual(emisHeaders.HeaderParameters.SessionId, _emisUserSession.SessionId);
            Assert.AreEqual(emisHeaders.HeaderParameters.EndUserSessionId, _emisUserSession.EndUserSessionId);
        }        
    }
}