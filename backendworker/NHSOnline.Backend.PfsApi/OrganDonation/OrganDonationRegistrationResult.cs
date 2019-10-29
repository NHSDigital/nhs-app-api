using NHSOnline.Backend.PfsApi.Areas.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public abstract class OrganDonationRegistrationResult
    {
        public abstract T Accept<T>(IOrganDonationRegistrationResultVisitor<T> visitor);

        public class SuccessfullyRegistered : OrganDonationRegistrationResult
        {
            public OrganDonationRegistrationResponse Response { get; }

            public SuccessfullyRegistered(OrganDonationRegistrationResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IOrganDonationRegistrationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class SystemError : OrganDonationRegistrationResult
        {
            public override T Accept<T>(IOrganDonationRegistrationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class UpstreamError : OrganDonationRegistrationResult
        {
            public IApiErrorResponse Response { get; }

            public UpstreamError(IApiErrorResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IOrganDonationRegistrationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class Timeout : OrganDonationRegistrationResult
        {
            public override T Accept<T>(IOrganDonationRegistrationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class BadRequest : OrganDonationRegistrationResult
        {
            public override T Accept<T>(IOrganDonationRegistrationResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}