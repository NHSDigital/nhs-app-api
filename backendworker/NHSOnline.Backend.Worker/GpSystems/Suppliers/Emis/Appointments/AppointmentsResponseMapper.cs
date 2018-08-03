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
        AppointmentsResponse Map(AppointmentsGetResponse sourceAppointments);
    }

    public class AppointmentsResponseMapper : IAppointmentsResponseMapper
    {
        private readonly IAppointmentsMapper _appointmentsMapper;

        public AppointmentsResponseMapper(
            IAppointmentsMapper appointmentsMapper)
        {
            _appointmentsMapper = appointmentsMapper;
        }

        public AppointmentsResponse Map(AppointmentsGetResponse sourceAppointments)
        {
            var appointments = _appointmentsMapper.Map(
                sourceAppointments.Appointments,
                sourceAppointments.Locations,
                sourceAppointments.SessionHolders,
                sourceAppointments.Sessions);

            var cancellationReasons = GetDefaultCancellationReasons();

            var response = new AppointmentsResponse
            {
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