using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class SummaryResponse : ISummaryResponse
    {
        public SummaryResponse()
        {
            ActionableReferralsAndAppointments = new List<SecondaryCareSummaryItem>();
            ConfirmedAppointments = new List<UpcomingAppointment>();
            ReferralsInReviewNotOverdue = new List<Referral>();
        }

        public IList<SecondaryCareSummaryItem> ActionableReferralsAndAppointments { get; set; }

        public IList<UpcomingAppointment> ConfirmedAppointments { get; set; }

        public IList<Referral> ReferralsInReviewNotOverdue { get; set; }

        [JsonIgnore]
        public int AppointmentCount { get; set; }
        [JsonIgnore]
        public int ReferralCount { get; set; }

        public void AddActionableReferral(Referral referral)
        {
            ReferralCount = ReferralCount + 1;
            ActionableReferralsAndAppointments.Add(referral);
        }

        public void AddActionableAppointment(UpcomingAppointment appointment)
        {
            AppointmentCount = AppointmentCount + 1;
            ActionableReferralsAndAppointments.Add(appointment);
        }

        public void AddConfirmedAppointment(UpcomingAppointment appointment)
        {
            AppointmentCount = AppointmentCount + 1;
            ConfirmedAppointments.Add(appointment);
        }

        public void AddReferralInReviewNotOverdue(Referral referral)
        {
            ReferralCount = ReferralCount + 1;
            ReferralsInReviewNotOverdue.Add(referral);
        }

        public void Sort()
        {
            // Earliest first
            static int CompareAppointmentTimes(UpcomingAppointment a1, UpcomingAppointment a2) =>
                a1.AppointmentDateTime is null || a2.AppointmentDateTime is null
                    ? 0
                    : a1.AppointmentDateTime.Value.CompareTo(a2.AppointmentDateTime.Value);

            // Appointments before referrals, then order by referred date or appointment date
            var actionableItemComparer = Comparer<SecondaryCareSummaryItem>.Create((x1, x2) =>
                x1 switch
                {
                    UpcomingAppointment _ when x2 is Referral => -1,
                    UpcomingAppointment a1 when x2 is UpcomingAppointment a2 => CompareAppointmentTimes(a1, a2),
                    Referral r1 when x2 is Referral r2 => r1.CompareTo(r2),
                    _ => 0,
                });

            ActionableReferralsAndAppointments = ActionableReferralsAndAppointments
                .OrderBy(x => x, actionableItemComparer)
                .ToList();

            // Booked appointments before cancelled, then order by date
            ConfirmedAppointments = ConfirmedAppointments
                .OrderBy(a => a.IsCancelled)
                .ThenBy(a => a.AppointmentDateTime)
                .ToList();

            ReferralsInReviewNotOverdue = ReferralsInReviewNotOverdue
                .OrderBy(r => r.ReferredDateTime)
                .ToList();
        }
    }
}