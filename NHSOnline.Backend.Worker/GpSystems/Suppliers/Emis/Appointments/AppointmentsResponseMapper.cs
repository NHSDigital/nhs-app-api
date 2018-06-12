using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using Location = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public interface IAppointmentsResponseMapper
    {
        AppointmentsResponse Map(AppointmentsGetResponse appointmentsGetResponse);
    }

    public class AppointmentsResponseMapper : IAppointmentsResponseMapper
    {
        private readonly IAppointmentsMapper _appointmentsMapper;
        private readonly IMapper<SessionHolder, Clinician> _clinicianMapper;
        private readonly IMapper<Location, Areas.Appointments.Models.Location> _locationMapper;
        private readonly IMapper<Models.Session, AppointmentSession> _sessionMapper;

        public AppointmentsResponseMapper(
            IMapper<SessionHolder, Clinician> clinicianMapper,
            IMapper<Models.Session, AppointmentSession> sessionMapper,
            IMapper<Location, Areas.Appointments.Models.Location> locationMapper,
            IAppointmentsMapper appointmentsMapper)
        {
            _clinicianMapper = clinicianMapper;
            _sessionMapper = sessionMapper;
            _locationMapper = locationMapper;
            _appointmentsMapper = appointmentsMapper;
        }

        public AppointmentsResponse Map(AppointmentsGetResponse appointmentsResponse)
        {
            var appointments = _appointmentsMapper.Map(
                appointmentsResponse.Appointments,
                appointmentsResponse.Locations,
                appointmentsResponse.SessionHolders,
                appointmentsResponse.Sessions);

            var locations = _locationMapper.Map(appointmentsResponse.Locations);
            var clinicians = _clinicianMapper.Map(appointmentsResponse.SessionHolders);
            var sessions = _sessionMapper.Map(appointmentsResponse.Sessions);
            var cancellationReasons = GetDefaultCancellationReasons();

            var response = new AppointmentsResponse
            {
                Locations = locations,
                Clinicians = clinicians,
                AppointmentSessions = sessions,
                Appointments = appointments,
                CancellationReasons = cancellationReasons
            };

            return response;
        }

        private static IEnumerable<CancellationReason> GetDefaultCancellationReasons()
        {
            var resourceManager = new ResourceManager(typeof(CancellationReasons));

            var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);

            return resourceSet.Cast<DictionaryEntry>()
                .Select(r => new CancellationReason
                {
                    Id = r.Key.ToString(),
                    DisplayName = r.Value.ToString()
                });
        }
    }
}