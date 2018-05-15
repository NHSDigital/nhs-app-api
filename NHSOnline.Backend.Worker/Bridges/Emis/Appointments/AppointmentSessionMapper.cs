using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Appointments
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
                DisplayName = emisSession.SessionName
            }));

            return appointmentSessions.ToArray();
        }
    }
}
