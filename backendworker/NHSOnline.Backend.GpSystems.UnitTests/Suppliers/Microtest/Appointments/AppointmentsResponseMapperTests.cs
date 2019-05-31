using System;
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
        private AppointmentsGetResponse _pastAppointmentsGetResponse;
        private AppointmentsResponseMapper _systemUnderTest;
        private List<Appointment> _getResponseAppointments;
        private List<Appointment> _getPastResponseAppointments;
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

            var appointmentDate = new DateTimeOffset(new DateTime(2018, 12, 25, 12, 30, 0));

            _fixture.Register(() => appointmentDate.AddHours(1));
            _getResponseAppointments = _fixture.CreateMany<Appointment>().ToList();
            _appointmentsGetResponse = _fixture.Build<AppointmentsGetResponse>()
                .With(x => x.Appointments, _getResponseAppointments)
                .Create();

            _fixture.Register(() => appointmentDate.AddHours(-1));
            _getPastResponseAppointments = _fixture.CreateMany<Appointment>().ToList();
            _pastAppointmentsGetResponse = _fixture.Build<AppointmentsGetResponse>()
                .With(x => x.Appointments, _getPastResponseAppointments)
                .Create();

            _mockDateTimeOffsetProvider = _fixture.Freeze<Mock<IDateTimeOffsetProvider>>();
            _mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset())
                .Returns(appointmentDate.AddHours(1));
            _systemUnderTest = _fixture.Create<AppointmentsResponseMapper>();
        }

        [TestMethod]
        public void Map_WhenPassedUpcomingAppointments_MapsCorrectly()
        {
            // Act
            var response = _systemUnderTest.Map(_appointmentsGetResponse);

            // Assert
            response.UpcomingAppointments.Count().Should().Be(3);
            foreach (var upcomingAppointment in response.UpcomingAppointments)
            {
                upcomingAppointment.Should()
                    .BeEquivalentTo(_getResponseAppointments.First(a => a.Id.Equals(upcomingAppointment.Id,
                        StringComparison.Ordinal)));
                upcomingAppointment.SessionName.Should().BeEmpty();
            }

            response.PastAppointments.Count().Should().Be(0);

            response.DisableCancellation.Should().Be(false);
            response.CancellationReasons.Should().BeEquivalentTo(_expectedCancellationReasons);
            _mockCancellationReasonService.Verify();
        }

        [TestMethod]
        public void Map_WhenPassedPastAppointments_MapsCorrectly()
        {
            // Act
            var response = _systemUnderTest.Map(_pastAppointmentsGetResponse);

            response.PastAppointments.Count().Should().Be(3);
            foreach (var pastAppointments in response.PastAppointments)
            {
                pastAppointments.Should()
                    .BeEquivalentTo(_getPastResponseAppointments.First(a => a.Id.Equals(pastAppointments.Id,
                        StringComparison.Ordinal)));
                pastAppointments.SessionName.Should().BeEmpty();
            }

            response.UpcomingAppointments.Count().Should().Be(0);

            response.PastAppointmentsEnabled.Should().Be(true);
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
    }
}