using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public interface IAvailableAppointmentsResponseMapper
    {
        AppointmentSlotsResponse Map(AvailableAppointmentsResponse availableAppointmentsResponse,
            PatientConfigurationResponse patientConfigurationResponse,
            VisionUserSession userSession);
    }
    
    public class AvailableAppointmentsResponseMapper : IAvailableAppointmentsResponseMapper
    {
        private readonly IAvailableAppointmentsMapper _availableAppointmentMapper;
        
        public AvailableAppointmentsResponseMapper(IAvailableAppointmentsMapper availableAppointmentMapper)
        {
            _availableAppointmentMapper = availableAppointmentMapper;
        }
        
        public AppointmentSlotsResponse Map(AvailableAppointmentsResponse availableAppointmentsResponse,
            PatientConfigurationResponse patientConfigurationResponse,
            VisionUserSession userSession)
        {
            var slots =_availableAppointmentMapper.Map(availableAppointmentsResponse.Appointments);

            return new AppointmentSlotsResponse
            {
                Slots = slots,
                BookingReasonNecessity = userSession.AppointmentBookingReasonNecessity,
                BookingGuidance =
                    ParseWelcomeText(patientConfigurationResponse?.Configuration?.Appointments?.WelcomeText)
            };
        }

        private static string ParseWelcomeText(IReadOnlyCollection<AppointmentsMessage> welcomeText)
        {
            if (welcomeText == null || !welcomeText.Any())
            {
                return null;
            }
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(welcomeText.First().Text ?? string.Empty);

            var bodyText = htmlDoc.DocumentNode.SelectSingleNode("//body")?.InnerText ?? string.Empty;

            var cleansed = Clean(bodyText);

            return string.IsNullOrWhiteSpace(cleansed)
                ? null
                : cleansed;
        }

        private static string Clean(string text)
        {
            return text.Replace("&nbsp;", " ", StringComparison.OrdinalIgnoreCase).Trim();
        }
    }
}