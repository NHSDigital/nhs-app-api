using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using Location = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public interface IAppointmentSlotsResponseMapper
    {
        AppointmentSlotsResponse Map(AppointmentSlotsGetResponse slotsResponse,
            AppointmentSlotsMetadataGetResponse slotsMetadataResponse);
    }

    public class AppointmentSlotsResponseMapper : IAppointmentSlotsResponseMapper
    {
        private readonly IMapper<Location, Areas.Appointments.Models.Location> _locationMapper;
        private readonly IMapper<Models.Session, AppointmentSession> _sessionMapper;
        private readonly IMapper<Models.SessionHolder, Clinician> _clinicianMapper;
        private readonly IAppointmentSlotsMapper _slotMapper;

        public AppointmentSlotsResponseMapper(IMapper<SessionHolder, Clinician> clinicianMapper,
            IMapper<Models.Session, AppointmentSession> sessionMapper,
            IMapper<Location, Areas.Appointments.Models.Location> locationMapper, IAppointmentSlotsMapper slotMapper)
        {
            _clinicianMapper = clinicianMapper;
            _sessionMapper = sessionMapper;
            _locationMapper = locationMapper;
            _slotMapper = slotMapper;
        }

        public AppointmentSlotsResponse Map(AppointmentSlotsGetResponse slotsResponse,
            AppointmentSlotsMetadataGetResponse slotsMetadataResponse)
        {
            var slots = _slotMapper.Map(slotsResponse.Sessions, slotsMetadataResponse.Locations,
                slotsMetadataResponse.SessionHolders, slotsMetadataResponse.Sessions);

            var locations = _locationMapper.Map(slotsMetadataResponse.Locations);
            var clinicians = _clinicianMapper.Map(slotsMetadataResponse.SessionHolders);
            var sessions = _sessionMapper.Map(slotsMetadataResponse.Sessions);

            var response = new AppointmentSlotsResponse
            {
                Locations = locations,
                Clinicians = clinicians,
                AppointmentSessions = sessions,
                Slots = slots
            };

            return response;
        }
    }
}