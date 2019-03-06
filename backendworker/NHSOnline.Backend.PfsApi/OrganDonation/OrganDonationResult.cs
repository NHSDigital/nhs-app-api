using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public abstract class OrganDonationResult
    {
        public abstract T Accept<T>(IOrganDonationResultVisitor<T> visitor);

        public class NewRegistration : OrganDonationResult
        {
            public OrganDonationRegistration Registration { get; }

            public NewRegistration(OrganDonationRegistration registration)
            {
                Registration = registration;
            }

            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class ExistingRegistration : OrganDonationResult
        {
            public OrganDonationRegistration Registration { get; }

            public ExistingRegistration(OrganDonationRegistration registration)
            {
                Registration = registration;
            }

            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class DemographicsBadGateway : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class DemographicsRetrievalFailed : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class DemographicsForbidden : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class DemographicsInternalServerError : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class SearchTimeout : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class SearchError : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class SearchUpstreamError : OrganDonationResult
        {
            public ApiErrorResponse Response { get; }

            public SearchUpstreamError(ApiErrorResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}