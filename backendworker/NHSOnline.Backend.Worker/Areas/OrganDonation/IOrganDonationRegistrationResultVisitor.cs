using NHSOnline.Backend.Worker.OrganDonation;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation
{
    public interface IOrganDonationRegistrationResultVisitor<out T>
    {
        T Visit(OrganDonationRegistrationResult.SuccessfullyRegistered result);
        T Visit(OrganDonationRegistrationResult.Timeout result);
        T Visit(OrganDonationRegistrationResult.SystemError result);
        T Visit(OrganDonationRegistrationResult.UpstreamError result);
    }
}