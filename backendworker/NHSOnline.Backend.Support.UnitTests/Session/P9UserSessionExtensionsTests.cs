using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Fake;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.Support.UnitTests.Session
{
    [TestClass]
    public class P9UserSessionExtensionsTests
    {
        [TestMethod]
        public void BuildGpLinkedAccountModel_ThrowsInvalidPatientIdException_WhenGuidNotFoundInPatientLookup()
        {
            // Arrange
            var p9UserSession = new P9UserSession("", "", new CitizenIdUserSession(), "");

            // Act
            Action act = () => p9UserSession.BuildGpLinkedAccountModel(Guid.Empty);

            // Assert
            act.Should().Throw<InvalidPatientIdException>();
        }

        [TestMethod]
        public void BuildGpLinkedAccountModel_ReturnsPatientGpIdentifier_WhenPatientIdIsFound()
        {
            // Arrange
            const string patientGpIdentifier = "patient-gp-system-identifier";
            var uniquePatientId = Guid.NewGuid();
            var gpUserSession = new FakeUserSession();
            var p9UserSession = new P9UserSession("", "", new CitizenIdUserSession(), "", gpUserSession);
            p9UserSession.PatientLookup.Add(uniquePatientId, patientGpIdentifier);

            // Act
            var result = p9UserSession.BuildGpLinkedAccountModel(uniquePatientId);

            // Assert
            result.Should().NotBeNull();
            result.RequestingPatientGpIdentifier.Should().Be(patientGpIdentifier);
            result.GpUserSession.Should().Be(gpUserSession);
        }

        [TestMethod]
        public void TryGetPatientGpIdentifierFromSessionIdentifier_ReturnsFalse_WhenGuidNotFoundInPatientLookup()
        {
            // Arrange
            var p9UserSession = new P9UserSession("", "", new CitizenIdUserSession(), "");

            // Act
            var result = p9UserSession.TryGetPatientGpIdentifierFromSessionIdentifier(
                Guid.Empty,
                out string value);

            // Assert
            result.Should().BeFalse();
            value.Should().BeEmpty();
        }

        [TestMethod]
        public void TryGetPatientGpIdentifierFromSessionIdentifier_ReturnsTrueAndSetsGpIdentifier_WhenPatientIdIsFound()
        {
            // Arrange
            const string patientGpIdentifier = "patient-gp-system-identifier";
            var uniquePatientId = Guid.NewGuid();
            var p9UserSession = new P9UserSession("", "", new CitizenIdUserSession(), "");
            p9UserSession.PatientLookup.Add(uniquePatientId, patientGpIdentifier);

            // Act
            var result = p9UserSession.TryGetPatientGpIdentifierFromSessionIdentifier(uniquePatientId, out string value);

            // Assert
            result.Should().BeTrue();
            value.Should().Be(patientGpIdentifier);
        }
    }
}
