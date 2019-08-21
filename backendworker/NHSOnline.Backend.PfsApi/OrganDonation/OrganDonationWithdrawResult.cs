using NHSOnline.Backend.PfsApi.Areas.OrganDonation;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public abstract class OrganDonationWithdrawResult
    {
        public abstract T Accept<T>(IOrganDonationWithdrawResultVisitor<T> visitor);

        public class SuccessfullyWithdrawn : OrganDonationWithdrawResult
        {
            public override T Accept<T>(IOrganDonationWithdrawResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class SystemError : OrganDonationWithdrawResult
        {
            public override T Accept<T>(IOrganDonationWithdrawResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class UpstreamError : OrganDonationWithdrawResult
        {
            public IApiErrorResponse Response { get; }
            
            public UpstreamError(IApiErrorResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IOrganDonationWithdrawResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class Timeout : OrganDonationWithdrawResult
        {
            public override T Accept<T>(IOrganDonationWithdrawResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}