using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class AppointmentSlotsResponseMapper : IAppointmentSlotsResponseMapper
    {
        private readonly IAppointmentSlotsMapper _appointmentSlotsMapper;
        private readonly ILogger<AppointmentSlotsResponseMapper> _logger;

        public AppointmentSlotsResponseMapper(IAppointmentSlotsMapper appointmentSlotsMapper,
            ILogger<AppointmentSlotsResponseMapper> logger)
        {
            _appointmentSlotsMapper = appointmentSlotsMapper;
            _logger = logger;
        }

        public AppointmentSlotsResponse Map(
            AppointmentSlotsGetResponse slotsResponse, 
            DemographicsGetResponse demographicsResponse)
        {
            _logger.LogEnter();

            var slots = _appointmentSlotsMapper.Map(slotsResponse.Slots);
            var telephoneNumbers = MapTelephoneNumbers(demographicsResponse);

            var response = new AppointmentSlotsResponse
            {
                Slots = slots,
                BookingReasonNecessity = Necessity.Mandatory,
                BookingGuidance = string.Empty,
                TelephoneNumbers = telephoneNumbers
            };

            _logger.LogExit();
            return response;
        }
        
        private static IList<PatientTelephoneNumber> MapTelephoneNumbers(DemographicsGetResponse demographicsResponse)
        {
            if (demographicsResponse?.Demographics == null)
            {
                return new List<PatientTelephoneNumber>();
            }
            
            return MapTelephoneNumbers(new[]{
                demographicsResponse.Demographics.Telephone1,
                demographicsResponse.Demographics.Telephone2});
        }
        
        private static IList<PatientTelephoneNumber> MapTelephoneNumbers(IEnumerable<string> sourceNumbers)
        {
            var numbers = sourceNumbers
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => new PatientTelephoneNumber() { TelephoneNumber = x })
                .ToList();

            return numbers;
        }
    }
}