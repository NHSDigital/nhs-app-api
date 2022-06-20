using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class Referral : SecondaryCareSummaryItem
    {
        public override string ItemType => SummaryItemType.Referral.ToString();

        public string ReferralId { get; set; }

        public DateTimeOffset ReferredDateTime { get; set; }

        public string ServiceSpecialty { get; set; }

        public string ReferrerOrganisation { get; set; }

        public string Status { get; set; }

        public DateTimeOffset? ReviewDueDate { get; set; }

        public string Provider { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Intentional; we wish to expose this as a string, do not intend to parse the URL")]
        public string DeepLinkUrl { get; set; }

        [JsonIgnore]
        public bool IsInReview => string.Equals(ReferralStatus.inReview.ToString(), Status, StringComparison.Ordinal);

        [JsonIgnore]
        public bool IsOverdue => ReviewDueDate.HasValue && ReviewDueDate.Value.Date.CompareTo(DateTime.Today) < 0;

        public int CompareTo(Referral referral) =>
            ReferredDateTime.CompareTo(referral.ReferredDateTime);
    }
}