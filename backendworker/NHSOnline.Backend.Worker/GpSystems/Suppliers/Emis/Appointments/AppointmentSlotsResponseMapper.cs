using System;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using System.Net.Http;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public interface IAppointmentSlotsResponseMapper
    {
        AppointmentSlotsResponse Map(AppointmentSlotsGetResponse slotsResponse,
            AppointmentSlotsMetadataGetResponse slotsMetadataResponse, 
            PracticeSettingsGetResponse body, 
            DemographicsGetResponse demographics,
            EmisUserSession userSession);
    }

    public class AppointmentSlotsResponseMapper : IAppointmentSlotsResponseMapper
    {
        private readonly IAppointmentSlotsMapper _slotMapper;

        public AppointmentSlotsResponseMapper(IAppointmentSlotsMapper slotMapper)
        {
            _slotMapper = slotMapper;
        }

        public AppointmentSlotsResponse Map(AppointmentSlotsGetResponse slotsResponse,
            AppointmentSlotsMetadataGetResponse slotsMetadataResponse, 
            PracticeSettingsGetResponse body, 
            DemographicsGetResponse demographics,
            EmisUserSession userSession)
        {
            if(slotsResponse == null)
            {
                throw new HttpRequestException();
            }
            var slots = _slotMapper.Map(slotsResponse?.Sessions, slotsMetadataResponse?.Locations,
                slotsMetadataResponse?.SessionHolders, slotsMetadataResponse?.Sessions);
            
            var response = new AppointmentSlotsResponse
            {
                BookingGuidance = body?.Messages?.AppointmentsMessage?.Trim() ?? string.Empty,
                Slots = slots,
                BookingReasonNecessity = userSession.AppointmentBookingReasonNecessity,
                TelephoneNumbers = demographics?.ContactDetails?.GetTelephoneArray() ?? Array.Empty<PatientTelephoneNumber>()
            };

            return response;
        }
    }
}