using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Appointments
{
    [TestClass]
    public class BookedAppointmentsResponseMapperTests
    {
        private IFixture _fixture;
        private List<CancellationReason> _cancellationReasons;
        private List<UpcomingAppointment> _appointments;
        private BookedAppointmentsResponse _bookedAppointmentsResponse;
        private BookedAppointmentsResponseMapper _systemUnderTest;
        private Mock<IBookedAppointmentMapper> _bookedAppointmentMapper;
        private Mock<ICancellationReasonMapper> _cancellationReasonMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _bookedAppointmentsResponse = _fixture.Create<BookedAppointmentsResponse>();
            _bookedAppointmentMapper = _fixture.Freeze<Mock<IBookedAppointmentMapper>>();
            _cancellationReasonMapper = _fixture.Freeze<Mock<ICancellationReasonMapper>>();

            _cancellationReasons = _fixture.CreateMany<CancellationReason>().ToList();
            _appointments = _fixture.CreateMany<UpcomingAppointment>().ToList();

            _bookedAppointmentMapper.Setup(b => b.Map(_bookedAppointmentsResponse.Appointments))
                .Returns(_appointments);
            _cancellationReasonMapper.Setup(c => c.Map(_bookedAppointmentsResponse))
                .Returns(_cancellationReasons);

            _systemUnderTest = _fixture.Create<BookedAppointmentsResponseMapper>();
        }
        
        [TestMethod]
        public void Map_ResponseIncludesMappedElements()
        {

            // Act
            var response = _systemUnderTest.Map(_bookedAppointmentsResponse);

            // Assert
            response.CancellationReasons.Should().BeEquivalentTo(_cancellationReasons);
            response.UpcomingAppointments.Should().BeEquivalentTo(_appointments);
        }

        [TestMethod]
        public void Map_CancellationReasonsMapped_CancellationIsEnabled()
        {
            // Act
            var response = _systemUnderTest.Map(_bookedAppointmentsResponse);

            // Assert
            response.DisableCancellation.Should().BeFalse();
        }

        [TestMethod]
        public void Map_NoCancellationReasonsMapped_CancellationIsDisabled()
        {
            // Arrange
            _cancellationReasonMapper.Reset();
            _cancellationReasonMapper.Setup(c => c.Map(_bookedAppointmentsResponse))
                .Returns(new List<CancellationReason>());

            // Act
            var response = _systemUnderTest.Map(_bookedAppointmentsResponse);

            // Assert
            response.DisableCancellation.Should().BeTrue();
        }
    }
}