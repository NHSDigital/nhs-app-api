using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using Location = NHSOnline.Backend.Worker.Areas.Appointments.Models.Location;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Appointments
{
    public class AppointmentLocationMapper
    {
        public Location[] Map(AppointmentSlotsMetadataGetResponse slotsMetadataResponse)
        {
            var locations = new List<Location>();

            if (slotsMetadataResponse.Locations == null)
            {
                return locations.ToArray();
            }

            locations.AddRange(slotsMetadataResponse.Locations.Select(emisLlocation => new Location
            {
                Id = emisLlocation.LocationId.ToString(),
                DisplayName = emisLlocation.LocationName
            }));

            return locations.ToArray();
        }
    }
}
