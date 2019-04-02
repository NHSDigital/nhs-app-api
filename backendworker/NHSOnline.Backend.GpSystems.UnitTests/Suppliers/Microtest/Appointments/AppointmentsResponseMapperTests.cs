using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.Support.Temporal;
using Appointment = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments.Appointment;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class AppointmentsResponseMapperTests
    {
        private IFixture _fixture;
        private Mock<IDateTimeOffsetProvider> _mockDateTimeOffsetProvider;
        private AppointmentsGetResponse _appointmentsGetResponse;
        private AppointmentsResponseMapper _systemUnderTest;
        private List<Appointment> _getResponseAppointments;
        private IEnumerable<CancellationReason> _expectedCancellationReasons;
        private Mock<ICancellationReasonService> _mockCancellationReasonService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _expectedCancellationReasons = _fixture.CreateMany<CancellationReason>();
            _mockCancellationReasonService = _fixture.Freeze<Mock<ICancellationReasonService>>();
            _mockCancellationReasonService.Setup(x => x.GetDefaultCancellationReasons())
                .Returns(_expectedCancellationReasons)
                .Verifiable();
            
            _fixture.Register(() => new DateTimeOffset(new DateTime(2018, 12, 25, 13, 0, 0)));
            _getResponseAppointments = _fixture.CreateMany<Appointment>().ToList();
            _appointmentsGetResponse = _fixture.Build<AppointmentsGetResponse>()
                .With(x => x.Appointments, _getResponseAppointments)
                .Create();

            _mockDateTimeOffsetProvider = _fixture.Freeze<Mock<IDateTimeOffsetProvider>>();
            _mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset())
                .Returns(new DateTimeOffset(new DateTime(2018, 12, 25, 12, 30, 0)));
            _systemUnderTest = _fixture.Create<AppointmentsResponseMapper>();
        }

        [TestMethod]
        public void Map_WhenPassedAppointments_MapsCorrectly()
        {
            // Act
            var response = _systemUnderTest.Map(_appointmentsGetResponse);

            // Assert
            foreach (var upcomingAppointment in response.UpcomingAppointments)
            {
                upcomingAppointment.Should()
                    .BeEquivalentTo(_getResponseAppointments.First(a => a.Id.Equals(upcomingAppointment.Id,
                        StringComparison.Ordinal)));
                upcomingAppointment.SessionName.Should().BeEmpty();
            }
            
            response.CancellationReasons.Should().BeEquivalentTo(_expectedCancellationReasons);
            _mockCancellationReasonService.Verify();
        }

        [TestMethod]
        public void Map_WhenPassedAppointmentWithNullStartTime_ExcludesIt()
        {
            // Arrange
            var invalidAppointment = _appointmentsGetResponse.Appointments.First();
            invalidAppointment.StartTime = null;

            // Act
            var response = _systemUnderTest.Map(_appointmentsGetResponse);

            // Assert
            response.UpcomingAppointments
                .FirstOrDefault(a => a.Id.Equals(invalidAppointment.Id, StringComparison.Ordinal)).Should().BeNull();
            response.UpcomingAppointments.Count().Should().Be(_appointmentsGetResponse.Appointments.Count() - 1);
        }

        [TestMethod]
        public void Map_WhenPassedAppointmentWithStartTimeInPast_ExcludesIt()
        {
            // Arrange
            var appointmentInPast = _appointmentsGetResponse.Appointments.First();
            appointmentInPast.StartTime = (new DateTimeOffset(new DateTime(2018, 12, 25, 12, 29, 0)));

            // Act
            var response = _systemUnderTest.Map(_appointmentsGetResponse);

            // Assert
            response.UpcomingAppointments
                .FirstOrDefault(a => a.Id.Equals(appointmentInPast.Id, StringComparison.Ordinal)).Should().BeNull();
            response.UpcomingAppointments.Count().Should().Be(_appointmentsGetResponse.Appointments.Count() - 1);
        }
    }
}