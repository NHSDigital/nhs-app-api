using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.PfsApi.Areas.Appointments;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Appointments
{
    [TestClass]
    public class AppointmentTypeTransformingVisitorTests
    {
        private IFixture _fixture;
        private AppointmentTypeTransformingVisitor _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<AppointmentTypeTransformingVisitor>();
        }

        [TestMethod]
        public void Visit_SuccessfulAppointmentsResult_TransformsPastAndUpcomingAppointments()
        {
            // Arrange
            var successfulResult = _fixture.Create<AppointmentsResult.Success>();
            var systemUnderTest = _fixture.Create<Mock<AppointmentTypeTransformingVisitor>>();
            systemUnderTest.CallBase = true;
            
            // Act
            systemUnderTest.Object.Visit(successfulResult);

            // Assert
            systemUnderTest.Verify(x => x.TransformSlots(successfulResult.Response.PastAppointments));
            systemUnderTest.Verify(x => x.TransformSlots(successfulResult.Response.UpcomingAppointments));
        }

        [TestMethod]
        public async Task Visit_SuccessfulAppointmentsResult_TransformsPastAppointmentsIntegrationTest()
        {
            // Arrange
            var successfulResult = _fixture.Create<AppointmentsResult.Success>();
            
            var firstPastAppointment = successfulResult.Response.PastAppointments.First();
            firstPastAppointment.Type = "Default";

            // Act
            await _systemUnderTest.Visit(successfulResult);
            
            // Assert
            firstPastAppointment.Type.Should().Be("General");
            firstPastAppointment.TypeFromGpSystem.Should().Be("Default");
        }
        
        [TestMethod]
        public async Task Visit_SuccessfulAppointmentsResult_TransformsUpcomingAppointmentsIntegrationTest()
        {
            // Arrange
            var successfulResult = _fixture.Create<AppointmentsResult.Success>();
            
            var firstUpcomingAppointment = successfulResult.Response.UpcomingAppointments.First();
            firstUpcomingAppointment.Type = "Default";

            // Act
            await _systemUnderTest.Visit(successfulResult);
            
            // Assert
            firstUpcomingAppointment.Type.Should().Be("General");
            firstUpcomingAppointment.TypeFromGpSystem.Should().Be("Default");
        }

        [TestMethod]
        public void Visit_SuccessfulAppointmentSlotsResult_TransformsSlots()
        {
            // Arrange
            var successfulResult = _fixture.Create<AppointmentSlotsResult.Success>();
            var systemUnderTest = _fixture.Create<Mock<AppointmentTypeTransformingVisitor>>();
            systemUnderTest.CallBase = true;
            
            // Act
            systemUnderTest.Object.Visit(successfulResult);
            
            // Assert
            systemUnderTest.Verify(x => x.TransformSlots(successfulResult.Response.Slots));
        }

        [TestMethod]
        public async Task Visit_SuccessfulAppointmentsSlotsResult_TransformsSlotsIntegrationTest()
        {
            // Arrange
            var successfulResult = _fixture.Create<AppointmentSlotsResult.Success>();
            successfulResult.Response.Slots = _fixture.CreateMany<Slot>().ToArray();
            
            var firstSlot = successfulResult.Response.Slots.First();
            firstSlot.Type = "Default";

            // Act
            await _systemUnderTest.Visit(successfulResult);
            
            // Assert
            firstSlot.Type.Should().Be("General");
            firstSlot.TypeFromGpSystem.Should().Be("Default");
        }
        
        [TestMethod]
        public void TransformSlots_NullEnumerable_NoOp()
        {
            // Arrange
            List<DummySlot> slots = null;
            var systemUnderTest = _fixture.Create<Mock<AppointmentTypeTransformingVisitor>>();
            systemUnderTest.CallBase = true;
            
            // Act
            systemUnderTest.Object.TransformSlots(slots);
            
            // Assert
            systemUnderTest.Verify(x => x.TransformSlot(It.IsAny<ISlot>()), Times.Never);
        }

        [TestMethod]
        public void TransformSlots_ValidEnumerable_TransformSlotCalledForEachSlot()
        {
            // Arrange
            var slots = _fixture.CreateMany<DummySlot>();
            var systemUnderTest = _fixture.Create<Mock<AppointmentTypeTransformingVisitor>>();
            systemUnderTest.CallBase = true;

            // Act
            systemUnderTest.Object.TransformSlots(slots);
            
            // Assert
            foreach (var slot in slots)
            {
                systemUnderTest.Verify(x => x.TransformSlot(slot), Times.Once);
            }
        }

        [TestMethod]
        public void TransformSlot_NullSlot_NoOp()
        {
            // Arrange
            ISlot slot = null;

            // Act
            Action act = () => _systemUnderTest.TransformSlot(slot);

            // Assert
            act.Should().NotThrow();
        }

        [DataTestMethod]
        [DataRow("default")]
        [DataRow("Default")]
        [DataRow("DEFAULT")]
        [DataRow("DeFAulT")]
        public void TransformSlot_DefaultSlotTypeOfAnyCase_IsTranslatedToGeneral(string typeFromGpSystem)
        {
            // Arrange
            var slot = _fixture.Build<DummySlot>()
                .With(x => x.Type, typeFromGpSystem)
                .With(x => x.TypeFromGpSystem, (string)null)
                .Create();

            // Act
            _systemUnderTest.TransformSlot(slot);

            // Assert
            slot.TypeFromGpSystem.Should().Be(typeFromGpSystem);
            slot.Type.Should().Be("General");
        }

        [TestMethod]
        public void TransformSlot_AnyOtherSlotType_IsUnchangedButCopiedToTypeFromGpSystem()
        {
            // Arrange
            var slot = _fixture.Build<DummySlot>()
                .With(x => x.TypeFromGpSystem, (string) null)
                .Create();

            var typeFromGpSystem = slot.Type;
            
            // Act
            _systemUnderTest.TransformSlot(slot);

            // Assert
            slot.TypeFromGpSystem.Should().Be(typeFromGpSystem);
            slot.Type.Should().Be(typeFromGpSystem);
        }

        private class DummySlot : ISlot
        {
            public DateTimeOffset StartTime { get; set; }
            public DateTimeOffset? EndTime { get; set; }
            public string Location { get; set; }
            public IEnumerable<string> Clinicians { get; set; }
            public string Type { get; set; }
            public string TypeFromGpSystem { get; set; }
            public string SessionName { get; set; }
            public Channel Channel { get; set; }
        }
    }
}