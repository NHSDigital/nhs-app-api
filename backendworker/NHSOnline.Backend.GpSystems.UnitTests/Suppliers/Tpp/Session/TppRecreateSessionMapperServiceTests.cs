using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Session
{
    [TestClass]
    public class TppRecreateSessionMapperServiceTests
    {
        private IFixture _fixture;
        private TppRecreateSessionMapperService _systemUnderTest;

        private TppUserSession _tppUserSession;
        private string _suid;
        private string _patientId;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<TppRecreateSessionMapperService>();

            _tppUserSession = _fixture.Create<TppUserSession>();
            _suid = _fixture.Create<string>();
            _patientId = _fixture.Create<string>();
        }

        [TestMethod]
        public void Map_SetsMainPatientSuid_WhenPatientIdFoundInMainPatient()
        {
            //Arrange
            _tppUserSession.PatientId = _patientId;
            var gpUserSession = (GpUserSession) _tppUserSession;

            //Act
            var result = _systemUnderTest.Map(gpUserSession, _suid, _patientId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(TppUserSession));
            var tppUserSession = (TppUserSession) result;
            Assert.AreEqual(tppUserSession.Suid, _suid);

            foreach (var proxy in tppUserSession.ProxyPatients)
            {
                Assert.IsNull(proxy.Suid);
            }
        }

        [TestMethod]
        public void Map_SetsProxyPatientSuid_WhenPatientIdFoundInProxyPatient()
        {
            //Arrange
            _tppUserSession.ProxyPatients.ToList()[1].PatientId = _patientId;
            var gpUserSession = (GpUserSession) _tppUserSession;

            //Act
            var result = _systemUnderTest.Map(gpUserSession, _suid, _patientId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(TppUserSession));
            var tppUserSession = (TppUserSession) result;
            Assert.IsNull(tppUserSession.Suid);

            foreach (var proxy in tppUserSession.ProxyPatients)
            {
                if (proxy.PatientId.Equals(_patientId, StringComparison.Ordinal))
                {
                    Assert.AreEqual(proxy.Suid, _suid);
                }
                else
                {
                    Assert.IsNull(proxy.Suid);
                }
            }
        }

        [TestMethod]
        public void Map_PatientIdNotInTppUserSession_NoSuidsAreAltered()
        {
            //Arrange
            var patientId = "unknown";
            var gpUserSession = (GpUserSession) _tppUserSession;

            List<string> suids = _tppUserSession.ProxyPatients.ToList().Select(pp => pp.Suid).ToList();
            suids.Add(_tppUserSession.Suid);

            //Act
            var result = _systemUnderTest.Map(gpUserSession, _suid, patientId);

            //Assert
            var tppUserSession = (TppUserSession) result;
            Assert.IsInstanceOfType(result, typeof(TppUserSession));

            Assert.IsNotNull(_tppUserSession.Suid);
            Assert.AreNotEqual(_suid, _tppUserSession.Suid);
            Assert.IsTrue(suids.Contains(_tppUserSession.Suid));

            foreach (var proxy in tppUserSession.ProxyPatients)
            {
                Assert.IsNotNull(proxy.Suid);
                Assert.AreNotEqual(_suid, proxy.Suid);
                Assert.IsTrue(suids.Contains(proxy.Suid));
            }
        }
    }
}