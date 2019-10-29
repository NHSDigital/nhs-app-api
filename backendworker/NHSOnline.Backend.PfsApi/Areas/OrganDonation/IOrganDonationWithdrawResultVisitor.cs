using NHSOnline.Backend.PfsApi.OrganDonation;

namespace NHSOnline.Backend.PfsApi.Areas.OrganDonation
{
    public interface IOrganDonationWithdrawResultVisitor<out T>
    {
        T Visit(OrganDonationWithdrawResult.SuccessfullyWithdrawn result);
        T Visit(OrganDonationWithdrawResult.Timeout result);
        T Visit(OrganDonationWithdrawResult.SystemError result);
        T Visit(OrganDonationWithdrawResult.UpstreamError result);
        T Visit(OrganDonationWithdrawResult.BadRequest result);
    }
}