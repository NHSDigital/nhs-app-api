using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.AppointmentSlots
{
    public class AppointmentClinicianMapper
    {
        public Clinician[] Map(AppointmentSlotsMetadataGetResponse slotsMetadataResponse)
        {
            var clinicians = new List<Clinician>();

            if (slotsMetadataResponse.SessionHolders == null)
            {
                return clinicians.ToArray();
            }

            clinicians.AddRange(slotsMetadataResponse.SessionHolders.Select(emisSessionHolder => new Clinician
            {
                Id = emisSessionHolder.ClinicianId.ToString(),
                DisplayName = emisSessionHolder.DisplayName
            }));

            return clinicians.ToArray();
        }
    }
}
