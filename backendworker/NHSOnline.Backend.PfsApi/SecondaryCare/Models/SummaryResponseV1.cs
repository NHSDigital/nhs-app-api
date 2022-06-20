using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class SummaryResponseV1 : ISummaryResponse
    {
        public SummaryResponseV1()
        {
            ReferralsNotInReview = new List<Referral>();
            ReferralsInReview = new List<Referral>();
            UnconfirmedAppointments = new List<UpcomingAppointment>();
            ConfirmedAppointments = new List<UpcomingAppointment>();
        }

        public IList<Referral> ReferralsNotInReview { get; set; }

        public IList<Referral> ReferralsInReview { get; set; }

        public IList<UpcomingAppointment> UnconfirmedAppointments { get; set; }

        public IList<UpcomingAppointment> ConfirmedAppointments { get; set; }

        [JsonIgnore]
        public int AppointmentCount { get; set; }

        [JsonIgnore]
        public int ReferralCount { get; set; }

        public void AddReferralNotInReview(Referral referral)
        {
            ReferralCount = ReferralCount + 1;
            ReferralsNotInReview.Add(referral);
        }

        public void AddReferralInReview(Referral referral)
        {
            ReferralCount = ReferralCount + 1;
            ReferralsInReview.Add(referral);
        }

        public void AddUnconfirmedAppointment(UpcomingAppointment appointment)
        {
            AppointmentCount = AppointmentCount + 1;
            UnconfirmedAppointments.Add(appointment);
        }

        public void AddConfirmedAppointment(UpcomingAppointment appointment)
        {
            AppointmentCount = AppointmentCount + 1;
            ConfirmedAppointments.Add(appointment);
        }

        public void Sort()
        {
            ReferralsNotInReview = ReferralsNotInReview
                .OrderBy(r => r.ReferredDateTime)
                .ToList();
            ReferralsInReview = ReferralsInReview
                .OrderBy(r => r.ReferredDateTime)
                .ToList();
            UnconfirmedAppointments = UnconfirmedAppointments
                .OrderBy(a => a.AppointmentDateTime)
                .ToList();
            ConfirmedAppointments = ConfirmedAppointments
                .OrderBy(a => a.IsCancelled)
                .ThenBy(a => a.AppointmentDateTime)
                .ToList();
        }
    }
}