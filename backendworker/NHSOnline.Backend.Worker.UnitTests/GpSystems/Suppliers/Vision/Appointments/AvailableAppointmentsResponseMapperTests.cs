using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Appointments
{
    [TestClass]
    public class AvailableAppointmentsResponseMapperTests
    {
        private IFixture _fixture;
        private AvailableAppointmentsResponseMapper _systemUnderTest;
        private VisionUserSession _userSession;
        private AvailableAppointmentsResponse _slotsResponse;
        private PatientConfigurationResponse _configResponse;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<AvailableAppointmentsResponseMapper>();
            _userSession = _fixture.Create<VisionUserSession>();
            _slotsResponse = _fixture.Create<AvailableAppointmentsResponse>();
            _configResponse = _fixture.Create<PatientConfigurationResponse>();
        }
        
        [TestMethod]
        public void Map_ReturnsAppointmentSlots()
        {
            // Act
            var response = _systemUnderTest.Map(_slotsResponse, _configResponse, _userSession);

            // Assert
            response.Slots.Should().BeEquivalentTo(new List<FreeSlot>());
        }

        [DataTestMethod]
        [DataRow(Necessity.Optional)]
        [DataRow(Necessity.NotAllowed)]
        public void Map_SetsBookingReasonNecessity(Necessity expectedNecessity)
        {
            // Arrange
            _userSession.AppointmentBookingReasonNecessity = expectedNecessity;

            // Act
            var response = _systemUnderTest.Map(_slotsResponse, _configResponse, _userSession);

            // Assert
            response.BookingReasonNecessity.Should().Be(expectedNecessity);
        }

        [TestMethod]
        public void Map_BookingGuidanceExists_SetsBookingGuidance()
        {
            // Arrange
            var bookingGuidance = _fixture.Create<string>();

            _configResponse.Configuration.Appointments.WelcomeText = new List<AppointmentsMessage>
            {
                new AppointmentsMessage
                {
                    Text = $"<html><body>{bookingGuidance}</body></html>"
                }
            };
            // Act
            var response = _systemUnderTest.Map(_slotsResponse, _configResponse, _userSession);

            // Assert
            response.BookingGuidance.Should()
                .Be(bookingGuidance);
        }

        [DataTestMethod]
        [DataRow("<HTML><BODY></BODY></HTML>")]
        [DataRow("  ")]
        [DataRow("")]
        [DataRow(null)]
        public void Map_BookingGuidanceDoesNotExist_SetsBookingGuidanceNull(string bookingGuidance)
        {
            // Arrange
            _configResponse.Configuration.Appointments.WelcomeText = new List<AppointmentsMessage>
            {
                new AppointmentsMessage
                {
                    Text = bookingGuidance
                }
            };

            // Act
            var response = _systemUnderTest.Map(_slotsResponse, _configResponse, _userSession);

            // Assert
            response.BookingGuidance.Should().Be(null);
        }

        [TestMethod]
        public void Map_BookingGuidanceSetToNbsp_SetsBookingGuidanceNull()
        {
            // Arrange
            var bookingGuidance = "<![CDATA[<HTML><HEAD>" + Environment.NewLine
                                                          + @"<META name=GENERATOR content=""MSHTML 11.00.9600.19236"" ></HEAD>" + Environment.NewLine
                                                          + "<BODY>" + Environment.NewLine
                                                          + "<P>&nbsp;</P></BODY></HTML>]]>";

            _configResponse.Configuration.Appointments.WelcomeText = new List<AppointmentsMessage>
            {
                new AppointmentsMessage
                {
                    Text = bookingGuidance
                }
            };

            // Act
            var response = _systemUnderTest.Map(_slotsResponse, _configResponse, _userSession);

            // Assert
            response.BookingGuidance.Should().Be(null);
        }

        [TestMethod]
        public void Map_BookingGuidanceStartsAndEndsWithCarriageReturns_RemovesAdditionalCarriageReturns()
        {
            // Arrange
            var expectedBookingGuidance = _fixture.Create<string>();

            var bookingGuidance = "<![CDATA[<HTML><HEAD>" + Environment.NewLine
                                                          + @"<META name=GENERATOR content=""MSHTML 11.00.9600.19236"" ></HEAD>" + Environment.NewLine
                                                          + "<BODY>" + Environment.NewLine
                                                          + $"<P>{expectedBookingGuidance}</P>" + Environment.NewLine
                                                          + "</BODY></HTML>]]>";

            _configResponse.Configuration.Appointments.WelcomeText = new List<AppointmentsMessage>
            {
                new AppointmentsMessage
                {
                    Text = bookingGuidance
                }
            };

            // Act
            var response = _systemUnderTest.Map(_slotsResponse, _configResponse, _userSession);

            // Assert
            response.BookingGuidance.Should().Be(expectedBookingGuidance);
        }
    }
}