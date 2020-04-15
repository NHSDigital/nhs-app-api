using System;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.Support
{
    [Serializable]
    public sealed class P9UserSession: P5UserSession
    {
        public GpUserSession GpUserSession { get; set; }

        public Guid OrganDonationSessionId { get; set; }

        public string Im1ConnectionToken { get; set; }

        public override TResult Accept<TResult>(IUserSessionVisitor<TResult> visitor)
            => visitor.Visit(this);
    }
}