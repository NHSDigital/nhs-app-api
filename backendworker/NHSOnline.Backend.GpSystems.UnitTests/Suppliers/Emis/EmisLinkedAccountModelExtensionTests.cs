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
        private IFixture _fixture;
        private static Mock<ILogger> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger =  _fixture.Freeze<Mock<ILogger>>();

            _emisUserSession = _fixture.Freeze<EmisUserSession>();
        }

        [TestMethod]
        public void BuildEmisHeaders_IdMatchedAgainstMainUser()
        {
            // Arrange
            var linkedAccountModel = new GpLinkedAccountModel(_emisUserSession, _emisUserSession.PatientActivityContextGuid);

            // Act
            var emisRequestParameters = EmisLinkedAccountModelExtensions.BuildEmisRequestParameters(linkedAccountModel, _logger.Object);

            // Assert
            Assert.AreEqual(emisRequestParameters.UserPatientLinkToken, _emisUserSession.UserPatientLinkToken);
            Assert.AreEqual(emisRequestParameters.SessionId, _emisUserSession.SessionId);
            Assert.AreEqual(emisRequestParameters.EndUserSessionId, _emisUserSession.EndUserSessionId);
        }

        [TestMethod]
        public void BuildEmisHeaders_IdMatchedAgainstProxyPatient()
        {
            // Arrange
            var emisProxyUserSession = ((IList<EmisProxyUserSession>)_emisUserSession.ProxyPatients).ElementAt(0);
            var linkedAccountModel = new GpLinkedAccountModel(_emisUserSession, emisProxyUserSession.PatientActivityContextGuid);

            // Act
            var emisRequestParameters = EmisLinkedAccountModelExtensions.BuildEmisRequestParameters(linkedAccountModel, _logger.Object);

            // Assert
            Assert.AreEqual(emisRequestParameters.UserPatientLinkToken, emisProxyUserSession.UserPatientLinkToken);
            Assert.AreEqual(emisRequestParameters.SessionId, _emisUserSession.SessionId);
            Assert.AreEqual(emisRequestParameters.EndUserSessionId, _emisUserSession.EndUserSessionId);
        }

        [TestMethod, ExpectedException(typeof(InvalidPatientIdException))]
        public void BuildEmisHeaders_IdNotFound_ThrowsInvalidPatientIdException()
        {
            // Arrange
            var linkedAccountModel = new GpLinkedAccountModel(_emisUserSession, _fixture.Create<string>());

            // Act
            EmisLinkedAccountModelExtensions.BuildEmisRequestParameters(linkedAccountModel, _logger.Object);
        }
    }
}