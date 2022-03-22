using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class SummaryResponse
    {
        public IEnumerable<Referral> Referrals { get; set; }

        public IEnumerable<UpcomingAppointment> UpcomingAppointments { get; set; }
    }
}