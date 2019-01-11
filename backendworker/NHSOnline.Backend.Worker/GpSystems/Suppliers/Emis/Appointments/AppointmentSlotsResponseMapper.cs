using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using System.Net.Http;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public interface IAppointmentSlotsResponseMapper
    {
        AppointmentSlotsResponse Map(AppointmentSlotsGetResponse slotsResponse,
            AppointmentSlotsMetadataGetResponse slotsMetadataResponse, PracticeSettingsGetResponse body, EmisUserSession userSession);
    }

    public class AppointmentSlotsResponseMapper : IAppointmentSlotsResponseMapper
    {
        private readonly IAppointmentSlotsMapper _slotMapper;

        public AppointmentSlotsResponseMapper(IAppointmentSlotsMapper slotMapper)
        {
            _slotMapper = slotMapper;
        }

        public AppointmentSlotsResponse Map(AppointmentSlotsGetResponse slotsResponse,
            AppointmentSlotsMetadataGetResponse slotsMetadataResponse, PracticeSettingsGetResponse body, EmisUserSession userSession)
        {
            if(slotsResponse == null)
            {
                throw new HttpRequestException();
            }
            var slots = _slotMapper.Map(slotsResponse?.Sessions, slotsMetadataResponse?.Locations,
                slotsMetadataResponse?.SessionHolders, slotsMetadataResponse?.Sessions);
            
            var response = new AppointmentSlotsResponse
            {
                BookingGuidance = body?.Messages?.AppointmentsMessage ?? string.Empty,
                Slots = slots,
                BookingReasonNecessity = userSession.AppointmentBookingReasonNecessity
            };

            return response;
        }
    }
}