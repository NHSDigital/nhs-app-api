using NHSOnline.Backend.Worker.OrganDonation;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation
{
    public interface IOrganDonationReferenceDataResultVisitor<out T>
    {
        T Visit(OrganDonationReferenceDataResult.SuccessfullyRetrieved result);
        T Visit(OrganDonationReferenceDataResult.SystemError result);
        T Visit(OrganDonationReferenceDataResult.UpstreamError result);
        T Visit(OrganDonationReferenceDataResult.Timeout result);
    }
}