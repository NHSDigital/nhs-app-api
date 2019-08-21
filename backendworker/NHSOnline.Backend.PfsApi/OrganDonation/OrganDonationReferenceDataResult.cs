using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public abstract class OrganDonationReferenceDataResult
    {
        public abstract T Accept<T>(IOrganDonationReferenceDataResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : OrganDonationReferenceDataResult
        {
            public OrganDonationReferenceDataResponse Response { get; }

            public SuccessfullyRetrieved(OrganDonationReferenceDataResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IOrganDonationReferenceDataResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class UpstreamError : OrganDonationReferenceDataResult
        {
            public IApiErrorResponse Response { get; }

            public UpstreamError(IApiErrorResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IOrganDonationReferenceDataResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class SystemError : OrganDonationReferenceDataResult
        {
            public override T Accept<T>(IOrganDonationReferenceDataResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class Timeout : OrganDonationReferenceDataResult
        {
            public override T Accept<T>(IOrganDonationReferenceDataResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}