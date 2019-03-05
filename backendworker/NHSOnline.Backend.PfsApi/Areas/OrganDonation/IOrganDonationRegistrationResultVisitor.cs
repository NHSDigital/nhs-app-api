using NHSOnline.Backend.PfsApi.OrganDonation;

namespace NHSOnline.Backend.PfsApi.Areas.OrganDonation
{
    public interface IOrganDonationRegistrationResultVisitor<out T>
    {
        T Visit(OrganDonationRegistrationResult.SuccessfullyRegistered result);
        T Visit(OrganDonationRegistrationResult.Timeout result);
        T Visit(OrganDonationRegistrationResult.SystemError result);
        T Visit(OrganDonationRegistrationResult.UpstreamError result);
    }
}