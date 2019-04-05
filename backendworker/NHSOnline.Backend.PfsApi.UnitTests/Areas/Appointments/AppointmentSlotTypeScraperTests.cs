using System;
using System.Security.Policy;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.Areas.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Appointments
{
    [TestClass]
    public class AppointmentSlotTypeScraperTests
    {
        private Mock<ILogger<AppointmentSlotTypeScraper>> _mockLogger;
        private Mock<ICurrentDateTimeProvider> _mockDateTimeProvider;
        private IFixture _fixture;
        private AppointmentSlotTypeScraper _systemUnderTest;
        private UserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _userSession = _fixture.Create<UserSession>();

            _mockDateTimeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<AppointmentSlotTypeScraper>>>();
            
            _systemUnderTest = _fixture.Create<AppointmentSlotTypeScraper>();
        }

        [TestMethod]
        public void CaptureAppointmentSlotTypes_AppointmentSlotsResultUnsuccessful_NoInformationBuiltNorLogged()
        {
            // Arrange
            var systemUnderTest = _fixture.Create<Mock<AppointmentSlotTypeScraper>>();
            systemUnderTest.CallBase = true;
            var badResult = new AppointmentSlotsResult.SupplierSystemUnavailable();

            // Act
            systemUnderTest.Object.CaptureAppointmentSlotTypes(_userSession, badResult);

            // Assert
            systemUnderTest.VerifyAll();
            systemUnderTest.Verify(x => 
                    x.BuildSlotsInformation(
                        It.IsAny<UserSession>(), It.IsAny<AppointmentSlotsResult.SuccessfullyRetrieved>()), 
                Times.Never());
            systemUnderTest.Verify(x => 
                    x.LogInformation(
                        It.IsAny<AppointmentSlotTypeScraper.AppointmentSlotsInformation>()), 
                Times.Never());
        }
        
        [TestMethod]
        public void CaptureAppointmentSlotTypes_AppointmentSlotsResultHasNoSlots_NoInformationBuiltNorLogged()
        {
            // Arrange
            var systemUnderTest = _fixture.Create<Mock<AppointmentSlotTypeScraper>>();
            systemUnderTest.CallBase = true;
            var emptyResult = BuildSuccessfulResult();

            // Act
            systemUnderTest.Object.CaptureAppointmentSlotTypes(_userSession, emptyResult);

            // Assert
            systemUnderTest.VerifyAll();
            systemUnderTest.Verify(x => 
                    x.BuildSlotsInformation(
                        It.IsAny<UserSession>(), It.IsAny<AppointmentSlotsResult.SuccessfullyRetrieved>()), 
                Times.Never());
            systemUnderTest.Verify(x => 
                    x.LogInformation(
                        It.IsAny<AppointmentSlotTypeScraper.AppointmentSlotsInformation>()), 
                Times.Never());
        }
        
        [TestMethod]
        public void CaptureAppointmentSlotTypes_AppointmentSlotsInformationShouldNotBeLogged_NoInformationLogged()
        {
            // Arrange
            var systemUnderTest = _fixture.Create<Mock<AppointmentSlotTypeScraper>>();
            systemUnderTest.CallBase = true;
            var slot = new Slot { Type = "Slot Type 1", SessionName = "Session Name 1" };
            var result = BuildSuccessfulResult(slot);
            var slotsInformation = _fixture.Create<AppointmentSlotTypeScraper.AppointmentSlotsInformation>();

            systemUnderTest.Setup(x => x.BuildSlotsInformation(_userSession, result))
                .Returns(slotsInformation);
            systemUnderTest.Setup(x => x.ShouldBeLogged(slotsInformation))
                .Returns(false);

            // Act
            systemUnderTest.Object.CaptureAppointmentSlotTypes(_userSession, result);

            // Assert
            systemUnderTest.VerifyAll();
            systemUnderTest.Verify(x => 
                    x.LogInformation(
                        It.IsAny<AppointmentSlotTypeScraper.AppointmentSlotsInformation>()), 
                Times.Never());
        }
        
        [TestMethod]
        public void CaptureAppointmentSlotTypes_AppointmentSlotsInformationShouldBeLogged_InformationLogged()
        {
            // Arrange
            var systemUnderTest = _fixture.Create<Mock<AppointmentSlotTypeScraper>>();
            systemUnderTest.CallBase = true;
            var slot = new Slot { Type = "Slot Type 1", SessionName = "Session Name 1" };
            var result = BuildSuccessfulResult(slot);
            var slotsInformation = _fixture.Create<AppointmentSlotTypeScraper.AppointmentSlotsInformation>();

            systemUnderTest.Setup(x => x.BuildSlotsInformation(_userSession, result))
                .Returns(slotsInformation);
            systemUnderTest.Setup(x => x.ShouldBeLogged(slotsInformation))
                .Returns(true);

            // Act
            systemUnderTest.Object.CaptureAppointmentSlotTypes(_userSession, result);

            // Assert
            systemUnderTest.VerifyAll();
            systemUnderTest.Verify(x => 
                    x.LogInformation(
                        It.IsAny<AppointmentSlotTypeScraper.AppointmentSlotsInformation>()), 
                Times.Once);
        }
        
        [TestMethod]
        public void BuildSlotsInformation_OneSlot_SlotTypesIncludesOneSlotType()
        {
            // Arrange
            var slot = new Slot { Type = "Slot Type 1", SessionName = "Session Name 1" };
            var result = BuildSuccessfulResult(slot);

            // Act
            var information = _systemUnderTest.BuildSlotsInformation(_userSession, result);

            // Assert
            information.SlotTypes.Should().BeEquivalentTo("Slot Type 1");
        }

        [TestMethod]
        public void BuildSlotsInformation_TwoSlotTypes_SlotTypesIncludesTwoSlotTypes()
        {
            // Arrange
            var slot1 = new Slot { Type = "Slot Type 1", SessionName = "Session Name 1" };
            var slot2 = new Slot { Type = "Slot Type 2", SessionName = "Session Name 2" };
            var result = BuildSuccessfulResult(slot1, slot2);

            // Act
            var information = _systemUnderTest.BuildSlotsInformation(_userSession, result);

            // Assert
            information.SlotTypes.Should().BeEquivalentTo("Slot Type 1", "Slot Type 2");
        }

        [TestMethod]
        public void BuildSlotsInformation_MultipleIdenticalSlotTypes_SlotTypesIncludesOnlyDistinctSlotTypes()
        {
            // Arrange
            var slot1 = new Slot { Type = "Slot Type 1", SessionName = "Session Name 1" };
            var slot2 = new Slot { Type = "Slot Type 2", SessionName = "Session Name 2" };
            var slot3 = new Slot { Type = "Slot Type 3", SessionName = "Session Name 3" };
            var slot4 = new Slot { Type = "Slot Type 3", SessionName = "Session Name 4" };
            var result = BuildSuccessfulResult(slot1, slot2, slot3, slot4);

            // Act
            var information = _systemUnderTest.BuildSlotsInformation(_userSession, result);

            // Assert
            information.SlotTypes.Should().BeEquivalentTo("Slot Type 1", "Slot Type 2", "Slot Type 3");
        }

        [TestMethod]
        public void BuildSlotsInformation_OneSlot_SessionNamesIncludesOneSessionName()
        {
            // Arrange
            var slot = new Slot { Type = "Slot Type 1", SessionName = "Session Name 1" };
            var result = BuildSuccessfulResult(slot);

            // Act
            var information = _systemUnderTest.BuildSlotsInformation(_userSession, result);

            // Assert
            information.SessionNames.Should().BeEquivalentTo("Session Name 1");
        }

        [TestMethod]
        public void BuildSlotsInformation_TwoSessionNames_SessionNamesIncludesTwoSessionNames()
        {
            // Arrange
            var slot1 = new Slot { Type = "Slot Type 1", SessionName = "Session Name 1" };
            var slot2 = new Slot { Type = "Slot Type 2", SessionName = "Session Name 2" };
            var result = BuildSuccessfulResult(slot1, slot2);

            // Act
            var information = _systemUnderTest.BuildSlotsInformation(_userSession, result);

            // Assert
            information.SessionNames.Should().BeEquivalentTo("Session Name 1", "Session Name 2");
        }

        [TestMethod]
        public void BuildSlotsInformation_MultipleIdenticalSessionNames_SessionNamesIncludesOnlyDistinctSessionNames()
        {
            // Arrange
            var slot1 = new Slot { Type = "Slot Type 1", SessionName = "Session Name 1" };
            var slot2 = new Slot { Type = "Slot Type 2", SessionName = "Session Name 2" };
            var slot3 = new Slot { Type = "Slot Type 3", SessionName = "Session Name 3" };
            var slot4 = new Slot { Type = "Slot Type 4", SessionName = "Session Name 3" };
            var result = BuildSuccessfulResult(slot1, slot2, slot3, slot4);

            // Act
            var information = _systemUnderTest.BuildSlotsInformation(_userSession, result);

            // Assert
            information.SessionNames.Should().BeEquivalentTo("Session Name 1", "Session Name 2", "Session Name 3");
        }

        [TestMethod]
        public void BuildSlotsInformation_OneSlot_SlotsIncludesOneSlot()
        {
            // Arrange
            var slot = new Slot { Type = "Slot Type 1", SessionName = "Session Name 1" };
            var result = BuildSuccessfulResult(slot);

            // Act
            var information = _systemUnderTest.BuildSlotsInformation(_userSession, result);

            // Assert
            information.Slots.Should().BeEquivalentTo(new[]
                { new AppointmentSlotTypeScraper.Slot("Slot Type 1", "Session Name 1") });
        }
        
        [TestMethod]
        public void BuildSlotsInformation_TwoDistinctSlots_SlotsIncludesTwoSlots()
        {
            // Arrange
            var slot1 = new Slot { Type = "Slot Type 1", SessionName = "Session Name 1" };
            var slot2 = new Slot { Type = "Slot Type 2", SessionName = "Session Name 2" };
            var result = BuildSuccessfulResult(slot1, slot2);

            // Act
            var information = _systemUnderTest.BuildSlotsInformation(_userSession, result);

            // Assert
            information.Slots.Should().BeEquivalentTo(new[]
            {
                new AppointmentSlotTypeScraper.Slot("Slot Type 1", "Session Name 1"),
                new AppointmentSlotTypeScraper.Slot("Slot Type 2", "Session Name 2")
            });
        }
        
        [TestMethod]
        public void BuildSlotsInformation_MultipleIdenticalSlots_SlotsIncludesOnlyDistinctSlots()
        {
            // Arrange
            var slot1 = new Slot { Type = "Slot Type 1", SessionName = "Session Name 1" };
            var slot2 = new Slot { Type = "Slot Type 2", SessionName = "Session Name 2" };
            var slot3 = new Slot { Type = "Slot Type 1", SessionName = "Session Name 2" };
            var slot4 = new Slot { Type = "Slot Type 2", SessionName = "Session Name 2" };
            var result = BuildSuccessfulResult(slot1, slot2, slot3, slot4);

            // Act
            var information = _systemUnderTest.BuildSlotsInformation(_userSession, result);

            // Assert
            information.Slots.Should().BeEquivalentTo(new[]
            {
                new AppointmentSlotTypeScraper.Slot("Slot Type 1", "Session Name 1"),
                new AppointmentSlotTypeScraper.Slot("Slot Type 1", "Session Name 2"),
                new AppointmentSlotTypeScraper.Slot("Slot Type 2", "Session Name 2")
            });
        }
        
        [TestMethod]
        public void BuildSlotsInformation_OdsCodeDerivedFromUserSession()
        {
            // Arrange
            var result = _fixture.Create<AppointmentSlotsResult.SuccessfullyRetrieved>();

            // Act
            var information = _systemUnderTest.BuildSlotsInformation(_userSession, result);

            // Assert
            information.OdsCode.Should().Be(_userSession.GpUserSession.OdsCode);
        }

        [TestMethod]
        public void BuildSlotsInformation_SupplierDerivedFromUserSession()
        {
            // Arrange
            var result = _fixture.Create<AppointmentSlotsResult.SuccessfullyRetrieved>();

            // Act
            var information = _systemUnderTest.BuildSlotsInformation(_userSession, result);

            // Assert
            information.Supplier.Should().Be(_userSession.GpUserSession.Supplier.ToString());
        }

        [TestMethod]
        public void ShouldBeLogged_NeverPreviouslyCalled_ReturnsTrue()
        {
            // Arrange
            var information = _fixture.Create<AppointmentSlotTypeScraper.AppointmentSlotsInformation>();

            // Act
            var shouldBeLogged = _systemUnderTest.ShouldBeLogged(information);

            // Assert
            shouldBeLogged.Should().BeTrue();
        }

        [TestMethod]
        public void ShouldBeLogged_CalledMultipleTimesOnSameDayWithIdenticalInformation_FirstOnlyLogged()
        {
            // Arrange
            var information = _fixture.Create<AppointmentSlotTypeScraper.AppointmentSlotsInformation>();
            var today = DateTime.Today;

            _mockDateTimeProvider.SetupSequence(x => x.Today)
                .Returns(today)
                .Returns(today)
                .Returns(today);

            // Act
            var result1 = _systemUnderTest.ShouldBeLogged(information);
            var result2 = _systemUnderTest.ShouldBeLogged(information);
            var result3 = _systemUnderTest.ShouldBeLogged(information);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeFalse();
            result3.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldBeLogged_CalledOnSubsequentDaysWithIdenticalInformation_LoggedOncePerDay()
        {
            // Arrange
            var information = _fixture.Create<AppointmentSlotTypeScraper.AppointmentSlotsInformation>();
            var yesterday = DateTime.Today.AddDays(-1);
            var today = DateTime.Today;

            _mockDateTimeProvider.SetupSequence(x => x.Today)
                .Returns(yesterday)
                .Returns(yesterday)
                .Returns(today)
                .Returns(today)
                .Returns(today);

            // Act
            var result1 = _systemUnderTest.ShouldBeLogged(information);
            var result2 = _systemUnderTest.ShouldBeLogged(information);
            var result3 = _systemUnderTest.ShouldBeLogged(information);
            var result4 = _systemUnderTest.ShouldBeLogged(information);
            var result5 = _systemUnderTest.ShouldBeLogged(information);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeFalse();
            result3.Should().BeTrue();
            result4.Should().BeFalse();
            result5.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldBeLogged_CalledMultipleTimesOnSameDayWithDecreasedNumberOfSlotTypes_FirstOnlyLogged()
        {
            // Arrange
            var information = _fixture.Create<AppointmentSlotTypeScraper.AppointmentSlotsInformation>();
            var today = DateTime.Today;

            _mockDateTimeProvider.SetupSequence(x => x.Today)
                .Returns(today)
                .Returns(today)
                .Returns(today);

            // Act
            var result1 = _systemUnderTest.ShouldBeLogged(information);
            var result2 = _systemUnderTest.ShouldBeLogged(information);

            var information2 = new AppointmentSlotTypeScraper.AppointmentSlotsInformation(
                new[] { information.SlotTypes[0], information.SlotTypes[1] },
                information.SessionNames, information.Slots, information.Supplier, information.OdsCode);
            var result3 = _systemUnderTest.ShouldBeLogged(information2);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeFalse();
            result3.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldBeLogged_CalledMultipleTimesOnSameDayWithIncreasedNumberOfSlotTypes_LoggedOnIncreasedSlotTypeCount()
        {
            // Arrange
            var information1 = _fixture.Create<AppointmentSlotTypeScraper.AppointmentSlotsInformation>();
            var today = DateTime.Today;

            _mockDateTimeProvider.SetupSequence(x => x.Today)
                .Returns(today)
                .Returns(today)
                .Returns(today);

            // Act
            var result1 = _systemUnderTest.ShouldBeLogged(information1);
            var result2 = _systemUnderTest.ShouldBeLogged(information1);

            var information2 = new AppointmentSlotTypeScraper.AppointmentSlotsInformation(
                new[]
                {
                    information1.SlotTypes[0], information1.SlotTypes[1], information1.SlotTypes[2],
                    _fixture.Create<string>()
                },
                information1.SessionNames, information1.Slots, information1.Supplier, information1.OdsCode);
            var result3 = _systemUnderTest.ShouldBeLogged(information2);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeFalse();
            result3.Should().BeTrue();
        }
        
        [TestMethod]
        public void ShouldBeLogged_CalledMultipleTimesOnSameDayForDifferentOdsCode_LoggedOnOdsCodeChange()
        {
            // Arrange
            var information1 = _fixture.Create<AppointmentSlotTypeScraper.AppointmentSlotsInformation>();
            var today = DateTime.Today;

            _mockDateTimeProvider.SetupSequence(x => x.Today)
                .Returns(today)
                .Returns(today)
                .Returns(today);

            // Act
            var result1 = _systemUnderTest.ShouldBeLogged(information1);
            var result2 = _systemUnderTest.ShouldBeLogged(information1);

            var information2 = new AppointmentSlotTypeScraper.AppointmentSlotsInformation(information1.SlotTypes,
                information1.SessionNames, information1.Slots, information1.Supplier, _fixture.Create<string>());
            var result3 = _systemUnderTest.ShouldBeLogged(information2);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeFalse();
            result3.Should().BeTrue();
        }

        [TestMethod]
        public void LogInformation_HappyPath_LogsInformationWithAppropriateKeyAndFormat()
        {
            // Arrange
            var appointmentSlotsInformation = _fixture.Create<AppointmentSlotTypeScraper.AppointmentSlotsInformation>();
            var expectedLogMessage = "slot_type_data=" + appointmentSlotsInformation.SerializeJson();
            
            // Act
            _systemUnderTest.LogInformation(appointmentSlotsInformation);
            
            // Assert
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, null, Times.Once());
           
        }
        
        private static AppointmentSlotsResult.SuccessfullyRetrieved BuildSuccessfulResult(params Slot[] slots)
        {
            return new AppointmentSlotsResult.SuccessfullyRetrieved(
                new AppointmentSlotsResponse
                {
                    Slots = slots
                });
        }
    }
}