using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class SummaryResponse
    {
        public SummaryResponse()
        {
            ReferralsNotInReview = new List<Referral>();
            ReferralsInReview = new List<Referral>();
            UnconfirmedAppointments = new List<UpcomingAppointment>();
            ConfirmedAppointments = new List<UpcomingAppointment>();
        }

        public IEnumerable<Referral> ReferralsNotInReview { get; set; }
        public IEnumerable<Referral> ReferralsInReview { get; set; }
        public IEnumerable<UpcomingAppointment> UnconfirmedAppointments { get; set; }
        public IEnumerable<UpcomingAppointment> ConfirmedAppointments { get; set; }
    }
}