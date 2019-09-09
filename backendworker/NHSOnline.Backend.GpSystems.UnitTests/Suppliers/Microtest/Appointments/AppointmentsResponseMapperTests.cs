using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using Appointment = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments.Appointment;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class AppointmentsResponseMapperTests
    {
        private IFixture _fixture;
        private AppointmentsGetResponse _appointmentsGetResponse;
        private AppointmentsResponseMapper _systemUnderTest;
        private List<Appointment> _getResponseAppointments;
        private List<NHSOnline.Backend.GpSystems.Appointments.Models.Appointment> _getAppointments;
        private IEnumerable<CancellationReason> _expectedCancellationReasons;
        private Mock<ICancellationReasonService> _mockCancellationReasonService;
        private Mock<IAppointmentsMapper> _mockAppointmentsMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _expectedCancellationReasons = _fixture.CreateMany<CancellationReason>();
            _mockCancellationReasonService = _fixture.Freeze<Mock<ICancellationReasonService>>();
            _mockCancellationReasonService.Setup(x => x.GetDefaultCancellationReasons())
                .Returns(_expectedCancellationReasons)
                .Verifiable();
            
            _getResponseAppointments = _fixture.CreateMany<Appointment>().ToList();
            _appointmentsGetResponse = _fixture.Build<AppointmentsGetResponse>()
                .With(x => x.Appointments, _getResponseAppointments)
                .Create();

            _getAppointments = new List<NHSOnline.Backend.GpSystems.Appointments.Models.Appointment>
            {
                _fixture.Create<UpcomingAppointment>(), 
                _fixture.Create<PastAppointment>()
            };
            
            _mockAppointmentsMapper = _fixture.Freeze<Mock<IAppointmentsMapper>>();
            _mockAppointmentsMapper.Setup(x => x.Map(It.IsAny<IEnumerable<Appointment>>()))
                .Returns(_getAppointments);

            _systemUnderTest = _fixture.Create<AppointmentsResponseMapper>();
        }

        [TestMethod]
        public void Map_WhenPassedAppointments_ReturnsExpectedUpcomingAppointments()
        {
            // Act
            var response = _systemUnderTest.Map(_appointmentsGetResponse);

            // Assert
            _getAppointments.Should().Contain(response.UpcomingAppointments);
            _mockAppointmentsMapper.Verify();
        }
        
        [TestMethod]
        public void Map_WhenPassedAppointments_ReturnsExpectedPastAppointments()
        {
            // Act
            var response = _systemUnderTest.Map(_appointmentsGetResponse);

            // Assert
            _getAppointments.Should().Contain(response.PastAppointments);
            _mockAppointmentsMapper.Verify();
        }
        
        [TestMethod]
        public void Map_WhenPassedAppointments_ReturnsExpectedCancellationReasons()
        {
            // Act
            var response = _systemUnderTest.Map(_appointmentsGetResponse);

            // Assert
            response.CancellationReasons.Should().BeEquivalentTo(_expectedCancellationReasons);
            _mockCancellationReasonService.Verify();
        }
    }
}