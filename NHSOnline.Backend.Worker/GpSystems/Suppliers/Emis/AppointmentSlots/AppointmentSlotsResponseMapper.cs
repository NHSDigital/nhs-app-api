using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Date;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.AppointmentSlots
{
    public interface IAppointmentSlotsResponseMapper
    {
        AppointmentSlotsResponse Map(AppointmentsSlotsGetResponse slotsResponse,
            AppointmentSlotsMetadataGetResponse slotsMetadataResponse);
    }
    
    public class AppointmentSlotsResponseMapper : IAppointmentSlotsResponseMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        
        public AppointmentSlotsResponseMapper(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }
        
        public AppointmentSlotsResponse Map(AppointmentsSlotsGetResponse slotsResponse, AppointmentSlotsMetadataGetResponse slotsMetadataResponse)
        {
            var response = new AppointmentSlotsResponse
            {
                Slots = new AppointmentSlotsMapper(_dateTimeOffsetProvider).Map(slotsResponse, slotsMetadataResponse),
                Locations = new AppointmentLocationMapper().Map(slotsMetadataResponse),
                Clinicians = new AppointmentClinicianMapper().Map(slotsMetadataResponse),
                AppointmentSessions = new AppointmentSessionMapper().Map(slotsMetadataResponse),
            };

            return response;
        }
    }
}