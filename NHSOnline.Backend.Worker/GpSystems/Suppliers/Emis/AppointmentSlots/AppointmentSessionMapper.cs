using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.AppointmentSlots
{
    public class AppointmentSessionMapper
    {
        public AppointmentSession[] Map(AppointmentSlotsMetadataGetResponse slotsMetadataResponse)
        {
            var appointmentSessions = new List<AppointmentSession>();

            if (slotsMetadataResponse.Sessions == null)
            {
                return appointmentSessions.ToArray();
            }

            appointmentSessions.AddRange(slotsMetadataResponse.Sessions.Select(emisSession => new AppointmentSession
            {
                Id = emisSession.SessionId.ToString(),
                DisplayName = emisSession.SessionType
            }));

            return appointmentSessions.ToArray();
        }
    }
}
