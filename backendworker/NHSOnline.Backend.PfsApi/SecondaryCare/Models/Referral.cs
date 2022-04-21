using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class Referral
    {
        public string ReferralId { get; set; }

        public DateTimeOffset ReferredDateTime { get; set; }

        public string ServiceSpecialty { get; set; }

        public string ReferrerOrganisation { get; set; }

        public string Status { get; set; }

        public DateTimeOffset? ReviewDueDate { get; set; }

        public string Provider { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Intentional; we wish to expose this as a string, do not intend to parse the URL")]
        public string DeepLinkUrl { get; set; }
    }
}