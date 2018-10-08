using System;
using System.Collections.Generic;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public interface ICancellationReasonMapper
    {
        IEnumerable<CancellationReason> Map(BookedAppointmentsResponse appointmentsResponses);
    }
    
    public class CancellationReasonMapper : ICancellationReasonMapper
    {
        private const string Language = "en_UK";
        
        public IEnumerable<CancellationReason> Map(BookedAppointmentsResponse appointmentsResponses)
        {
            var cancellationReasons = new List<CancellationReason>();
            if (appointmentsResponses?.Appointments?.Settings?.CancellationReasons == null)
                return cancellationReasons;
            
            foreach (var reason in appointmentsResponses.Appointments.Settings.CancellationReasons)
            {

                var displayName = "";
                foreach (var description in reason.Descriptions)
                {
                    if (!description.Language.Equals(Language, StringComparison.Ordinal)) continue;
                    displayName = description.Text;
                    break;
                }

                cancellationReasons.Add(
                    new CancellationReason
                    {
                        Id = reason.Id,
                        DisplayName = displayName
                    }
                );
            }

            return cancellationReasons;
        }
    }
}