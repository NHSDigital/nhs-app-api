using System;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class Referral
    {
        public string ReferralId { get; set; }

        public DateTimeOffset ReferredDateTime { get; set; }

        public string ServiceSpeciality { get; set; }

        public string ReferrerOrganisation { get; set; }

        public string Status { get; set; }

        public DateTimeOffset? ReviewDueDate { get; set; }

        public string Provider { get; set; }
    }
}