using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class SummaryResponse
    {
        public SummaryResponse()
        {
            Referrals = new List<Referral>();
            UpcomingAppointments = new List<UpcomingAppointment>();
        }

        public IEnumerable<Referral> Referrals { get; set; }

        public IEnumerable<UpcomingAppointment> UpcomingAppointments { get; set; }
    }
}